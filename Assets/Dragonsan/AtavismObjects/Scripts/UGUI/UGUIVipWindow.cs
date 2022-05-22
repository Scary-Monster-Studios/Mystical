using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atavism
{

    public class UGUIVipWindow : MonoBehaviour
    {

        public UGUIPanelTitleBar titleBar;
        public TextMeshProUGUI VipName;
        public TextMeshProUGUI VipTime;
        public Color defaultTimeColor = Color.white;
        public Color expiredTimeColor = Color.red;

        public TextMeshProUGUI VipStatus;
        public Slider VipProgress;
        public TextMeshProUGUI TitleVipA;
        public TextMeshProUGUI TitleVipB;
        public Color currentLevelTitleColor = Color.green;
        public Color defaultTitleColor = Color.white;
        public UGUIVipSlot prefab;
        public Transform grid;
        List<UGUIVipSlot> list = new List<UGUIVipSlot>();
        int levDisplay = 1;
        bool showing = false;
        CanvasGroup _canvasGroup;
        // Start is called before the first frame update
        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            AtavismEventSystem.RegisterEvent("VIPS_UPDATE", this);
            AtavismEventSystem.RegisterEvent("VIP_UPDATE", this);
            AtavismVip.Instance.GetAllVips();
            UpdateDisplay();
            Hide();
            if (VipTime)
            {
                defaultTimeColor = VipTime.color;
            }
        }
        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("VIPS_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("VIP_UPDATE", this);
        }
        // Update is called once per frame
        void Update()
        {
            if(VipTime)
            if (AtavismVip.Instance.GetTime != 0)
            {

                float timeLeft = AtavismVip.Instance.GetTimeElapsed - Time.time;
                int days = 0;
                int hours = 0;
                int minutes = 0;
                if (timeLeft > 86400)
                {
                    days = (int)timeLeft / 86400;
                    timeLeft -= days * 86400;
                }
                if (timeLeft > 3600)
                {
                    hours = (int)timeLeft / 3600;
                    timeLeft -= hours * 3600;
                }
                if (timeLeft > 60)
                {
                    minutes = (int)timeLeft / 60;
                    timeLeft = minutes * 60;
                }
                if (days > 0)
                {
#if AT_I2LOC_PRESET
                       VipTime.text =I2.Loc.LocalizationManager.GetTranslation("Expires in :") + days + " "+I2.Loc.LocalizationManager.GetTranslation("days");
#else
                        VipTime.text ="Expires in :" + days + " days";
#endif
                        VipTime.color = defaultTimeColor;
                    }
                else if (hours > 0)
                {
#if AT_I2LOC_PRESET
                      VipTime.text =I2.Loc.LocalizationManager.GetTranslation("Expires in :") + hours + " "+I2.Loc.LocalizationManager.GetTranslation("hours");
#else
                        VipTime.text = "Expires in :" + hours + " hour";
#endif
                        VipTime.color = defaultTimeColor;
                    }
                else if (minutes > 0 && timeLeft > 0)
                {
#if AT_I2LOC_PRESET
                       VipTime.text =I2.Loc.LocalizationManager.GetTranslation("Expires in :") + minutes + " "+I2.Loc.LocalizationManager.GetTranslation("minutes");
#else
                        VipTime.text = "Expires in :" + minutes + " minutes";
#endif
                        VipTime.color = defaultTimeColor;
                    }
                else
                {
#if AT_I2LOC_PRESET
                       VipTime.text =I2.Loc.LocalizationManager.GetTranslation("Expired");
#else
                        VipTime.text = "Expired";
#endif
                        VipTime.color = expiredTimeColor;

                }
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "VIPS_UPDATE")
            {
                UpdateDisplay();
            }

            if (eData.eventType == "VIP_UPDATE")
            {
                UpdateDisplay();
            }

        }

        void UpdateDisplay()
        {
            if (VipName)
            {
#if AT_I2LOC_PRESET
                VipName.text = I2.Loc.LocalizationManager.GetTranslation(AtavismVip.Instance.GetName);
#else
                VipName.text = AtavismVip.Instance.GetName;
#endif
            }
            if (VipProgress)
            {
                if (AtavismVip.Instance.GetMaxPoints != 0)
                {
                    VipProgress.minValue = 0;
                    VipProgress.maxValue = AtavismVip.Instance.GetMaxPoints;
                    VipProgress.value = AtavismVip.Instance.GetPoints;
                    if (VipStatus)
                    {
                        VipStatus.text = AtavismVip.Instance.GetPoints+" / "+ AtavismVip.Instance.GetMaxPoints;
                        if (!VipStatus.enabled)
                            VipStatus.enabled = true;
                    }
                }
                else
                {
                    VipProgress.enabled = false;
                    if (VipStatus && VipStatus.enabled)
                        VipStatus.enabled = false;
                }
            }


            //  Debug.LogError("VipFull: Start");
            if (levDisplay >= AtavismVip.Instance.GetVips.Count)
                levDisplay = 1;
            int c = 1;
            foreach (string s in AtavismVip.Instance.GetBonuseNames)
            {
               // Debug.LogError("VipFull: c="+c);
                if (c >= list.Count)
                {
                //    Debug.LogError("VipFull: Instantiate c=" + c);
                    UGUIVipSlot go = Instantiate(prefab, grid);
                    list.Add(go);
                }
                string vipA = "";
                string vipB = "";

                if (AtavismVip.Instance.GetVips.ContainsKey(levDisplay))
                {
                    if (TitleVipA)
                    {
#if AT_I2LOC_PRESET
              TitleVipA.text= I2.Loc.LocalizationManager.GetTranslation( AtavismVip.Instance.GetVips[levDisplay].name);
#else
                        TitleVipA.text = AtavismVip.Instance.GetVips[levDisplay].name;
#endif
                        if (AtavismVip.Instance.GetLevel.Equals(AtavismVip.Instance.GetVips[levDisplay].level))
                            TitleVipA.color = currentLevelTitleColor;
                        else
                            TitleVipA.color = defaultTitleColor;

                    }
                    if (AtavismVip.Instance.GetVips[levDisplay].bonuses.ContainsKey(s))
                    {
                        if (AtavismVip.Instance.GetVips[levDisplay].bonuses[s].value != 0)
                        {
                            vipA = AtavismVip.Instance.GetVips[levDisplay].bonuses[s].value.ToString();
                        }
                        if (AtavismVip.Instance.GetVips[levDisplay].bonuses[s].percentage != 0)
                        {
                            if (AtavismVip.Instance.GetVips[levDisplay].bonuses[s].value != 0)
                            {
                                if (AtavismVip.Instance.GetVips[levDisplay].bonuses[s].percentage > 0)
                                    vipA += "\n&\n+" + AtavismVip.Instance.GetVips[levDisplay].bonuses[s].percentage +  " %";
                                else
                                    vipA += "\n&\n" + AtavismVip.Instance.GetVips[levDisplay].bonuses[s].percentage + " %";
                            }
                            else
                            {
                                if (AtavismVip.Instance.GetVips[levDisplay].bonuses[s].percentage > 0)
                                    vipA = "+" + AtavismVip.Instance.GetVips[levDisplay].bonuses[s].percentage + " %";
                                else
                                    vipA = AtavismVip.Instance.GetVips[levDisplay].bonuses[s].percentage + " %";
                            }
                        }
                    }
                    else
                    {
                        vipA = "-";
                    }

                }
                else
                {
                 //   Debug.LogError("VipFull: Instantiate c=" + c+ " levDisplay="+ levDisplay+" level not on list");
                }

                if (AtavismVip.Instance.GetVips.ContainsKey(levDisplay+1))
                {
                    if (TitleVipB)
                    {
                        TitleVipB.text = AtavismVip.Instance.GetVips[levDisplay+1].name;
                        if (AtavismVip.Instance.GetLevel.Equals(AtavismVip.Instance.GetVips[levDisplay+1].level))
                            TitleVipB.color = currentLevelTitleColor;
                        else
                            TitleVipB.color = defaultTitleColor;
                    }
                    if (AtavismVip.Instance.GetVips[levDisplay+1].bonuses.ContainsKey(s))
                    {
                        if (AtavismVip.Instance.GetVips[levDisplay+1].bonuses[s].value != 0)
                        {
                            vipB = AtavismVip.Instance.GetVips[levDisplay+1].bonuses[s].value.ToString();
                        }
                        if (AtavismVip.Instance.GetVips[levDisplay+1].bonuses[s].percentage != 0)
                        {
                            if (AtavismVip.Instance.GetVips[levDisplay + 1].bonuses[s].value != 0)
                            {
                                if (AtavismVip.Instance.GetVips[levDisplay + 1].bonuses[s].percentage > 0)
                                    vipB += "\n&\n+" + AtavismVip.Instance.GetVips[levDisplay + 1].bonuses[s].percentage + " %";
                                else
                                    vipB += "\n&\n" + AtavismVip.Instance.GetVips[levDisplay + 1].bonuses[s].percentage + " %";
                            }
                            else
                            {
                                if (AtavismVip.Instance.GetVips[levDisplay + 1].bonuses[s].percentage > 0)
                                    vipB = "+" + AtavismVip.Instance.GetVips[levDisplay + 1].bonuses[s].percentage + " %";
                                else
                                    vipB = AtavismVip.Instance.GetVips[levDisplay + 1].bonuses[s].percentage + " %";
                            }
                        }
                    }
                    else
                    {
                        vipB = "-";
                    }
                }
                else
                {
                    //  Debug.LogError("VipFull: Instantiate c=" + c + " levDisplay=" + (levDisplay+1) + " level not on list");
                    //  Debug.LogError("VipFull: Instantiate c=" + c + " levDisplay=" + (levDisplay+1) + " level not on list");
                    //  Debug.LogError("VipFull: Instantiate c=" + c + " levDisplay=" + (levDisplay+1) + " level not on list");
                    //  Debug.LogError("VipFull: Instantiate c=" + c + " levDisplay=" + (levDisplay+1) + " level not on list");
                    // Debug.LogError("VipFull: Instantiate c=" + c + " levDisplay=" + (levDisplay+1) + " level not on list");
                    // Debug.LogError("VipFull: Instantiate c=" + c + " levDisplay=" + (levDisplay+1) + " level not on list");
                    // Debug.LogError("VipFull: Instantiate c=" + c + " levDisplay=" + (levDisplay+1) + " level not on list");
                    // Debug.LogError("VipFull: Instantiate c=" + c + " levDisplay=" + (levDisplay+1) + " level not on list");
                }
                string name = s;
#if AT_I2LOC_PRESET
             name= I2.Loc.LocalizationManager.GetTranslation(name);
#endif
                // Debug.LogError("VipFull: "+s+" "+vipA+" "+vipB);
                list[c-1].UpdateDetaile(name, vipA, vipB);
                list[c - 1].gameObject.SetActive(true);
                c++;
            }
           // Debug.LogError("VipFull: End");

        }
        public void Next()
        {
            levDisplay++;
            if (levDisplay >= AtavismVip.Instance.GetVips.Count - 1)
                levDisplay = AtavismVip.Instance.GetVips.Count - 1;
            UpdateDisplay();
        }
        public void Prev()
        {
            levDisplay--;
            if (levDisplay < 1)
                levDisplay = 1;
            UpdateDisplay();
        }
        public void Show()
        {
            AtavismUIUtility.BringToFront(gameObject);
            AtavismSocial.Instance.SendGetFriends();
            UpdateDisplay();
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
            }
            showing = true;
        }

        public void Hide()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
            }
            showing = false;
        }

        public void Toggle()
        {
            if (showing)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}