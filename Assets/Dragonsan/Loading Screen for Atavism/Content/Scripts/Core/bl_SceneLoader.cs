using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;
#if AT_STREAMER2
using WorldStreamer2;
#endif

namespace Atavism
{
    [RequireComponent(typeof(AudioSource))]
    public class bl_SceneLoader : MonoBehaviour
    {
        [Header("Settings")]
        public SceneSkipType SkipType = SceneSkipType.Button;
        [Range(0.5f, 7)] public float SceneSmoothLoad = 3;
        [Range(0.5f, 7)] public float FadeInSpeed = 2;
        [Range(0.5f, 7)] public float FadeOutSpeed = 2;
        public bool useTimeScale = false;
        [Header("Background")]
        public bool useBackgrounds = true;
        public bool ShowDescription = true;
        [Range(1, 60)] public float TimePerBackground = 5;
        [Range(0.5f, 7)] public float FadeBackgroundSpeed = 2;
        [Range(0.5f, 5)] public float TimeBetweenBackground = 0.5f;
        [Header("Tips")]
        public bool RandomTips = false;
        [Range(1, 60)] public float TimePerTip = 5;
        [Range(0.5f, 5)] public float FadeTipsSpeed = 2;
        [Header("Loading")]
        public bool FadeLoadingBarOnFinish = false;
        [Range(50, 1000)] public float LoadingCircleSpeed = 300;
        [TextArea(2, 2)] public string LoadingTextFormat = "{0}";

        [Header("Audio")]
        [Range(0.1f, 1)] public float AudioVolume = 1f;
        [Range(0.5f, 5)] public float FadeAudioSpeed = 0.5f;
        [Range(0.1f, 1)] public float FinishSoundVolume = 0.5f;
        [SerializeField] private AudioClip FinishSound = null;
        [SerializeField] private AudioClip BackgroundAudio = null;

        [Header("References")]
        [SerializeField] private Text SceneNameText = null;
        [SerializeField] private TextMeshProUGUI TMPSceneNameText = null;
        [SerializeField] private Text DescriptionText = null;
        [SerializeField] private TextMeshProUGUI TMPDescriptionText = null;
        [SerializeField] private Text ProgressText = null;
        [SerializeField] private TextMeshProUGUI TMPProgressText = null;
        [SerializeField] private Text TipText = null;
        [SerializeField] private TextMeshProUGUI TMPTipText = null;
        [SerializeField] private Image BackgroundImage = null;
        [SerializeField] private Image FilledImage = null;
        [SerializeField] private Slider LoadBarSlider = null;
        [SerializeField] private GameObject ContinueUI = null;
        [SerializeField] private GameObject RootUI;
        [SerializeField] private GameObject FlashImage = null;
        [SerializeField] private GameObject SkipKeyText = null;
        [SerializeField] private RectTransform LoadingCircle = null;
        [SerializeField] private CanvasGroup LoadingCircleCanvas = null;
        [SerializeField] private CanvasGroup FadeImageCanvas = null;

        private bl_SceneLoaderManager Manager = null;
        private AsyncOperation async;
        private bool isOperationStarted = false;
        private bool FinishLoad = false;
        private CanvasGroup RootAlpha;
        private CanvasGroup BackgroundAlpha = null;
        private CanvasGroup LoadingBarAlpha = null;
        private bool isTipFadeOut = false;
        private int CurrentTip = 0;
        private TheTipList cacheTips = null;
        private int CurrentBackground = 0;
        private List<Sprite> cacheBackgrounds = new List<Sprite>();
        private AudioSource Source = null;
        private float lerpValue = 0;
        private bool canSkipWithKey = false;
        private bl_SceneLoaderInfo CurrentLoadLevel = null;
#if AT_STREAMER || AT_STREAMER2
        [SerializeField] private List<Streamer> streamers;//Dragonsan
#endif
        bool load_leval = false;
        float fillAmount = 0f;
        public UnityEvent onDone;
        bool teleport = false;
        bool loadedMainScene = false;
        void Start()
        {
#if AT_STREAMER || AT_STREAMER2
            streamers = new List<Streamer>();
#endif
            // Atavism Link
            AtavismClient.Instance.LoadLevelAsync = LoadLevel;
            if (LoadBarSlider != null)
            {
                LoadingBarAlpha = LoadBarSlider.GetComponent<CanvasGroup>();
            }
            if (BackgroundImage != null)
            {
                BackgroundAlpha = BackgroundImage.GetComponent<CanvasGroup>();
            }

            AtavismEventSystem.RegisterEvent("PLAYER_TELEPORTED", this);
            if (ProgressText != null)
                ProgressText.text = "0";
            if (TMPProgressText != null)
                TMPProgressText.text = "0";
        }
        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("PLAYER_TELEPORTED", this);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
          //  Debug.LogWarning("OnSceneLoaded: Scene Loaded " + scene.name);
            if (sceneNameInLoading.Length > 0)
            {
                if (scene.name.Equals(sceneNameInLoading))
                    loadedMainScene = true;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        void Awake()
        {
            Manager = bl_SceneLoaderUtils.GetSceneLoaderManager();
            SceneManager.sceneLoaded += OnSceneLoaded;
            //Setup Audio Source
            Source = GetComponent<AudioSource>();
            Source.volume = 0;
            Source.loop = true;
            if (BackgroundAudio != null)
            {
                Source.clip = BackgroundAudio;
            }

            //Setup UI
            RootUI.SetActive(false);
            RootAlpha = RootUI.GetComponent<CanvasGroup>();
            if (ContinueUI != null)
            {
                ContinueUI.SetActive(false);
            }
            if (FlashImage != null)
            {
                FlashImage.SetActive(false);
            }
            if (FadeImageCanvas != null)
            {
                FadeImageCanvas.alpha = 1;
                StartCoroutine(FadeOutCanvas(FadeImageCanvas));
            }
            if (SkipKeyText != null)
            {
                SkipKeyText.SetActive(false);
            }
            if (LoadBarSlider != null)
            {
                LoadingBarAlpha = LoadBarSlider.GetComponent<CanvasGroup>();
                LoadBarSlider.value = 0;
            }
            if (BackgroundImage != null)
            {
                BackgroundAlpha = BackgroundImage.GetComponent<CanvasGroup>();
            }
            if (Manager.HasTips)
            {
                cacheTips = Manager.TipList;
            }
            if (FilledImage != null)
            {
                FilledImage.type = Image.Type.Filled;
                FilledImage.fillAmount = 0;
            }
            transform.SetAsLastSibling();
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (!isOperationStarted)
                return;
            if (teleport)
                LoadingRotator();
            if (async == null)
                return;

            UpdateUI();
            LoadingRotator();
            SkipWithKey();
        }

        /// <summary>
        /// 
        /// </summary>
        void SkipWithKey()
        {
            if (!canSkipWithKey)
                return;

            if (Input.anyKeyDown)
            {
                LoadNextScene();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateUI()
        {
            if (CurrentLoadLevel.LoadingType == LoadingType.Async)
            {
                //Get progress of load level
                float Extra = (GetSkipType == SceneSkipType.InstantComplete) ? 0 : 0.1f;
                float p = (async.progress + Extra); //Fix problem of 90%
                lerpValue = Mathf.Lerp(lerpValue, p, DeltaTime * SceneSmoothLoad);
                if (async.isDone || lerpValue > 0.99f)
                {
                    //Called one time what is inside in this function.
                    if (!FinishLoad)
                    {
                        OnFinish();
                    }
                }
            }
            else
            {
                if (lerpValue >= 1)
                {
                    //Called one time what is inside in this function.
                    if (!FinishLoad)
                    {
                        OnFinish();
                    }
                }
            }
            if (FilledImage != null)
            {
                FilledImage.fillAmount = lerpValue;
            }
            if (LoadBarSlider != null)
            {
                LoadBarSlider.value = lerpValue;
            }
            if (ProgressText != null)
            {
                string percent = (lerpValue * 100).ToString("F0");
                ProgressText.text = string.Format(LoadingTextFormat, percent);
            }
            if (TMPProgressText != null)
            {
                string percent = (lerpValue * 100).ToString("F0");
                TMPProgressText.text = string.Format(LoadingTextFormat, percent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void LoadingRotator()
        {
            if (LoadingCircle == null)
                return;

            LoadingCircle.Rotate(-Vector3.forward * DeltaTime * LoadingCircleSpeed);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnFinish()
        {
            FinishLoad = true;
            if (FlashImage != null)
            {
                FlashImage.SetActive(true);
            }
            //Can skip when next level is loaded.
            if (GetSkipType == SceneSkipType.Button)
            {
                if (ContinueUI != null)
                {
                    ContinueUI.SetActive(true);
                }
                if (FinishSound != null)
                {
                    AudioSource.PlayClipAtPoint(FinishSound, transform.position, FinishSoundVolume);
                }
                if (LoadingCircleCanvas != null)
                {
                    StartCoroutine(FadeOutCanvas(LoadingCircleCanvas, 0.5f));
                }
                if (LoadingBarAlpha != null && FadeLoadingBarOnFinish)
                {
                    StartCoroutine(FadeOutCanvas(LoadingBarAlpha, 1));
                }

            }
            else if (GetSkipType == SceneSkipType.Instant)
            {
                LoadNextScene();
            }
            else if (GetSkipType == SceneSkipType.InstantComplete)
            {
                LoadNextScene();
            }
            else if (GetSkipType == SceneSkipType.AnyKey)
            {
                canSkipWithKey = true;
                if (SkipKeyText != null)
                {
                    SkipKeyText.SetActive(true);
                }
                if (FinishSound != null)
                {
                    AudioSource.PlayClipAtPoint(FinishSound, transform.position, FinishSoundVolume);
                }
                if (LoadingCircleCanvas != null)
                {
                    StartCoroutine(FadeOutCanvas(LoadingCircleCanvas, 0.5f));
                }
                if (LoadingBarAlpha != null && FadeLoadingBarOnFinish)
                {
                    StartCoroutine(FadeOutCanvas(LoadingBarAlpha, 1));
                }

            }

        }
        string sceneNameInLoading = "";
        /// <summary>
        /// 
        /// </summary>
        public void LoadLevel(string level)
        {
            StopAllCoroutines();
            sceneNameInLoading = level;
            load_leval = true;
            FinishLoad = false;
            teleport = false;
            loadedMainScene = false;
            if (LoadBarSlider != null)
            {
                LoadBarSlider.value = 0;
            }
            if (ProgressText != null)
            {
                ProgressText.text = string.Format(LoadingTextFormat, 0);
            }
            if (TMPProgressText != null)
            {
                TMPProgressText.text = string.Format(LoadingTextFormat, 0);
            }
            if (FilledImage != null)
            {
                FilledImage.type = Image.Type.Filled;
                FilledImage.fillAmount = 0;
            }
            StartCoroutine(FadeOutCanvas(FadeImageCanvas));
            if (LoadingCircleCanvas != null)
            {
                StartCoroutine(FadeInCanvas(LoadingCircleCanvas, 0.1f));
            }
            CurrentLoadLevel = Manager.GetSceneInfo(level);
            if (CurrentLoadLevel == null)
                return;

            SetupUI(CurrentLoadLevel);
            StartCoroutine(StartAsyncOperation(CurrentLoadLevel.SceneName));
            if (CurrentLoadLevel.LoadingType == LoadingType.Fake)
            {
                StartCoroutine(StartFakeLoading());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        void SetupUI(bl_SceneLoaderInfo info)
        {
            if (BackgroundImage != null && useBackgrounds)
            {
                if (info.Backgrounds != null && info.Backgrounds.Length > 1)
                {
                    cacheBackgrounds.AddRange(info.Backgrounds);
                    for (int i = 0; i < cacheBackgrounds.Count; i++)
                    {
                        Sprite temp = cacheBackgrounds[i];
                        int randomIndex = Random.Range(i, cacheBackgrounds.Count);
                        cacheBackgrounds[i] = cacheBackgrounds[randomIndex];
                        cacheBackgrounds[randomIndex] = temp;
                    }
                    StartCoroutine(BackgroundTransition());
                    BackgroundImage.color = Color.white;
                }
                else if (info.Backgrounds != null && info.Backgrounds.Length > 0)
                {
                    BackgroundImage.sprite = info.Backgrounds[0];
                    BackgroundImage.color = Color.white;
                }
            }
            if (SceneNameText != null)
            {
                SceneNameText.text = info.DisplayName;
            }
            if (TMPSceneNameText != null)
            {
                TMPSceneNameText.text = info.DisplayName;
            }
            if (DescriptionText != null)
            {
                if (ShowDescription)
                {
                    DescriptionText.text = info.Description;
                }
                else
                {
                    DescriptionText.text = string.Empty;
                }
            }
            if (TMPDescriptionText != null)
            {
                if (ShowDescription)
                {
                    TMPDescriptionText.text = info.Description;
                }
                else
                {
                    TMPDescriptionText.text = string.Empty;
                }
            }
            if (LoadBarSlider != null)
            {
                LoadBarSlider.value = 0;
            }
            if (ProgressText != null)
            {
                ProgressText.text = string.Format(LoadingTextFormat, 0);
            }
            if (TMPProgressText != null)
            {
                TMPProgressText.text = string.Format(LoadingTextFormat, 0);
            }
            if (Manager.HasTips && (TipText != null || TMPTipText!=null))
            {
                if (RandomTips)
                {
                    CurrentTip = Random.Range(0, cacheTips.Count);
                    if (TipText != null)
                        TipText.text = cacheTips[CurrentTip];
                    if (TMPTipText != null)
                        TMPTipText.text = cacheTips[CurrentTip];
                }
                else
                {
                    if (TipText != null)
                        TipText.text = cacheTips[0];
                    if (TMPTipText != null)
                        TMPTipText.text = cacheTips[0];
                }
                StartCoroutine(TipsLoop());
            }
            //Show all UI
            RootAlpha.alpha = 0;
            RootUI.SetActive(true);

            //start audio loop
            Source.Play();
            StartCoroutine(FadeAudio(true));
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadNextScene()
        {
            //fade audio loop
            StartCoroutine(FadeAudio(false));
            //StartCoroutine(LoadNextSceneIE());
            StartCoroutine(SceneCheck());
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator StartAsyncOperation(string level)
        {
            while (RootAlpha.alpha < 1)
            {
                RootAlpha.alpha += DeltaTime * FadeInSpeed;
                yield return null;
            }
            async = bl_SceneLoaderUtils.LoadLevelAsync(level);
            if (GetSkipType != SceneSkipType.InstantComplete || CurrentLoadLevel.LoadingType == LoadingType.Fake)
            {
                async.allowSceneActivation = false;
            }
            else
            {
                async.allowSceneActivation = true;
            }
            isOperationStarted = true;
            yield return async;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator StartFakeLoading()
        {
            lerpValue = 0;
            while (lerpValue < 1)
            {
                lerpValue += Time.deltaTime / CurrentLoadLevel.FakeLoadingTime;
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator BackgroundTransition()
        {
            while (true)
            {
                BackgroundImage.sprite = cacheBackgrounds[CurrentBackground];
                while (BackgroundAlpha.alpha < 1)
                {
                    BackgroundAlpha.alpha += DeltaTime * FadeBackgroundSpeed * 0.8f;
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(TimePerBackground);
                while (BackgroundAlpha.alpha > 0)
                {
                    BackgroundAlpha.alpha -= DeltaTime * FadeBackgroundSpeed;
                    yield return new WaitForEndOfFrame();
                }
                CurrentBackground = (CurrentBackground + 1) % cacheBackgrounds.Count;
                yield return new WaitForSeconds(TimeBetweenBackground);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator TipsLoop()
        {
            if (TipText == null && TMPTipText == null)
                yield break;
           
            Color alpha = Color.white;
            if (TipText != null)
                alpha = TipText.color;
            if (TMPTipText != null)
                alpha = TMPTipText.color;
            if (isTipFadeOut)
            {
                while (alpha.a < 1)
                {
                    alpha.a += DeltaTime * FadeTipsSpeed;
                    if (TipText != null)
                        TipText.color = alpha;
                    if (TMPTipText != null)
                        TMPTipText.color = alpha;
                    yield return null;
                }
                StartCoroutine(WaitNextTip(TimePerTip));
            }
            else
            {
                while (alpha.a > 0)
                {
                    alpha.a -= DeltaTime * FadeTipsSpeed;
                    if (TipText != null)
                        TipText.color = alpha;
                    if (TMPTipText != null)
                        TMPTipText.color = alpha;
                    yield return null;
                }
                StartCoroutine(WaitNextTip(0.75f));
            }
            if (isTipFadeOut)
            {
                if (RandomTips)
                {
                    int lastTip = CurrentTip;
                    CurrentTip = Random.Range(0, cacheTips.Count);
                    while (CurrentTip == lastTip)
                    {
                        CurrentTip = Random.Range(0, cacheTips.Count);
                        yield return null;
                    }
                    if (TipText != null)
                        TipText.text = cacheTips[CurrentTip];
                    if (TMPTipText != null)
                        TMPTipText.text = cacheTips[CurrentTip];
                }
                else
                {
                    CurrentTip = (CurrentTip + 1) % cacheTips.Count;
                    if (TipText != null)
                        TipText.text = cacheTips[CurrentTip];
                    if (TMPTipText != null)
                        TMPTipText.text = cacheTips[CurrentTip];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator FadeAudio(bool FadeIn)
        {
            if (BackgroundAudio == null)
                yield break;

            if (FadeIn)
            {
                while (Source.volume < AudioVolume)
                {
                    Source.volume += DeltaTime * FadeAudioSpeed;
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                while (Source.volume > 0)
                {
                    Source.volume -= DeltaTime * FadeAudioSpeed * 3;
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        IEnumerator WaitNextTip(float t)
        {
            isTipFadeOut = !isTipFadeOut;
            yield return new WaitForSeconds(t);
            StartCoroutine(TipsLoop());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadNextSceneIE()
        {
            load_leval = false;

            FadeImageCanvas.alpha = 0;
            while (FadeImageCanvas.alpha < 1)
            {
                FadeImageCanvas.alpha += DeltaTime * FadeInSpeed;
                yield return null;
            }
            if (async != null)
                async.allowSceneActivation = true;
            RootUI.SetActive(false);
            isOperationStarted = false;
            async = null;
            teleport = false;
            loadedMainScene = false;
            sceneNameInLoading = "";
            //Debug.LogError("LoadingScreen Disable Screen");
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator FadeOutCanvas(CanvasGroup alpha, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            while (alpha.alpha > 0)
            {
               // Debug.LogWarning("FadeOutCanvas "+ alpha.name+" "+ alpha.alpha);
                alpha.alpha -= DeltaTime * FadeOutSpeed;
                yield return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator FadeInCanvas(CanvasGroup alpha, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            while (alpha.alpha < 1)
            {
              //  Debug.LogWarning("FadeInCanvas " + alpha.name + " " + alpha.alpha);

                alpha.alpha += DeltaTime * FadeInSpeed;
                yield return null;
            }
        }

        private SceneSkipType GetSkipType
        {
            get
            {
                if (CurrentLoadLevel != null)
                {
                    if (CurrentLoadLevel.SkipType != SceneSkipType.None)
                    {
                        return CurrentLoadLevel.SkipType;
                    }
                }
                if (SkipType != SceneSkipType.None)
                {
                    return SkipType;
                }
                else
                {
                    return SceneSkipType.AnyKey;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private float DeltaTime
        {
            get
            {
                return (useTimeScale) ? Time.deltaTime : Time.unscaledDeltaTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eData"></param>
        public void OnEvent(AtavismEventData eData)
        {
            AtavismLogger.LogDebugMessage("Scene Loader " + eData.eventType + " " + load_leval);

            if (eData.eventType == "PLAYER_TELEPORTED" && !load_leval)
            {
                FinishLoad = false;
                isOperationStarted = true;
                teleport = true;
                StartCoroutine(FadeOutCanvas(FadeImageCanvas));
                if (LoadingCircleCanvas != null)
                {
                    StartCoroutine(FadeInCanvas(LoadingCircleCanvas, 0.1f));
                }
                if (BackgroundImage != null && useBackgrounds)
                {
                    if (cacheBackgrounds.Count > 1)
                    {
                        for (int i = 0; i < cacheBackgrounds.Count; i++)
                        {
                            Sprite temp = cacheBackgrounds[i];
                            int randomIndex = Random.Range(i, cacheBackgrounds.Count);
                            cacheBackgrounds[i] = cacheBackgrounds[randomIndex];
                            cacheBackgrounds[randomIndex] = temp;

                        }
                        CurrentBackground = 0;
                        BackgroundImage.color = Color.white;
                    }
                    else
                    {
                        BackgroundImage.color = Color.white;
                    }
                }
                if (LoadBarSlider != null)
                {
                    LoadBarSlider.value = 0;
                }
                if (ProgressText != null)
                {
                    ProgressText.text = string.Format(LoadingTextFormat, 0);
                }
         
                if (Manager.HasTips && ( TipText != null || TMPTipText!=null))
                {
                    if (RandomTips)
                    {
                        CurrentTip = Random.Range(0, cacheTips.Count);
                        if (TipText != null)
                            TipText.text = cacheTips[CurrentTip];
                        if (TMPTipText != null)
                            TMPTipText.text = cacheTips[CurrentTip];
                    }
                    else
                    {
                        if (TipText != null)
                            TipText.text = cacheTips[0];
                        if (TMPTipText != null)
                            TMPTipText.text = cacheTips[0];
                    }
                    StartCoroutine(TipsLoop());
                }
                //Show all UI
                RootAlpha.alpha = 1;
                RootUI.SetActive(true);

                StartCoroutine(SceneCheckTeleport());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator SceneCheckTeleport()
        {
            AtavismLogger.LogDebugMessage("Loading screen stremer SceneCheckTeleport");
            //fade audio loop
            WaitForSeconds delay = new WaitForSeconds(1f);
            yield return delay;
#if AT_STREAMER || AT_STREAMER2
            GameObject[] gos = GameObject.FindGameObjectsWithTag("SceneStreamer");
            foreach (GameObject go in gos)
            {
                Streamer s = go.GetComponent<Streamer>();
                if (s != null)
                {
                    streamers.Add(s);
                    s.showLoadingScreen = true;
                }
            }
            AtavismLogger.LogDebugMessage("Loading screen  streamer LoadNextScene count streamers " + streamers.Count);

            yield return delay;
#endif
            StartCoroutine(StreamerCheck());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator SceneCheck()
        {
            AtavismLogger.LogDebugMessage("Loading screen  streamer SceneCheck");
            //fade audio loop
            //  StartCoroutine(FadeAudio(false));
            async.allowSceneActivation = true;
            WaitForSeconds delay = new WaitForSeconds(1f);
            yield return delay;
#if AT_STREAMER || AT_STREAMER2
            GameObject[] gos = GameObject.FindGameObjectsWithTag("SceneStreamer");
            foreach (GameObject go in gos)
            {
                Streamer s = go.GetComponent<Streamer>();
                if (s != null)
                {
                    streamers.Add(s);
                    s.showLoadingScreen = true;
                }
            }
            AtavismLogger.LogDebugMessage("Loading screen  streamer SceneCheck count streamers " + streamers.Count);

         
#endif
            yield return delay;
            StartCoroutine(StreamerCheck());

        }


        IEnumerator StreamerCheck()
        {
            AtavismLogger.LogDebugMessage("Loading screen StreamerCheck  ");
            fillAmount = 0f;
            WaitForSeconds delay2 = new WaitForSeconds(2f);
#if AT_STREAMER || AT_STREAMER2
            WaitForSeconds delay = new WaitForSeconds(0.3f);
             if (sceneNameInLoading.Length > 0)
            {
                WaitForSeconds delay3 = new WaitForSeconds(0.3f);
                while (!loadedMainScene)
                {
                    Debug.LogWarning("Loading screen wait");
                    yield return delay3;
                }
            }
            if (streamers != null && streamers.Count > 0 && RootUI.activeSelf)
            {
                bool initialized = true;
                bool run = true;
                while (run)
                {
                    while (fillAmount < 1)
                    {
                        AtavismLogger.LogDebugMessage("Loading screen + streamer StreamerCheck " + fillAmount);
                        fillAmount = 0f;
                        foreach (var s in streamers)
                        {
                            if (s != null)
                            {
                                fillAmount += s.GetLoadingProgress() / (float)streamers.Count;
                                AtavismLogger.LogDebugMessage("Loading screen + streamer " + s.name + " -> " + s.tilesLoaded + " / " + s.tilesToLoad + "| fillAmount ->" + fillAmount);
                                initialized = initialized && s.initialized;
                            }
                        }
                        if (initialized)
                        {
                            if (fillAmount >= 1)
                            {
                                try
                                {
                                    if (onDone != null)
                                        onDone.Invoke();
                                }
                                catch (System.Exception e)
                                {
                                    AtavismLogger.LogError("Loading screen + streamer StreamerCheck Exception " + e.Message + " " + e);
                                }
                            }
                        }
                        yield return delay;
                    }
                    fillAmount = 0f;
                    yield return delay2;
                    if (streamers.Count > 0)
                        foreach (var s in streamers)
                        {
                            if (s != null)
                            {
                                fillAmount += s.GetLoadingProgress() / (float)streamers.Count;
                                AtavismLogger.LogDebugMessage("Loading screen + streemner " + s.name + " -> " + s.tilesLoaded + " / " + s.tilesToLoad + "| fillAmount ->" + fillAmount);
                                initialized = initialized && s.initialized;
                            }
                        }
                    if (fillAmount >= 1)
                        run = false;
                }
            }
            streamers.Clear();
#else
            if (sceneNameInLoading.Length > 0)
            {
                WaitForSeconds delay3 = new WaitForSeconds(0.3f);
                while (!loadedMainScene)
                {
                    Debug.LogWarning("Loading screen wait");
                    yield return delay3;
                }
            }
            yield return delay2;
#endif

            //  for (int i = 0; i < SceneManager.sceneCount; i++)
            //  {

            // }
            ///  foreach (Scene s in SceneManager.GetAllScenes())
            //  {

            // }

            CurrentBackground = 0;

            if (FinishSound != null)
            {
                AudioSource.PlayClipAtPoint(FinishSound, transform.position, FinishSoundVolume);
            }
            if (LoadingCircleCanvas != null)
            {
                StartCoroutine(FadeOutCanvas(LoadingCircleCanvas, 0.5f));
            }
            if (LoadingBarAlpha != null && FadeLoadingBarOnFinish)
            {
                StartCoroutine(FadeOutCanvas(LoadingBarAlpha, 1));
            }

            StartCoroutine(LoadNextSceneIE());
        }
        /*
            public float waitTime = 2;
            public IEnumerator TurnOff()
            {
                yield return new WaitForSeconds(waitTime);
                RootUI.SetActive(false);
            }
            */
    }
}