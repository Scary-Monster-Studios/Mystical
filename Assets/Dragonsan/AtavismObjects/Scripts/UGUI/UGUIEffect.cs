using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public Text timeText;
        public TextMeshProUGUI TMPTimeText;
        public Text stackText;
        public TextMeshProUGUI TMPStackText;
        public Image imageEffect;
        public Image cooldown;

        AtavismEffect effect;
        int effectPos;
        //  float effectDuration = 0f;
        bool textCount = true;
        Coroutine cor = null;
        //   [SerializeField] bool corRuning = false;
        //  float expiration = 0f;

        private void OnDestroy()
        {
            if (cor != null)
                StopCoroutine(cor);
            //   corRuning = false;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            cor = null;
            //     corRuning = false;
        }

        void RunTimerUpdate()
        {
            // Check if this is on cooldown
            if (timeText != null)
                timeText.text = "";
            if (TMPTimeText != null)
                TMPTimeText.text = "";
            if (effect == null)
            {
                if (cooldown != null)
                    cooldown.fillAmount = 1f;
                return;
            }
            if (effect.Passive)
            {
                if (cooldown != null)
                    cooldown.fillAmount = 0f;
                return;
            }
            // effectDuration = effect.Expiration - Time.time;
            // if (!corRuning )
            //  {
            if (cor != null)
                StopCoroutine(cor);
            cor = StartCoroutine(UpdateTimer());
            // }


        }

        IEnumerator UpdateTimer()
        {
            //  corRuning = true;
            while (effect != null && effect.Expiration > Time.time)
            {
                float timeLeft = effect.Expiration - Time.time;
                if (timeLeft > 60)
                {
                    int minutes = (int)timeLeft / 60;
                    if (textCount)
                    {
                        if (timeText != null)
                            timeText.text = "" + (int)minutes + "m";
                        if (TMPTimeText != null)
                            TMPTimeText.text = "" + (int)minutes + "m";
                    }
                }
                else
                {
                    if (textCount)
                    {
                        if (timeText != null)
                            timeText.text = "" + (int)timeLeft + "s";
                        if (TMPTimeText != null)
                            TMPTimeText.text = "" + (int)timeLeft + "s";
                    }
                }
                if (!textCount)
                {
                    if (timeText != null)
                        timeText.text = "";
                    if (TMPTimeText != null)
                        TMPTimeText.text = "";
                }
                if (cooldown != null)
                    cooldown.fillAmount = timeLeft / effect.Length;
                yield return new WaitForSeconds(0.04f);
            }
            //    corRuning = false;
            if (timeText != null)
                timeText.text = "";
            if (TMPTimeText != null)
                TMPTimeText.text = "";
            if (cooldown != null)
                cooldown.fillAmount = 1f;
        }




        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
        }

        public void SetEffect(AtavismEffect effect, int pos, bool textCount)
        {
            /*   if(this.effect!=null)
                   if (this.effect.Equals(effect))
                       return;*/
            float timeLeft = effect.Expiration - Time.time;
         //   Debug.LogError("UGUIEffect: "+effect+" textCount="+textCount+ " timeLeft="+ timeLeft);
            this.textCount = textCount;
            this.effect = effect;
            this.effectPos = pos;
            if (effect.StackSize > 1)
            {
                if (stackText != null)
                    stackText.text = effect.StackSize.ToString();
                if (TMPStackText != null)
                    TMPStackText.text = effect.StackSize.ToString();
            }
            else
            {
                if (stackText != null)
                    stackText.text = "";
                if (TMPStackText != null)
                    TMPStackText.text = "";

            }

            //    expiration = effect.Expiration;
            if (imageEffect != null)
                imageEffect.sprite = effect.icon;
            RunTimerUpdate();
        }

        public void SetEffect(AtavismEffect effect, int pos)
        {
            SetEffect(effect, pos, true);
        }


        public void RemoveEffect()
        {
            Abilities.Instance.RemoveBuff(effect, effectPos);
        }

        void ShowTooltip()
        {
#if AT_I2LOC_PRESET
        UGUITooltip.Instance.SetTitle(I2.Loc.LocalizationManager.GetTranslation("Effects/" + effect.name));
        UGUITooltip.Instance.SetDescription(I2.Loc.LocalizationManager.GetTranslation("Effects/" + effect.tooltip));
#else
            UGUITooltip.Instance.SetTitle(effect.name);
            UGUITooltip.Instance.SetDescription(effect.tooltip);
#endif
            //   UGUITooltip.Instance.SetQuality(1);
            UGUITooltip.Instance.SetQualityColor(AtavismSettings.Instance.effectQualityColor);

            UGUITooltip.Instance.SetType("");
            UGUITooltip.Instance.SetWeight("");
            UGUITooltip.Instance.SetIcon(effect.icon);
            UGUITooltip.Instance.Show(gameObject);
        }

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
        }

        public bool MouseEntered
        {
            set
            {
                if (value && effect != null)
                {
                    ShowTooltip();
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}