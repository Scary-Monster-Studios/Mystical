using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;
using System.Collections.Generic;

namespace Atavism
{
    [Serializable]
    public class DamageType
    {
        public string type ;
        public Color damageColor;
        public Color criticDamageColor;
        
    }
    
    public class UGUIFloatingMobPanel : MonoBehaviour
    {

        public UGUIFloatingMobPanelController FloatingPanelController;
        public int ThisPanelID;

        //public Text nameText;
        //public Text tagText; // Guild names will be shown here
        public Text nameText;
        public TextMeshProUGUI nameTextTMP;
        public Text tagText; // Guild names will be shown here
        public TextMeshProUGUI tagTextTMP; // Guild names will be shown here
        public Text levelText;
        //public Text combatText;  //TODO Create an object pool of "Text" so more than one combat event can be seen.
        public Text combatText;
        public TextMeshProUGUI combatTextTMP;
        public TMP_FontAsset CombatTextFont, MessageFont, XPFont, CombatHealFont, CombatHealCritFont, BuffGainedFont, DebuffGainedFont, SelfDamageFont, SelfDamageCritFont, EnemyDamageCritFont;
        public RectTransform chatPanel;
        public Text chatText;
        public TextMeshProUGUI chatTextTMP;
        public bool usingWorldSpaceCanvas = true;
        public bool faceCamera = true;
        public float renderDistance = 50f;
        public float chatDisplayTime = 3f;
        public Color friendlyNameColour = Color.green;
        public Color neutralNameColour = Color.yellow;
        public Color enemyNameColour = Color.red;
     //  public Color myDamageColour = Color.red;
       // public Color targetDamageColour = Color.white;
        public Color myMessageColour = Color.yellow;
        public Color targetMessageColour = Color.yellow;
        public Color XPTextColor = Color.white;
        public Color HealColor;
        public Color CriticalHealColor;
        public Color BuffGainedColor;
        public Color DebuffGainedColor;
       // public Color MagicalDamageColor;
        public Color DamageColor;
        public Color SelfDamageColor;
        public Color CriticalDamageColor;
      //  public Color MagicalCriticalDamageColor;
        public List<DamageType> damageTypeColor = new  List<DamageType>();
        public float CriticalSizeUpRate;
        public float CriticalSizeDownRate;

        //public Image NpcIcon;
        //public TextMeshPro textMeshPro;
       // public TextMeshProUGUI textMeshProUgui;
        float addNameHeight = 0f;
        AtavismObjectNode mobNode;
        float stopDisplay;
        float stopChatDisplay;

        float combatDisplayTime = 1.5f;
        public float NormalDamageDisplayTime, CriticalDmgDisplayTime;

        private float initialScaleFactor = 3f;
        public float initialAlphaFactor = 3f;
        public float combatTextSpeed = 50f;
        public float combatColorSpeed = 1f;
        public float combatTextScaleDownSpeed = 3f;

        //Set automatically
        Vector3 startPosition;
        Vector3 startScale;
        float currentScale = 1f;
        float currentAlpha = 1f;
        float currentOffset = 0;
        bool showName = true;

        public bool IsCritical, MaxCritSizedReached;
       public float ThisTextTime;

        // Use this for initialization
        void Awake()
        {
            if (combatText != null)
            {
                this.startPosition = combatText.rectTransform.localPosition;
                this.startScale = combatText.rectTransform.localScale;
            }
            if (combatTextTMP != null)
            {
                this.startPosition = combatTextTMP.rectTransform.localPosition;
                this.startScale = combatTextTMP.rectTransform.localScale;
            }

            this.currentScale = initialScaleFactor;
            this.currentAlpha = initialAlphaFactor;
        }

        void OnDestroy()
        {
            if (mobNode != null)
            {
                mobNode.RemovePropertyChangeHandler("level", LevelHandler);
                mobNode.RemovePropertyChangeHandler("reaction", TargetTypeHandler);
                mobNode.RemovePropertyChangeHandler("adminLevel", AdminLevelHandler);
                mobNode.RemovePropertyChangeHandler("nameDisplay", NameDisplayHandler);
                mobNode.RemovePropertyChangeHandler("guildName", GuildNameDisplayHandler);
                mobNode.RemovePropertyChangeHandler("level", LevelHandler);
                mobNode.RemovePropertyChangeHandler("questavailable", QuestAvailableHandler);
                mobNode.RemovePropertyChangeHandler("questinprogress", QuestInProgressHandler);
                mobNode.RemovePropertyChangeHandler("questconcludable", QuestConcludableHandler);
                mobNode.RemovePropertyChangeHandler("dialogue_available", DialogueAvailableHandler);
                mobNode.RemovePropertyChangeHandler("itemstosell", ItemsToSellHandler);
                mobNode.RemovePropertyChangeHandler("mount", HandleMount);
            }
        }

        // Update is called once per frame
        public void RunUpdate(Camera cam)
        {
            //GUI.color = new Color (1.0f, 1.0f, 1.0f, 1.0f - (cameraDistance - fadeDistance) / (hideDistance - fadeDistance));
            if (mobNode == null || (mobNode!=null && mobNode.MobController == null))
            {
                return;
            }

            Vector3 worldPos = new Vector3(mobNode.Position.x, mobNode.Position.y + mobNode.MobController.nameHeight + addNameHeight, mobNode.Position.z);
            if (!usingWorldSpaceCanvas)
            {
                //GetComponent<CanvasGroup>().alpha = 1f;
                Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
                transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
            }
            else
            {
                transform.position = worldPos;
                if (faceCamera)
                {
                    transform.rotation = cam.transform.rotation;
                }
                else
                {
                    Quaternion cameraRotation = cam.transform.rotation;
                    cameraRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
                    transform.rotation = cameraRotation;
                }

            }

            UpdateCombatText();

            /*  if (Time.time > stopChatDisplay)
              {
                  HideChatBubble();
              }*/
        }

        void ResetCombatText()
        {
            currentOffset = 0;
            currentAlpha = initialAlphaFactor;
            if (combatText != null)
                combatText.text = "";
            if (combatTextTMP != null)
                combatTextTMP.text = "";

            currentScale = initialScaleFactor;
            if (combatText != null)
            {
                combatText.rectTransform.localPosition = startPosition;
                combatText.rectTransform.localScale = startScale;

                combatText.rectTransform.localPosition = startPosition + new Vector3(0, currentOffset, 0);

                var color = combatText.color;
                color.a = currentAlpha;
                combatText.color = color;

                combatText.rectTransform.localScale = startScale * currentScale;
            }
            if (combatTextTMP != null)
            {
                combatTextTMP.rectTransform.localPosition = startPosition;
                combatTextTMP.rectTransform.localScale = startScale;

                combatTextTMP.rectTransform.localPosition = startPosition + new Vector3(0, currentOffset, 0);

                var color = combatTextTMP.color;
                color.a = currentAlpha;
                combatTextTMP.color = color;

                combatTextTMP.rectTransform.localScale = startScale * currentScale;
            }
            startPosition.x = 0;
            IsCritical = false;
            MaxCritSizedReached = false;
            ThisTextTime = 0;
        }

        void UpdateCombatText()
        {
            if (Time.time > stopDisplay)
            {
              //  Debug.LogError("UpdateCombatText stop",gameObject);

                FloatingPanelController.ALLcombatTextData[ThisPanelID].CurrentNode = null;
                FloatingPanelController.ALLcombatTextData[ThisPanelID].CurrentCliamObject=null;
                FloatingPanelController.ALLcombatTextData[ThisPanelID].RendererReference = null;
                FloatingPanelController.ALLcombatTextData[ThisPanelID].LastTimeUsed = 0;
                FloatingPanelController.ALLcombatTextData[ThisPanelID].FloatingPanelGO.SetActive(false);

                stopDisplay = 0;

                if (combatText != null)
                    combatText.text = "";
                if (combatTextTMP != null)
                    combatTextTMP.text = "";



                if (chatText != null)
                {
                    chatText.text = "";
                }
                if (chatTextTMP != null)
                {
                    chatTextTMP.text = "";
                }

                if (chatPanel != null)
                    chatPanel.gameObject.SetActive(false);

                return;
            }

            //Update the current position.
            if (!IsCritical)
            {
                currentOffset += Time.deltaTime * combatTextSpeed;
                if (combatText != null)
                {
                    combatText.rectTransform.localPosition = startPosition + new Vector3(0, currentOffset, 0);
                }

                if (combatTextTMP != null)
                {
                    combatTextTMP.rectTransform.localPosition = startPosition + new Vector3(0, currentOffset, 0);
                }


                //Update the current alpha channel.
                currentAlpha = Mathf.Max(0, currentAlpha - Time.deltaTime * combatColorSpeed);
                if (combatText != null)
                {
                    var color = combatText.color;
                    color.a = currentAlpha;
                    combatText.color = color;
                }
                if (combatTextTMP != null)
                {
                    var color = combatTextTMP.color;
                    color.a = currentAlpha;
                    combatTextTMP.color = color;
                }

                //Update the current scale.
                currentScale = Mathf.Max(1f, currentScale - Time.deltaTime * combatTextScaleDownSpeed);
                if (combatText != null)
                    combatText.rectTransform.localScale = startScale * currentScale;
                if (combatTextTMP != null)
                    combatTextTMP.rectTransform.localScale = startScale * currentScale;

            }

            else
            {

                //Update the current alpha channel.
                currentAlpha = Mathf.Max(0, currentAlpha - Time.deltaTime * combatColorSpeed);
                if (combatText != null)
                {
                    var color = combatText.color;
                    color.a = currentAlpha;
                    combatText.color = color;
                }
                if (combatTextTMP != null)
                {
                    var color = combatTextTMP.color;
                    color.a = currentAlpha;
                    combatTextTMP.color = color;
                }

                //Update the current scale.


                if (combatTextTMP.rectTransform.localScale.x <= 1.5f)
                {
                    if (!MaxCritSizedReached)
                    {
                        float newscale = combatTextTMP.rectTransform.localScale.x + CriticalSizeUpRate;

                        combatTextTMP.rectTransform.localScale = new Vector3(newscale, newscale, newscale);
                    }
                }
                else
                {
                    MaxCritSizedReached = true;
                    if (combatTextTMP.rectTransform.localScale.x > 1)
                    {
                        float newscale = combatTextTMP.rectTransform.localScale.x - CriticalSizeDownRate;

                        combatTextTMP.rectTransform.localScale = new Vector3(newscale, newscale, newscale);
                    }
                }



            }
        }

        public void SetMobDetails(AtavismObjectNode mobNode, bool showName)
        {
            this.mobNode = mobNode;
            this.showName = showName;
            if (showName && mobNode != null)
            {
                mobNode.RegisterPropertyChangeHandler("level", LevelHandler);
                mobNode.RegisterPropertyChangeHandler("reaction", TargetTypeHandler);
                mobNode.RegisterPropertyChangeHandler("adminLevel", AdminLevelHandler);
                mobNode.RegisterPropertyChangeHandler("nameDisplay", NameDisplayHandler);
                mobNode.RegisterPropertyChangeHandler("guildName", GuildNameDisplayHandler);
                mobNode.RegisterPropertyChangeHandler("questavailable", QuestAvailableHandler);
                mobNode.RegisterPropertyChangeHandler("questinprogress", QuestInProgressHandler);
                mobNode.RegisterPropertyChangeHandler("questconcludable", QuestConcludableHandler);
                mobNode.RegisterPropertyChangeHandler("dialogue_available", DialogueAvailableHandler);
                mobNode.RegisterPropertyChangeHandler("itemstosell", ItemsToSellHandler);
                mobNode.RegisterPropertyChangeHandler("mount", HandleMount);
            }
            UpdateNameDisplay(showName);

        }

        private void HandleMount(object sender, PropertyChangeEventArgs args)
        {
            if (ClientAPI.GetObjectNode(mobNode.Oid).Parent != null)
            {
                CharacterController col = ClientAPI.GetObjectNode(mobNode.Oid).Parent.GetComponent<CharacterController>();
                if (col != null)
                    addNameHeight = col.height * ClientAPI.GetObjectNode(mobNode.Oid).Parent.transform.localScale.y;
            }
            else
            {
                addNameHeight = 0f;
            }
        }

        void UpdateNameDisplay(bool showName)
        {
            if (mobNode != null)
                if (mobNode.PropertyExists("nameDisplay") && !mobNode.CheckBooleanProperty("nameDisplay"))
                {
                    showName = false;
                }
            if (mobNode != null)
                if (ClientAPI.GetObjectNode(mobNode.Oid) != null && ClientAPI.GetObjectNode(mobNode.Oid).Parent != null)
                {
                    CharacterController col = ClientAPI.GetObjectNode(mobNode.Oid).Parent.GetComponent<CharacterController>();
                    if (col != null)
                        addNameHeight = col.height * ClientAPI.GetObjectNode(mobNode.Oid).Parent.transform.localScale.y;
                }
                else
                {
                    addNameHeight = 0f;
                }
            if (mobNode!=null && showName)
            {
                if (levelText != null && mobNode.PropertyExists("level"))
                {
                    int mobLevel = (int)mobNode.GetProperty("level");
                    Color textColor = levelText.color;
                    if (ClientAPI.GetPlayerObject().PropertyExists("level"))
                    {
                        int playerLevel = (int)ClientAPI.GetPlayerObject().GetProperty("level");
                        if (mobLevel - playerLevel > 5)
                        {
                            textColor = Color.red;
                        }
                        else if (playerLevel - mobLevel > 5)
                        {
                            textColor = Color.green;
                        }
                    }

                    levelText.color = textColor;
                    if (nameText != null && showName)
                        nameText.text = mobNode.Name;
                    if (nameTextTMP != null && showName)
                        nameTextTMP.text = mobNode.Name;
#if AT_I2LOC_PRESET
                levelText.text = "[" + I2.Loc.LocalizationManager.GetTranslation("Level") + " " + mobLevel + "]";
#else
                    levelText.text = "[" + "Level" + " " + mobLevel + "]";
#endif
                }
                else
                {
                    if (nameText != null && showName)
                        nameText.text = mobNode.Name;
                    if (nameTextTMP != null && showName)
                        nameTextTMP.text = mobNode.Name;
                    if (levelText != null)
                        levelText.text = "";
                }
                // Show tag if the player is in a Guild
                if (tagText != null)
                {
                    if (mobNode.PropertyExists("guildName"))
                    {
                        string guildName = (string)mobNode.GetProperty("guildName");
                        if (guildName != null && guildName != "")
                        {
                            tagText.text = "<" + guildName + ">";
                        }
                        else
                        {
                            tagText.text = "";
                        }
                    }
                    else
                    {
                        tagText.text = "";
                    }
                }
                if (tagTextTMP != null)
                {
                    if (mobNode.PropertyExists("guildName"))
                    {
                        string guildName = (string)mobNode.GetProperty("guildName");
                        if (guildName != null && guildName != "")
                        {
                            tagTextTMP.text = "<" + guildName + ">";
                        }
                        else
                        {
                            tagTextTMP.text = "";
                        }
                    }
                    else
                    {
                        tagTextTMP.text = "";
                    }
                }

                // Set name colour based on target type
                if (nameText != null)
                    nameText.color = neutralNameColour;
                if (nameTextTMP != null)
                    nameTextTMP.color = neutralNameColour;
                if (tagText != null)
                {
                    tagText.color = neutralNameColour;
                }
                if (tagTextTMP != null)
                {
                    tagTextTMP.color = neutralNameColour;
                }
                if (mobNode.PropertyExists("reaction"))
                {
                    int targetType = (int)mobNode.GetProperty("reaction");
                    if (targetType < 0)
                    {
                        if (nameText != null)
                            nameText.color = enemyNameColour;
                        if (nameTextTMP != null)
                            nameTextTMP.color = enemyNameColour;
                        if (tagText != null)
                        {
                            tagText.color = enemyNameColour;
                        }
                        if (tagTextTMP != null)
                        {
                            tagTextTMP.color = neutralNameColour;
                        }
                    }
                    else if (targetType > 0)
                    {
                        if (nameText != null)
                            nameText.color = friendlyNameColour;
                        if (nameTextTMP != null)
                            nameTextTMP.color = friendlyNameColour;
                        if (tagText != null)
                        {
                            tagText.color = friendlyNameColour;
                        }
                        if (tagTextTMP != null)
                        {
                            tagTextTMP.color = neutralNameColour;
                        }
                    }
                }
            }
            else
            {
                if (nameText != null)
                    nameText.text = "";
                if (nameTextTMP != null)
                    nameTextTMP.text = "";
                if (levelText != null)
                {
                    levelText.text = "";
                }
                if (tagText != null)
                {
                    tagText.text = "";
                }
                if (tagTextTMP != null)
                {
                    tagTextTMP.text = "";
                }
            }

            // Show admin Icon?
            /*if (mobNode != null && mobNode.PropertyExists("adminLevel")) {
                int adminLevel = (int)mobNode.GetProperty("adminLevel");
                adminIcon.gameObject.SetActive(adminLevel == 5);
            } else {
                adminIcon.gameObject.SetActive(false);
            }*/
        }

        public void ShowCombatText(string message, string eventType, string damageType)
        {
         //   Debug.LogError("ShowCombatText message="+message+" eventType="+eventType+" damageType="+damageType, gameObject);
            if (eventType == "CombatBuffGained" || eventType == "CombatDebuffGained" || eventType == "CombatDebuffLost" || eventType == "CombatBuffLost" || eventType == "CombatAbilityLearned")
            {
                return;
            }

            ResetCombatText();
            if (combatText != null)
            {
                combatText.text = message;
            }

            if (combatTextTMP != null)
            {
                combatTextTMP.text = message;

            }


            ThisTextTime = Time.time;

            //stopDisplay = Time.time + combatDisplayTime;
            //     Debug.LogError(" ShowCombatText " + message + " | " + eventType);
            // Change colour based on eventType

            stopDisplay = Time.time + NormalDamageDisplayTime;




            if (eventType == "CombatDamage")
            {
             //   Debug.LogError("ShowCombatText CombatDamage");
                // print("in combat physical damage");

                if (mobNode is AtavismPlayer)
                {
                //    Debug.LogError("ShowCombatText CombatDamage AtavismPlayer");
                    if (mobNode.Oid == ClientAPI.GetPlayerOid())
                    {
                     //   Debug.LogError("ShowCombatText CombatDamage Player Self");

                        if (combatText != null)
                            combatText.color = SelfDamageColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = SelfDamageColor;
                            combatTextTMP.font = SelfDamageFont;
                            combatTextTMP.fontSize = 40;
                            combatTextTMP.enableVertexGradient = false;
                            combatTextTMP.text = "- " + message;
                            startPosition.x = 225;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(300, 0, 0);
                        }
                    }
                    else
                    {
                      //  Debug.LogError("ShowCombatText CombatDamage player not Self");

                        if (combatText != null)
                            combatText.color = getDamageTypeColor(damageType).damageColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = getDamageTypeColor(damageType).damageColor;
                            combatTextTMP.font = CombatTextFont;
                            combatTextTMP.fontSize = 20;
                            combatTextTMP.enableVertexGradient = false;

                            float RdmX = UnityEngine.Random.Range(-70, 70);
                            float RdmY = UnityEngine.Random.Range(-70, 70);

                            startPosition.x = RdmX;
                            startPosition.y = RdmY;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                }
                else
                {
                //    Debug.LogError("ShowCombatText CombatDamage no player "+CombatTextFont);

                    if (combatText != null)
                        combatText.color = getDamageTypeColor(damageType).damageColor;
                    if (combatTextTMP != null)
                    {
                        combatTextTMP.color = getDamageTypeColor(damageType).damageColor;
                        combatTextTMP.font = CombatTextFont;
                        combatTextTMP.fontSize = 20;

                        float RdmX = UnityEngine.Random.Range(-70, 70);
                        float RdmY = UnityEngine.Random.Range(-70, 70);

                        startPosition.x = RdmX;
                        startPosition.y = RdmY;
                        //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                        // combatTextTMP.enableVertexGradient = true;
                    }
                }
            }
            else if (eventType == "CombatDamageCritical")
            {
                IsCritical = true;
                stopDisplay = Time.time + CriticalDmgDisplayTime;

                if (mobNode is AtavismPlayer)
                {
                    if (mobNode.Oid == ClientAPI.GetPlayerOid())
                    {
                        if (combatText != null)
                            combatText.color = SelfDamageColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = SelfDamageColor;
                            combatTextTMP.font = SelfDamageCritFont;
                            combatTextTMP.fontSize = 65;
                            combatTextTMP.enableVertexGradient = false;
                            if(message.Equals("0"))
                                combatTextTMP.text = message;
                            else
                                combatTextTMP.text = "- " + message;
                            startPosition.x = 225;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(300, 0, 0);
                        }
                    }
                    else
                    {
                        if (combatText != null)
                            combatText.color = getDamageTypeColor(damageType).criticDamageColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color =  getDamageTypeColor(damageType).criticDamageColor;
                            combatTextTMP.font = CombatTextFont;
                            combatTextTMP.fontSize = 65;
                            combatTextTMP.enableVertexGradient = false;

                            float RdmX = UnityEngine.Random.Range(-70, 70);
                            float RdmY = UnityEngine.Random.Range(-70, 70);

                            startPosition.x = RdmX;
                            startPosition.y = RdmY;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                }
                else
                {
                    if (combatText != null)
                        combatText.color =  getDamageTypeColor(damageType).criticDamageColor;
                    if (combatTextTMP != null)
                    {
                        combatTextTMP.color =  getDamageTypeColor(damageType).criticDamageColor;
                        combatTextTMP.font = EnemyDamageCritFont;
                        combatTextTMP.fontSize = 65;

                        float RdmX = UnityEngine.Random.Range(-70, 70);
                        float RdmY = UnityEngine.Random.Range(-70, 70);

                        startPosition.x = RdmX;
                        startPosition.y = RdmY;
                        //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                        // combatTextTMP.enableVertexGradient = true;
                    }
                }
            }
         /*   else if (eventType == "CombatMagicalDamage")
            {

                // print("in combat magical damage");

                if (mobNode is AtavismPlayer)
                {
                    if (mobNode.Oid == ClientAPI.GetPlayerOid())
                    {
                        if (combatText != null)
                            combatText.color = SelfDamageColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = SelfDamageColor;
                            combatTextTMP.font = SelfDamageFont;
                            combatTextTMP.fontSize = 40;
                            combatTextTMP.enableVertexGradient = false;
                            if (message.Equals("0"))
                                combatTextTMP.text = message;
                            else
                                combatTextTMP.text = "- " + message;
                            startPosition.x = 225;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(300, 0, 0);
                        }
                    }
                    else
                    {
                        if (combatText != null)
                            combatText.color = MagicalDamageColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = MagicalDamageColor;
                            combatTextTMP.font = CombatTextFont;
                            combatTextTMP.fontSize = 20;
                            combatTextTMP.enableVertexGradient = false;

                            float RdmX = UnityEngine.Random.Range(-70, 70);
                            float RdmY = UnityEngine.Random.Range(-70, 70);

                            startPosition.x = RdmX;
                            startPosition.y = RdmY;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                }
                else
                {
                    if (combatText != null)
                        combatText.color = MagicalDamageColor;
                    if (combatTextTMP != null)
                    {
                        combatTextTMP.color = MagicalDamageColor;
                        combatTextTMP.font = CombatTextFont;
                        combatTextTMP.fontSize = 20;
                        combatTextTMP.enableVertexGradient = false;

                        float RdmX = UnityEngine.Random.Range(-70, 70);
                        float RdmY = UnityEngine.Random.Range(-70, 70);

                        startPosition.x = RdmX;
                        startPosition.y = RdmY;
                        //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                    }
                }

            }*/
         /*   else if (eventType == "CombatMagicalCritical")
            {

                IsCritical = true;
                stopDisplay = Time.time + CriticalDmgDisplayTime;

                if (mobNode is AtavismPlayer)
                {
                    if (mobNode.Oid == ClientAPI.GetPlayerOid())
                    {
                        if (combatText != null)
                            combatText.color = SelfDamageColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = SelfDamageColor;
                            combatTextTMP.font = SelfDamageFont;
                            combatTextTMP.fontSize = 65;
                            combatTextTMP.enableVertexGradient = false;
                            if (message.Equals("0"))
                                combatTextTMP.text = message;
                            else
                                combatTextTMP.text = "- " + message;

                            startPosition.x = 225;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(300, 0, 0);
                        }
                    }
                    else
                    {
                        if (combatText != null)
                            combatText.color = MagicalDamageColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = MagicalCriticalDamageColor;
                            combatTextTMP.font = CombatTextFont;
                            combatTextTMP.fontSize = 65;
                            combatTextTMP.enableVertexGradient = false;

                            float RdmX = UnityEngine.Random.Range(-70, 70);
                            float RdmY = UnityEngine.Random.Range(-70, 70);

                            startPosition.x = RdmX;
                            startPosition.y = RdmY;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                }
                else
                {
                    if (combatText != null)
                        combatText.color = MagicalDamageColor;
                    if (combatTextTMP != null)
                    {
                        combatTextTMP.color = MagicalCriticalDamageColor;
                        combatTextTMP.font = CombatTextFont;
                        combatTextTMP.fontSize = 65;
                        combatTextTMP.enableVertexGradient = false;

                        float RdmX = UnityEngine.Random.Range(-70, 70);
                        float RdmY = UnityEngine.Random.Range(-70, 70);

                        startPosition.x = RdmX;
                        startPosition.y = RdmY;
                        //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                    }
                }

            }*/
            else if (eventType == "CombatExpGained")
            {
                if (combatText != null)
                    combatText.text = "Exp: " + message;
                if (combatTextTMP != null)
                {
                    combatTextTMP.text = "Exp: " + message;
                    combatTextTMP.color = XPTextColor;
                    combatTextTMP.font = XPFont;
                    combatTextTMP.fontSize = 30;
                    combatTextTMP.enableVertexGradient = false;
                    startPosition.x = 0;
                    //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                }
            }
            if (eventType == "CombatHeal")
            {
                if (mobNode is AtavismPlayer)
                {
                    if (mobNode.Oid == ClientAPI.GetPlayerOid())
                    {
                        if (combatText != null)
                            combatText.color = HealColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = HealColor;
                            combatTextTMP.font = SelfDamageFont;
                            combatTextTMP.fontSize = 40;
                            combatTextTMP.enableVertexGradient = false;
                            combatTextTMP.text = "+ " + message;
                            startPosition.x = -225;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(-300, 0, 0);
                        }
                    }
                    else
                    {
                        if (combatText != null)
                            combatText.color = HealColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = HealColor;
                            combatTextTMP.font = CombatHealFont;
                            combatTextTMP.fontSize = 20;
                            combatTextTMP.enableVertexGradient = false;

                            float RdmX = UnityEngine.Random.Range(-70, 70);
                            float RdmY = UnityEngine.Random.Range(-70, 70);

                            startPosition.x = RdmX;
                            startPosition.y = RdmY;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                }
                else
                {
                    if (combatText != null)
                        combatText.color = HealColor;
                    if (combatTextTMP != null)
                    {
                        combatTextTMP.color = HealColor;
                        combatTextTMP.font = CombatHealFont;
                        combatTextTMP.fontSize = 20;
                        combatTextTMP.enableVertexGradient = false;

                        float RdmX = UnityEngine.Random.Range(-70, 70);
                        float RdmY = UnityEngine.Random.Range(-70, 70);

                        startPosition.x = RdmX;
                        startPosition.y = RdmY;
                        //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                    }
                }

            }
            else  if (eventType == "CombatHealCritical")
            {
                IsCritical = true;
                stopDisplay = Time.time + CriticalDmgDisplayTime;
                if (mobNode is AtavismPlayer)
                {
                    if (mobNode.Oid == ClientAPI.GetPlayerOid())
                    {
                        if (combatText != null)
                            combatText.color = CriticalHealColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = CriticalHealColor;
                            combatTextTMP.font = CombatHealCritFont;
                            combatTextTMP.fontSize = 65;
                            combatTextTMP.enableVertexGradient = false;
                            combatTextTMP.text = "+ " + message;
                            startPosition.x = -225;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(-300, 0, 0);
                        }
                    }
                    else
                    {
                        if (combatText != null)
                            combatText.color = CriticalHealColor;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = CriticalHealColor;
                            combatTextTMP.font = CombatHealCritFont;
                            combatTextTMP.fontSize = 65;
                            combatTextTMP.enableVertexGradient = false;

                            float RdmX = UnityEngine.Random.Range(-70, 70);
                            float RdmY = UnityEngine.Random.Range(-70, 70);

                            startPosition.x = RdmX;
                            startPosition.y = RdmY;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                }
                else
                {
                    if (combatText != null)
                        combatText.color = CriticalHealColor;
                    if (combatTextTMP != null)
                    {
                        combatTextTMP.color = CriticalHealColor;
                        combatTextTMP.font = CombatHealCritFont;
                        combatTextTMP.fontSize = 65;
                        combatTextTMP.enableVertexGradient = false;

                        float RdmX = UnityEngine.Random.Range(-70, 70);
                        float RdmY = UnityEngine.Random.Range(-70, 70);

                        startPosition.x = RdmX;
                        startPosition.y = RdmY;
                        //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                    }
                }

            }
            else if (eventType == "CombatBuffGained")
            {
                if (mobNode is AtavismPlayer)
                {
                    if (combatText != null)
                        combatText.color = BuffGainedColor;
                    if (combatTextTMP != null)
                    {
                        combatTextTMP.color = BuffGainedColor;
                        combatTextTMP.font = BuffGainedFont;
                        combatTextTMP.fontSize = 20;
                        combatTextTMP.enableVertexGradient = false;
                        startPosition.x = 0;
                        //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                    }
                }
                else
                {
                    if (combatText != null)
                        combatText.color = BuffGainedColor;
                    if (combatTextTMP != null)
                    {
                        combatTextTMP.color = BuffGainedColor;
                        combatTextTMP.font = BuffGainedFont;
                        combatTextTMP.fontSize = 20;
                        combatTextTMP.enableVertexGradient = false;
                        startPosition.x = 0;
                        //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                    }
                }
            }
            else if (eventType == "CombatDebuffGained")
            {
                if (mobNode is AtavismPlayer)
                {
                    if (combatText != null)
                        combatText.color = DebuffGainedColor;
                    if (combatTextTMP != null)
                    {
                        combatTextTMP.color = DebuffGainedColor;
                        combatTextTMP.font = DebuffGainedFont;
                        combatTextTMP.fontSize = 20;
                        combatTextTMP.enableVertexGradient = false;
                        startPosition.x = 0;
                        //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                    }
                }
                else
                {
                    if (combatText != null)
                        combatText.color = DebuffGainedColor;
                    if (combatTextTMP != null)
                    {
                        combatTextTMP.color = DebuffGainedColor;
                        combatTextTMP.font = DebuffGainedFont;
                        combatTextTMP.fontSize = 20;
                        combatTextTMP.enableVertexGradient = false;
                        startPosition.x = 0;
                        //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                    }
                }
            }
            else
            {



                if (eventType != "CombatDebuffGained" && eventType != "CombatBuffGained" && eventType != "CombatHeal" && eventType != "CombatHealCritical" && eventType != "CombatExpGained" &&
                    eventType != "CombatDamage" && eventType != "CombatDamageCritical" )
                {
                    print(eventType);
                    //print("non displayed event type is : " + eventType);

                    if (mobNode is AtavismPlayer)
                    {
                        if (combatText != null)
                            combatText.color = myMessageColour;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = myMessageColour;
                            combatTextTMP.enableVertexGradient = false;
                            combatTextTMP.font = MessageFont;
                            combatTextTMP.fontSize = 20;
                            startPosition.x = 0;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                    else
                    {
                        if (combatText != null)
                            combatText.color = targetMessageColour;
                        if (combatTextTMP != null)
                        {
                            combatTextTMP.color = targetMessageColour;
                            combatTextTMP.enableVertexGradient = false;
                            combatTextTMP.font = MessageFont;
                            combatTextTMP.fontSize = 20;
                            startPosition.x = 0;
                            //combatTextTMP.rectTransform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                }
            }
        }

        public void LevelHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateNameDisplay(showName);
        }

        public void TargetTypeHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateNameDisplay(showName);
        }

        public void AdminLevelHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateNameDisplay(showName);
        }

        public void NameDisplayHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateNameDisplay(showName);
        }

        public void GuildNameDisplayHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateNameDisplay(showName);
        }
        public void QuestAvailableHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateNameDisplay(showName);
        }
        public void QuestInProgressHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateNameDisplay(showName);
        }
        public void QuestConcludableHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateNameDisplay(showName);
        }
        public void DialogueAvailableHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateNameDisplay(showName);
        }
        public void ItemsToSellHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateNameDisplay(showName);
        }

        public void ShowChatBubble(string text)
        {
            if (chatPanel != null)
                chatPanel.gameObject.SetActive(true);
            if (chatText != null)
            {
                int numLines = text.Length / 60;
                for (int i = 0; i < numLines; i++)
                {
                    int spacePos = text.IndexOf(" ", (i + 1) * 60);
                    if (spacePos > 0)
                        text = text.Insert(spacePos, "\n");
                }
                chatText.text = text;
                stopDisplay = Time.time + chatDisplayTime;
            }
            if (chatTextTMP != null)
            {
                int numLines = text.Length / 60;
                for (int i = 0; i < numLines; i++)
                {
                    int spacePos = text.IndexOf(" ", (i + 1) * 60);
                    if (spacePos > 0)
                        text = text.Insert(spacePos, "\n");
                }
                chatTextTMP.text = text;
                stopDisplay = Time.time + chatDisplayTime;
            }
        }

        DamageType getDamageTypeColor(string damageType)
        {
            DamageType d = new DamageType() {criticDamageColor = CriticalDamageColor, type = "", damageColor = DamageColor};
            foreach (var dt in damageTypeColor)
            {
                if (dt.type.Equals(damageType))
                {
                    d = dt;
                }
            }

            return d;
        }
        public void HideChatBubble()
        {
            Debug.LogError("HideChatBubble",gameObject);
            FloatingPanelController.ALLcombatTextData[ThisPanelID].CurrentNode = null;
            FloatingPanelController.ALLcombatTextData[ThisPanelID].RendererReference = null;
            FloatingPanelController.ALLcombatTextData[ThisPanelID].LastTimeUsed = 0;
            FloatingPanelController.ALLcombatTextData[ThisPanelID].FloatingPanelGO.SetActive(false);


            if (chatText != null)
            {
                chatText.text = "";
            }
            if (chatTextTMP != null)
            {
                chatTextTMP.text = "";
            }
            /*
            if (chatPanel != null)
                chatPanel.gameObject.SetActive(false);*/
        }
    }
}