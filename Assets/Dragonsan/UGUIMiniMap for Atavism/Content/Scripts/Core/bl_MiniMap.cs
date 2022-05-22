using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
//using UGUIMiniMap;

namespace Atavism
{
    [Serializable]
    public class MapLang
    {
        public string name="";
        public Texture mapTexture;
    }

    public class bl_MiniMap : MonoBehaviour
    {
        [Separator("General Settings")]
        // Target for the minimap.
        public GameObject m_Target;
        public string LevelName;
        [LayerMask]
        public int MiniMapLayer = 10;
        [Tooltip("Keycode to toggle map size mode (world and mini map)")]
        public KeyCode ToogleKey = KeyCode.E;
        public Camera MMCamera = null;
        public RenderType m_Type = RenderType.Picture;
        public RenderMode m_Mode = RenderMode.Mode2D;
        public MapType m_MapType = MapType.Local;
        public bool Ortographic2D = false;
        public Color playerColor = Color.white;
        [Separator("Height")]
        [Range(0.05f, 2)] public float IconMultiplier = 1;
        [Tooltip("How much should we move for each small movement on the mouse wheel?")]
        [Range(1, 10)] public int scrollSensitivity = 3;
        //Default height to view from, if you need have a static height, just edit this.
        [Range(5, 500)]
        public float DefaultHeight = 30;
        [Tooltip("Maximum heights that the camera can reach.")]
        public float MaxZoom = 80;
        [Tooltip("Minimum heights that the camera can reach.")]
        public float MinZoom = 5;
        //If you can that the player cant Increase or decrease, just put keys as "None".
        public KeyCode IncreaseHeightKey = KeyCode.KeypadPlus;
        //If you can that the player cant Increase or decrease, just put keys as "None".
        public KeyCode DecreaseHeightKey = KeyCode.KeypadMinus;
        [Range(1, 15)]
        [Tooltip("Smooth speed to height change.")]
        public float LerpHeight = 8;

        [Separator("Rotation")]
        [Tooltip("Compass rotation for circle maps, rotate icons around pivot.")]
        [CustomToggle("Use Compass Rotation")]
        public bool useCompassRotation = false;
        [Range(25, 500)]
        [Tooltip("Size of Compass rotation diameter.")]
        public float CompassSize = 175f;
        [CustomToggle("Rotation Always in front")]
        public bool RotationAlwaysFront = true;
        [Tooltip("Should the minimap rotate with the player?")]
        [CustomToggle("Dynamic Rotation")]
        public bool DynamicRotation = true;
        [Tooltip("this work only is dynamic rotation.")]
        [CustomToggle("Smooth Rotation")]
        public bool SmoothRotation = true;
        [Range(1, 15)]
        public float LerpRotation = 8;

        public bool AllowMapMarks = true;
        public GameObject MapPointerPrefab;
        public bool AllowMultipleMarks = false;
        private GameObject mapPointer;

        [Separator("Area Grid")]
        public bool ShowAreaGrid = true;
        [Range(1, 20)] public float AreasSize = 4;
        public Material AreaMaterial;

        [Separator("Animations")]
        [CustomToggle("Show Level Name")] public bool ShowLevelName = true;
        [CustomToggle("Show Panel Info")] public bool ShowPanelInfo = true;
        [CustomToggle("Fade OnFull Screen")] public bool FadeOnFullScreen = false;
        [Range(0.1f, 5)] public float HitEffectSpeed = 1.5f;
        public Animator BottonAnimator;
        public Animator PanelInfoAnimator;
        public Animator HitEffectAnimator;

        [Separator("Map Rect")]
        [Tooltip("Position for World Map.")]
        public Vector3 FullMapPosition = Vector2.zero;
        [Tooltip("Rotation for World Map.")]
        public Vector3 FullMapRotation = Vector3.zero;
        [Tooltip("Size of World Map.")]
        public Vector2 FullMapSize = Vector2.zero;

        private Vector3 MiniMapPosition = Vector2.zero;
        private Vector3 MiniMapRotation = Vector3.zero;
        private Vector2 MiniMapSize = Vector2.zero;

        [Space(5)]
        [Tooltip("Smooth Speed for MiniMap World Map transition.")]
        [Range(1, 15)]
        public float LerpTransition = 7;

        [Space(5)]
        [InspectorButton("GetFullMapSize")]
        public string GetWorldRect = "";

        [Separator("Drag Settings")]
        [CustomToggle("Can Drag MiniMap")]
        public bool CanDragMiniMap = true;
        [CustomToggle("Drag Only On Full screen")]
        public bool DragOnlyOnFullScreen = true;
        [CustomToggle("Reset Position On Change")]
        public bool ResetOffSetOnChange = true;
        public Vector2 DragMovementSpeed = new Vector2(0.5f, 0.35f);
        public Vector2 MaxOffSetPosition = new Vector2(1000, 1000);
        public Texture2D DragCursorIcon;
        public Vector2 HotSpot = Vector2.zero;


        [Separator("Picture Mode Settings")]
        [Tooltip("Texture for MiniMap renderer, you can take a snapshot from map.")]
        public Texture MapTexture = null;
        public List<MapLang> MapTextures;
        public Color TintColor = new Color(1, 1, 1, 0.9f);
        public Color SpecularColor = new Color(1, 1, 1, 0.9f);
        public Color EmessiveColor = new Color(0, 0, 0, 0.9f);
        [Range(0.1f, 4)] public float EmissionAmount = 1;
        public Material ReferenceMat;
        [Space(3)]
        public GameObject MapPlane = null;
        public GameObject AreaPrefab;
        public RectTransform WorldSpace = null;
        [Separator("UI")]
        public Canvas m_Canvas = null;
        public RectTransform MMUIRoot = null;
        public Image PlayerIcon = null;
        public Image PlayerCamera = null;

        public CanvasGroup RootAlpha;
        public GameObject ItemPrefabSimple = null;
        public GameObject HoofdPuntPrefab;
        public GameObject ItemPrefab;
        public Dictionary<string, Transform> ItemsList = new Dictionary<string, Transform>();

        //Global variables
        public bool isFullScreen
        {
            get; set;
        }
        public static Camera MiniMapCamera = null;
        public static RectTransform MapUIRoot = null;

        //Drag variables
        private Vector3 DragOffset = Vector3.zero;

        //Privates
        private bool DefaultRotationMode = false;
        private Vector3 DeafultMapRot = Vector3.zero;
        private bool DefaultRotationCircle = false;
        private GameObject plane;
        private GameObject areaInstance;
        private float defaultYCameraPosition;
        const string MMHeightKey = "MinimapCameraHeight";
        private bool getDelayPositionCamera = false;
        private bool isAlphaComplete = false;
        public bool hasError
        {
            get; set;
        }
        private bool isPlanedCreated = false;
        private int orgSlibingIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        void Awake()
        {
            GetMiniMapSize();
            MiniMapCamera = MMCamera;
            MapUIRoot = MMUIRoot;
            DefaultRotationMode = DynamicRotation;
            DeafultMapRot = m_Transform.eulerAngles;
            DefaultRotationCircle = useCompassRotation;

            m_Target = ClientAPI.GetPlayerObject().GameObject;
            AtavismSettings.Instance.MiniMap = this;
            orgSlibingIndex = m_Canvas.transform.GetSiblingIndex();

            PlayerIcon.color = playerColor;
            SetHoofdPunt();
            if (hasError)
                return;
            if (m_Type == RenderType.Picture)
            {
                CreateMapPlane(false);
            }
            else if (m_Type == RenderType.RealTime)
            {
                CreateMapPlane(true);
            }
            if (m_Mode == RenderMode.Mode3D)
            {
                ConfigureCamera3D();
            }
            if (m_MapType == MapType.Local)
            {
                //Get Save Height
                DefaultHeight = PlayerPrefs.GetFloat(MMHeightKey, DefaultHeight);
            }
            else
            {
                ConfigureWorlTarget();
                PlayerIcon.gameObject.SetActive(false);
            }
            if (RootAlpha != null)
            {
                StartCoroutine(StartFade(0));
            }
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);

        }
        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);

        }
        /// <summary>
        /// 
        /// </summary>
        void OnEnable()
        {
            if (!isAlphaComplete)
            {
                if (RootAlpha != null)
                {
                    StartCoroutine(StartFade(0));
                }
            }
        }

        /// <summary>
        /// Create a Plane with Map Texture
        /// MiniMap Camera will be renderer only this plane.
        /// This is more optimizing that RealTime type.
        /// </summary>
        void CreateMapPlane(bool area)
        {
            if (isPlanedCreated)
                return;
            if (MapTexture == null)
            {
                Debug.LogError("Map Texture has not been allocated.");
                return;
            }
            //Get Position reference from world space rect.
            Vector3 pos = WorldSpace.localPosition;
            //Get Size reference from world space rect.
            Vector3 size = WorldSpace.sizeDelta;
            //Set to camera culling only MiniMap Layer.
            if (!area)
            {
                MMCamera.cullingMask = 1 << MiniMapLayer;
                //Create plane
                plane = Instantiate(MapPlane) as GameObject;
                //Set position
                plane.transform.localPosition = pos;
                //Set Correct size.
                plane.transform.localScale = (new Vector3(size.x, 10, size.y) / 10);
                //Apply material with map texture.
                plane.GetComponent<Renderer>().material = CreateMaterial();
                //Apply MiniMap Layer
                plane.layer = MiniMapLayer;
                plane.SetActive(false);
                plane.SetActive(true);
                if (!ShowAreaGrid)
                {
                    plane.transform.GetChild(0).gameObject.SetActive(false);
                }

                Invoke("DelayPositionInvoke", 2);
            }
            else if (AreaPrefab != null && ShowAreaGrid)
            {
                areaInstance = Instantiate(AreaPrefab) as GameObject;
                //Set position
                areaInstance.transform.localPosition = pos;
                //Set Correct size.
                areaInstance.transform.localScale = (new Vector3(size.x, 10, size.y) / 10);
                //Apply MiniMap Layer
                areaInstance.layer = MiniMapLayer;
            }
            isPlanedCreated = true;
        }

        void DelayPositionInvoke()
        {
            defaultYCameraPosition = MMCamera.transform.position.y;
            getDelayPositionCamera = true;
        }

        /// <summary>
        /// Avoid to UI world space collision with other objects in scene.
        /// </summary>
        public void ConfigureCamera3D()
        {
            Camera cam = (Camera.main != null) ? Camera.main : Camera.current;
            if (cam == null)
            {
                Debug.LogWarning("Not to have found a camera to configure,please assign this.");
                return;
            }
            m_Canvas.worldCamera = cam;
            //Avoid to 3D UI transferred other objects in the scene.
            cam.nearClipPlane = 0.015f;
            m_Canvas.planeDistance = 0.1f;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ConfigureWorlTarget()
        {
            if (m_Target == null)
                return;

            bl_MiniMapItem mmi = m_Target.AddComponent<bl_MiniMapItem>();
            mmi.GraphicPrefab = ItemPrefab;
            mmi.Icon = PlayerIcon.sprite;
            mmi.IconColor = PlayerIcon.color;
            mmi.Target = m_Target.transform;
            mmi.Size = PlayerIcon.rectTransform.sizeDelta.x + 2;
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (m_Target == null && ClientAPI.GetPlayerObject()!=null)
                m_Target = ClientAPI.GetPlayerObject().GameObject;

            if (hasError)
                return;
            if (m_Target == null || MMCamera == null)
                return;

            //Controlled inputs key for minimap
            Inputs();
            //controlled that minimap follow the target
            PositionControll();
            //Apply rotation settings
            RotationControll();
            //for minimap and world map control
            MapSize();
        }

        /// <summary>
        /// Minimap follow the target.
        /// </summary>
        void PositionControll()
        {
            if (m_MapType == MapType.Local)
            {
                Vector3 p = m_Transform.position;
                // Update the transformation of the camera as per the target's position.
                p.x = Target.position.x;
                if (!Ortographic2D)
                {
                    p.z = Target.position.z;
                }
                else
                {
                    p.y = Target.position.y;
                }
                p += DragOffset;

                //Calculate player position
                if (Target != null)
                {
                    Vector3 pp = MMCamera.WorldToViewportPoint(TargetPosition);
                    PlayerIcon.rectTransform.anchoredPosition = bl_MiniMapUtils.CalculateMiniMapPosition(pp, MapUIRoot);
                }

                // For this, we add the predefined (but variable, see below) height var.
                if (!Ortographic2D)
                {
                    p.y = (MaxZoom + MinZoom * 0.5f) + (Target.position.y * 2);
                }
                else
                {
                    p.z = ((Target.position.z) * 2) - (MaxZoom + MinZoom * 0.5f);
                }
                //Camera follow the target
                m_Transform.position = Vector3.Lerp(m_Transform.position, p, Time.deltaTime * 10);
            }

            if (plane != null && getDelayPositionCamera)
            {
                Vector3 v = plane.transform.position;
                //Get Position reference from world space rect.
                Vector3 pos = WorldSpace.position;
                float ydif = defaultYCameraPosition - MMCamera.transform.position.y;
                v.y = pos.y - ydif;
                plane.transform.position = v;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void RotationControll()
        {
            // If the minimap should rotate as the target does, the rotateWithTarget var should be true.
            // An extra catch because rotation with the full screen map is a bit weird.
            RectTransform rt = PlayerIcon.GetComponent<RectTransform>();
            RectTransform crt = PlayerCamera.GetComponent<RectTransform>();

            if (DynamicRotation && m_MapType != MapType.Global)
            {
                //get local reference.
                Vector3 e = m_Transform.eulerAngles;
                e.y = Target.eulerAngles.y;
                if (SmoothRotation)
                {

                    Vector3 ctr = Camera.main.transform.localEulerAngles;
                    Vector3 cr = Vector3.zero;
                    cr.z = -ctr.y + e.y;
                    crt.localEulerAngles = cr;


                    if (m_Mode == RenderMode.Mode2D)
                    {
                        //For 2D Mode
                        rt.eulerAngles = Vector3.zero;
                    }
                    else
                    {
                        //For 3D Mode
                        rt.localEulerAngles = Vector3.zero;
                    }

                    if (m_Transform.eulerAngles.y != e.y)
                    {
                        //calculate the difference 
                        float d = e.y - m_Transform.eulerAngles.y;
                        //avoid lerp from 360 to 0 or reverse
                        if (d > 180 || d < -180)
                        {
                            m_Transform.eulerAngles = e;
                        }
                    }
                    //Lerp rotation of map
                    m_Transform.eulerAngles = Vector3.Lerp(this.transform.eulerAngles, e, Time.deltaTime * LerpRotation);
                }
                else
                {
                    m_Transform.eulerAngles = e;
                }
            }
            else
            {
                m_Transform.eulerAngles = DeafultMapRot;
                if (m_Mode == RenderMode.Mode2D)
                {
                    //When map rotation is static, only rotate the player icon
                    Vector3 e = Vector3.zero;
                    //get and fix the correct angle rotation of target
                    e.z = -Target.eulerAngles.y;
                    rt.eulerAngles = e;
                }
                else
                {
                    //Use local rotation in 3D mode.
                    Vector3 tr = Target.localEulerAngles;
                    Vector3 r = Vector3.zero;
                    r.z = -tr.y;
                    rt.localEulerAngles = r;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void Inputs()
        {
            // If the minimap button is pressed then toggle the map state.
            //  if (Input.GetKeyDown(ToogleKey))

            if (!ClientAPI.UIHasFocus() && Input.GetKeyDown(AtavismSettings.Instance.GetKeySettings().map))
            {
                ToggleSize();
            }
            if (Input.GetKeyDown(DecreaseHeightKey) && DefaultHeight < MaxZoom)
            {
                ChangeHeight(true);
            }
            if (Input.GetKeyDown(IncreaseHeightKey) && DefaultHeight > MinZoom)
            {
                ChangeHeight(false);
            }
        }

        /// <summary>
        /// Map FullScreen or MiniMap
        /// Lerp all transition for smooth effect.
        /// </summary>
        void MapSize()
        {
            RectTransform rt = MMUIRoot;
            float delta = Time.deltaTime;
            if (isFullScreen)
            {
                m_Canvas.transform.SetSiblingIndex(orgSlibingIndex + 1);
                if (DynamicRotation)
                {
                    DynamicRotation = false;
                    ResetMapRotation();
                }
                rt.sizeDelta = Vector2.Lerp(rt.sizeDelta, FullMapSize, delta * LerpTransition);
                rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, FullMapPosition, delta * LerpTransition);
                rt.localEulerAngles = Vector3.Lerp(rt.localEulerAngles, FullMapRotation, delta * LerpTransition);
            }
            else
            {
                m_Canvas.transform.SetSiblingIndex(orgSlibingIndex);
                if (DynamicRotation != DefaultRotationMode)
                {
                    DynamicRotation = DefaultRotationMode;
                }
                rt.sizeDelta = Vector2.Lerp(rt.sizeDelta, MiniMapSize, delta * LerpTransition);
                rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, MiniMapPosition, delta * LerpTransition);
                rt.localEulerAngles = Vector3.Lerp(rt.localEulerAngles, MiniMapRotation, delta * LerpTransition);
            }
            MMCamera.orthographicSize = Mathf.Lerp(MMCamera.orthographicSize, DefaultHeight, delta * LerpHeight);
        }
        
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "UPDATE_LANGUAGE")
            {
#if AT_I2LOC_PRESET
                if (m_Type == RenderType.Picture)
                    {
                    if (MapTextures.Count > 0)
                        {
                            foreach (MapLang ml in MapTextures)
                            {
                                if (ml.name.Equals(I2.Loc.LocalizationManager.CurrentLanguage))
                                {
                                    SetMapTexture(ml.mapTexture);
                                    return;
                                }
                            }
                        }
                    }
#endif
                SetMapTexture(MapTexture);
            }
        }

        /// <summary>
        /// This called one time when press the toggle key
        /// </summary>
        void ToggleSize()
        {
            isFullScreen = !isFullScreen;
            if (RootAlpha != null && FadeOnFullScreen)
            {
                StopCoroutine("StartFade");
                StartCoroutine("StartFade", 0.35f);
            }
            if (isFullScreen)
            {
                if (m_MapType != MapType.Global)
                {
                    //when change to full screen, the height is the max
                    DefaultHeight = MaxZoom;
                }
                useCompassRotation = false;
                if (m_maskHelper)
                {
                    m_maskHelper.OnChange(true);
                }
            }
            else
            {
                if (m_MapType != MapType.Global)
                {
                    //when return of full screen, return to current height
                    DefaultHeight = PlayerPrefs.GetFloat(MMHeightKey, DefaultHeight);
                }
                if (useCompassRotation != DefaultRotationCircle)
                {
                    useCompassRotation = DefaultRotationCircle;
                }
                if (m_maskHelper)
                {
                    m_maskHelper.OnChange();
                }
            }
            //reset offset position 
            if (ResetOffSetOnChange)
            {
                GoToTarget();
            }
            int state = (isFullScreen) ? 1 : 2;
            if (BottonAnimator != null && ShowLevelName)
            {
                if (!BottonAnimator.gameObject.activeSelf)
                {
                    BottonAnimator.gameObject.SetActive(true);
                }
                if (BottonAnimator.transform.GetComponentInChildren<Text>() != null)
                {
                    BottonAnimator.transform.GetComponentInChildren<Text>().text = LevelName;
                }
                BottonAnimator.SetInteger("state", state);
            }
            else if (BottonAnimator != null)
            {
                BottonAnimator.gameObject.SetActive(false);
            }
            if (PanelInfoAnimator != null && ShowPanelInfo)
            {
                if (!PanelInfoAnimator.gameObject.activeSelf)
                {
                    PanelInfoAnimator.gameObject.SetActive(true);
                }
                PanelInfoAnimator.SetInteger("show", state);
            }
            else if (PanelInfoAnimator != null)
            {
                PanelInfoAnimator.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        public void SetDragPosition(Vector3 pos)
        {
            if (DragOnlyOnFullScreen)
            {
                if (!isFullScreen)
                    return;
            }

            DragOffset.x += ((-pos.x) * DragMovementSpeed.x);
            DragOffset.z += ((-pos.y) * DragMovementSpeed.y);

            DragOffset.x = Mathf.Clamp(DragOffset.x, -MaxOffSetPosition.x, MaxOffSetPosition.x);
            DragOffset.z = Mathf.Clamp(DragOffset.z, -MaxOffSetPosition.y, MaxOffSetPosition.y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Position">world map position</param>
        public void SetPointMark(Vector3 Position)
        {
            if (!AllowMapMarks)
                return;
            if (!AllowMultipleMarks)
            {
                Destroy(mapPointer);
            }
            mapPointer = Instantiate(MapPointerPrefab, Position, Quaternion.identity) as GameObject;
            mapPointer.GetComponent<bl_MapPointer>().SetColor(playerColor);
        }

        /// <summary>
        /// 
        /// </summary>
        public void GoToTarget()
        {
            StopCoroutine("ResetOffset");
            StartCoroutine("ResetOffset");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator ResetOffset()
        {
            while (Vector3.Distance(DragOffset, Vector3.zero) > 0.2f)
            {
                DragOffset = Vector3.Lerp(DragOffset, Vector3.zero, Time.deltaTime * LerpTransition);
                yield return null;
            }
            DragOffset = Vector3.zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void ChangeHeight(bool b)
        {
            if (m_MapType == MapType.Global)
                return;

            if (b)
            {
                if (DefaultHeight + scrollSensitivity <= MaxZoom)
                {
                    DefaultHeight += scrollSensitivity;
                }
                else
                {
                    DefaultHeight = MaxZoom;
                }
            }
            else
            {
                if (DefaultHeight - scrollSensitivity >= MinZoom)
                {
                    DefaultHeight -= scrollSensitivity;
                }
                else
                {
                    DefaultHeight = MinZoom;
                }
            }
            PlayerPrefs.SetFloat(MMHeightKey, DefaultHeight);
        }

        /// <summary>
        /// Call this when player / target receive damage
        /// for play a 'Hit effect' in minimap.
        /// </summary>
        public void DoHitEffect()
        {
            if (HitEffectAnimator == null)
            {
                Debug.LogWarning("Please assign Hit animator for play effect!");
                return;
            }
            HitEffectAnimator.speed = HitEffectSpeed;
            HitEffectAnimator.Play("HitEffect", 0, 0);
        }

        /// <summary>
        /// Create Material for Minimap image in plane.
        /// you can edit and add your own shader.
        /// </summary>
        /// <returns></returns>
        public Material CreateMaterial()
        {
            Material mat = new Material(ReferenceMat);
            mat.mainTexture = MapTexture;
            mat.SetTexture("_EmissionMap", MapTexture);
#if AT_I2LOC_PRESET
            if (m_Type == RenderType.Picture)
            {
                if (MapTextures.Count > 0)
                {
                    foreach (MapLang ml in MapTextures)
                    {
                        if (ml.name.Equals(I2.Loc.LocalizationManager.CurrentLanguage))
                        {
                            mat.mainTexture = ml.mapTexture;
                            mat.SetTexture("_EmissionMap", ml.mapTexture);
                         }
                    }
                }
            }
#endif
            mat.SetFloat("_EmissionScaleUI", EmissionAmount);
            mat.SetColor("_EmissionColor", EmessiveColor);
            mat.SetColor("_SpecColor", SpecularColor);
            mat.EnableKeyword("_EMISSION");

            return mat;
        }

        /// <summary>
        /// Create a new icon without reference in runtime.
        /// see all structure options in bl_MMItemInfo.
        /// </summary>
        public bl_MiniMapItem CreateNewItem(bl_MMItemInfo item)
        {
            if (hasError)
                return null;

            GameObject newItem = Instantiate(ItemPrefabSimple, item.Position, Quaternion.identity) as GameObject;
            bl_MiniMapItem mmItem = newItem.GetComponent<bl_MiniMapItem>();
            if (item.Target != null)
            {
                mmItem.Target = item.Target;
            }
            mmItem.Size = item.Size;
            mmItem.IconColor = item.Color;
            mmItem.isInteractable = item.Interactable;
            mmItem.m_Effect = item.Effect;
            if (item.Sprite != null)
            {
                mmItem.Icon = item.Sprite;
            }

            return mmItem;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetHoofdPunt()
        {
            //Verify is MiniMap Layer Exist in Layer Mask List.
            string layer = LayerMask.LayerToName(MiniMapLayer);
            //If not exist.
            if (string.IsNullOrEmpty(layer))
            {
                Debug.LogError("MiniMap Layer is null, please assign it in the inspector.");
                MMUIRoot.gameObject.SetActive(false);
                hasError = true;
                enabled = false;
                return;
            }
            if (HoofdPuntPrefab == null || m_MapType == MapType.Global)
                return;

            GameObject newItem = Instantiate(HoofdPuntPrefab, new Vector3(0, 0, 100), Quaternion.identity) as GameObject;
            bl_MiniMapItem mmItem = newItem.GetComponent<bl_MiniMapItem>();
            mmItem.Target = newItem.transform;
        }

        /// <summary>
        /// Reset this transform rotation helper.
        /// </summary>
        void ResetMapRotation()
        {
            m_Transform.eulerAngles = new Vector3(90, 0, 0);
        }
        /// <summary>
        /// Call this fro change the mode of rotation of map
        /// Static or dynamic
        /// </summary>
        /// <param name="mode">static or dynamic</param>
        /// <returns></returns>
        public void RotationMap(bool mode)
        {
            if (isFullScreen)
                return;
            DynamicRotation = mode;
            DefaultRotationMode = DynamicRotation;
        }
        /// <summary>
        /// Change the size of Map full screen or mini
        /// </summary>
        /// <param name="fullscreen">is full screen?</param>
        public void ChangeMapSize(bool fullscreen)
        {
            isFullScreen = fullscreen;
        }

        /// <summary>
        /// Set target in runtime
        /// </summary>
        /// <param name="t"></param>
        public void SetTarget(GameObject t)
        {
            m_Target = t;
        }

        /// <summary>
        /// Set Map Texture in Runtime
        /// </summary>
        /// <param name="t"></param>
        public void SetMapTexture(Texture t)
        {
           if (m_Type != RenderType.Picture)
            {
                Debug.LogWarning("You only can set texture in Picture Mode");
                return;
            }
            plane.GetComponent<MeshRenderer>().material.mainTexture = t;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (MMCamera != null)
            {
                MMCamera.orthographicSize = DefaultHeight;
            }
            if (AreaMaterial != null)
            {
                Vector2 r = AreaMaterial.GetTextureScale("_MainTex");
                r.x = AreasSize;
                r.y = AreasSize;
                AreaMaterial.SetTextureScale("_MainTex", r);
            }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        public void SetGridSize(float value)
        {
            if (AreaMaterial != null)
            {
                Vector2 r = AreaMaterial.GetTextureScale("_MainTex");
                r.x = value;
                r.y = value;
                AreaMaterial.SetTextureScale("_MainTex", r);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetActiveGrid(bool active)
        {
            if (m_Type == RenderType.Picture && plane != null)
            {
                plane.transform.GetChild(0).gameObject.SetActive(active);
            }
            else if (areaInstance != null)
            {
                areaInstance.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetMapRotation(bool dynamic)
        {
            DynamicRotation = dynamic;
            DefaultRotationMode = dynamic;
            m_Transform.eulerAngles = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        void GetMiniMapSize()
        {
            MiniMapSize = MMUIRoot.sizeDelta;
            MiniMapPosition = MMUIRoot.anchoredPosition;
            MiniMapRotation = MMUIRoot.eulerAngles;
        }

        [ContextMenu("GetFullMapRect")]
        public void GetFullMapSize()
        {
            FullMapSize = MMUIRoot.sizeDelta;
            FullMapPosition = MMUIRoot.anchoredPosition;
            FullMapRotation = MMUIRoot.eulerAngles;
        }

        IEnumerator StartFade(float delay)
        {
            RootAlpha.alpha = 0;
            yield return new WaitForSeconds(delay);
            while (RootAlpha.alpha < 1)
            {
                RootAlpha.alpha += Time.deltaTime;
                yield return null;
            }
            isAlphaComplete = true;
        }

        public Transform Target
        {
            get
            {
                if (m_Target != null)
                {
                    return m_Target.GetComponent<Transform>();
                }
                return this.GetComponent<Transform>();
            }
        }
        public Vector3 TargetPosition
        {
            get
            {
                Vector3 v = Vector3.zero;
                if (m_Target != null)
                {
                    v = m_Target.transform.position;
                }
                return v;
            }
        }


        //Get Transform
        private Transform t;
        private Transform m_Transform
        {
            get
            {
                if (t == null)
                {
                    t = this.GetComponent<Transform>();
                }
                return t;
            }
        }
        //Get Mask Helper (if exist one)for management of texture mask
        private bl_MaskHelper _maskHelper = null;
        private bl_MaskHelper m_maskHelper
        {
            get
            {
                if (_maskHelper == null)
                {
                    _maskHelper = this.transform.root.GetComponentInChildren<bl_MaskHelper>();
                }
                return _maskHelper;
            }
        }

        [System.Serializable]
        public enum RenderType
        {
            RealTime,
            Picture,
        }

        [System.Serializable]
        public enum RenderMode
        {
            Mode2D,
            Mode3D,
        }

        [System.Serializable]
        public enum MapType
        {
            Local,
            Global,
        }
    }
}