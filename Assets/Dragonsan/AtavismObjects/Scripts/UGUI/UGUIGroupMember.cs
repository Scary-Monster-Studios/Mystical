using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace Atavism
{

    public class UGUIGroupMember : MonoBehaviour, IPointerClickHandler
    {

        public Text name;
        public TextMeshProUGUI nameText2;
        public Text levelText;
        public TextMeshProUGUI levelText2;
        public Image portrait;
        public Image portraitOffline;
        public Image portraitAway;
        public Slider healthBar;
        public Image ihealthBar;
        public Text healthText;
        public TextMeshProUGUI healthText2;
        public GameObject dHealthBar;
        public Slider manaBar;
        public Image imanaBar;
        public Text manaText;
        public TextMeshProUGUI manaText2;
        public GameObject dManaBar;
        public RectTransform popupMenu;
        public Image leaderIcon;
        GroupMember member;
        string characterName;
        string gender;
        //	int classID = -1;
        string level;
        bool portraitNeedsLoaded = false;
        [SerializeField]
        bool activeEffect = true;
        AtavismObjectNode node;
        public List<UGUIEffect> effectButtons;
        List<AtavismEffect> memberEffects = new List<AtavismEffect>();
        public float fillMinWidth = 35f;
        public float fillMaxWidth = 213f;
        [SerializeField] private string healthStat = "health";
        [SerializeField] private string healthMaxStat = "health-max";
        [SerializeField] private string manaStat = "mana";
        [SerializeField] private string manaMaxStat = "mana-max";

        // Use this for initialization
        void Awake()
        {
            if (leaderIcon != null)
                leaderIcon.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
        }

        void OnDisable()
        {
            //        if (node != null)
            //           if (node.GameObject != null)
            //                Destroy(node.GameObject.GetComponent<bl_MiniMapItem>());
            node = null;
            this.member = null;
        }

        public void deactive()
        {
            //        if (node != null)
            //            Destroy(node.GameObject.GetComponent<bl_MiniMapItem>());
            node = null;
            this.member = null;
        }

        void Update()
        {
            if (portraitNeedsLoaded)
            {
                SetPortrait();
            }
            if (member != null)
            {
                if (node != null && node.GameObject == null)
                {
                    node = null;
                }
                if (node == null)
                {
                    node = ClientAPI.WorldManager.GetObjectNode(member.oid);
                }
                /*            if (node != null && node.GameObject != null && node.GameObject.GetComponent<bl_MiniMapItem>() == null) {
                                mmi = node.GameObject.AddComponent<bl_MiniMapItem>();
                                mmi.Target = node.GameObject.transform;
                                mmi.SetIcon(GameSettings.Instance.MinimapSettings.minimapIcon);
                                mmi.SetColor(Color.blue);
                                mmi.Size = 14;
                                mmi.InfoItem = "";
                            }
                            */
                //    CheckEffects();
            }

        }

        public void UpdateGroupMember(GroupMember member)
        {
            this.member = member;
            if (member == null)
                return;
            /*    if (node == null) {
                    node = ClientAPI.WorldManager.GetObjectNode(member.oid);
                    if (node != null && node.GameObject != null) {
                        mmi = node.GameObject.AddComponent<bl_MiniMapItem>();
                        mmi.Target = node.GameObject.transform;
                        mmi.SetIcon(AtavismSettings.Instance.MinimapSettings.minimapIcon);
                        mmi.SetColor(Color.blue);
                        mmi.Size = 7;
                        mmi.InfoItem = "";
                    }
                }
                */
            if (name != null)
                name.text = member.name;
            if (nameText2 != null)
                nameText2.text = member.name;
            if (levelText != null)
            {
                if (member.properties.ContainsKey("level"))
                {
                    levelText.text = member.properties["level"].ToString();
                }
            }
            if (levelText2 != null)
            {
                if (member.properties.ContainsKey("level"))
                {
                    levelText2.text = member.properties["level"].ToString();
                }
            }

            if (portraitOffline != null)
                portraitOffline.enabled = false;
            if (member.status == 0)
            {
                if (portraitOffline != null)
                    portraitOffline.enabled = true;
                if (healthText != null)
                {
                    healthText.text = "Offline";
                }
            }
            else if (member.properties.ContainsKey(healthStat) && member.properties.ContainsKey(healthMaxStat))
            {
                int health = (int)member.properties[healthStat];
                int maxHealth = (int)member.properties[healthMaxStat];
                if (healthBar != null)
                {
                    healthBar.value = (float)health / (float)maxHealth;
                }
                if (ihealthBar != null)
                {
                    ihealthBar.fillAmount = (float)health / (float)maxHealth;
                }
                if (healthText != null)
                {
                    healthText.text = member.properties[healthStat].ToString() + " / " + member.properties[healthMaxStat].ToString();
                }
                if (healthText2 != null)
                {
                    healthText2.text = health.ToString() + " / " + maxHealth.ToString();
                }
                if (dHealthBar != null)
                {
                    UpdateBarFill(dHealthBar, (float)health / (float)maxHealth);
                }

            }
            if (member.properties.ContainsKey(manaStat) && member.properties.ContainsKey(manaMaxStat))
            {
                int mana = (int)member.properties[manaStat];
                int maxMana = (int)member.properties[manaMaxStat];
                if (manaBar != null)
                {
                    manaBar.value = (float)mana / (float)maxMana;
                }
                if (imanaBar != null)
                {
                    imanaBar.fillAmount = (float)mana / (float)maxMana;
                }
                if (manaText != null)
                {
                    manaText.text = member.properties[manaStat].ToString() + " / " + member.properties[manaMaxStat].ToString();
                }
                if (manaText2 != null)
                {
                    manaText2.text = mana.ToString() + " / " + maxMana.ToString();
                }
                if (dManaBar != null)
                {
                    UpdateBarFill(dManaBar, (float)mana / (float)maxMana);
                }
            }
            else
                Debug.LogError("No mana param");

            SetPortrait();

            if (leaderIcon != null)
            {
                if (AtavismGroup.Instance.LeaderOid == member.oid)
                {
                    leaderIcon.gameObject.SetActive(true);
                }
                else
                {
                    leaderIcon.gameObject.SetActive(false);
                }
            }
        }

        void SetPortrait()
        {
            if (portrait != null)
            {
                Sprite portraitSprite = null;
                if (member.properties.ContainsKey("portrait") && member.properties["portrait"] != null && ((string)member.properties["portrait"]).Length > 0)
                {
                    portraitSprite = PortraitManager.Instance.LoadPortrait((string)member.properties["portrait"]);
                }
                else if (member.properties.ContainsKey(" custom: portrait") && member.properties[" custom: portrait"] != null && ((string)member.properties[" custom: portrait"]).Length > 0)
                {
                    portraitSprite = PortraitManager.Instance.LoadPortrait((string)member.properties[" custom: portrait"]);
                }
                else if (ClientAPI.GetObjectNode(member.oid.ToLong()) != null && ClientAPI.GetObjectNode(member.oid.ToLong()).GameObject != null)
                {
                    portraitSprite = PortraitManager.Instance.LoadPortrait(ClientAPI.GetObjectNode(member.oid.ToLong()).GameObject.GetComponent<AtavismNode>());
                    portraitNeedsLoaded = false;
                }
                else
                {
                    portraitNeedsLoaded = true;
                }


                if (portraitSprite != null)
                    portrait.sprite = portraitSprite;
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ClientAPI.SetTarget(member.oid.ToLong());
                return;
            }
            if (eventData.button != PointerEventData.InputButton.Right)
                return;

            if (popupMenu.gameObject.activeSelf)
            {
                popupMenu.gameObject.SetActive(false);
                AtavismSettings.Instance.DsContextMenu(null);
                return;
            }

            // Verify the player is group leader
            if (AtavismGroup.Instance.LeaderOid.ToLong() != ClientAPI.GetPlayerOid())
                return;

            // Work out what to put in the popup menu here
            popupMenu.gameObject.SetActive(true);
            AtavismSettings.Instance.DsContextMenu(popupMenu.gameObject);
        }

        public void PromoteToLeader()
        {
            AtavismGroup.Instance.PromoteToLeader(member.oid);
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }

        public void Kick()
        {
            AtavismGroup.Instance.RemoveGroupMember(member.oid);
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }
        public void UpdateBarFill(GameObject go, float fillAmount)
        {
            // Update the bar fill by changing it's width
            // we are doing it this way because we are using a mask on the bar and have it's fill inside with static width and position
            (go.transform as RectTransform).SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                Mathf.Round(this.fillMinWidth + ((this.fillMaxWidth - this.fillMinWidth) * fillAmount))
            );
        }

        public void UpdateEffectButtons()
        {
            for (int i = 0; i < effectButtons.Count; i++)
            {
                if (i < Abilities.Instance.PlayerEffects.Count)
                {
                    effectButtons[i].gameObject.SetActive(true);
                    effectButtons[i].SetEffect(Abilities.Instance.PlayerEffects[i], i);
                }
                else
                {
                    effectButtons[i].gameObject.SetActive(false);
                }
            }
        }

        public void CheckEffects()
        {
            AtavismEffect[] aEffects = null;
            LinkedList<object> effects_prop = new LinkedList<object>();
            float effects_prop_time = Time.time;
            if (member.properties.ContainsKey("effects"))
                effects_prop = (LinkedList<object>)member.properties["effects"];
            if (member.properties.ContainsKey("effects_t"))
                effects_prop_time = (float)member.properties["effects_t"];
            memberEffects.Clear();
            //  int pos = 0;
            foreach (string effectsProp in effects_prop)
            {
                string[] effectData = effectsProp.Split(',');
                int effectID = int.Parse(effectData[0]);
                //  int stack = int.Parse(effectData[1]);
                //   bool isBuff = bool.Parse(effectData[2]);
                //    long endTime = long.Parse(effectData[3]);

                long timeUntilEnd = long.Parse(effectData[4]);
                bool active = bool.Parse(effectData[5]);
                long duration = long.Parse(effectData[6]);
                //    bool passive = bool.Parse(effectData[7]);

                float secondsLeft = (float)timeUntilEnd / 1000f;
                //      Debug.LogError("Group Member active " + active);
                AtavismEffect effect = null;
             /*   foreach (AtavismEffect aEffect in aEffects)
                {
                    if (aEffect.id.Equals(effectID))
                    {
                        effect = aEffect;
                        break;
                    }
                }*/
                if (effect == null)
                    if (Abilities.Instance.GetEffect(effectID) != null)
                        effect = Abilities.Instance.GetEffect(effectID).Clone();
                if (effect == null)
                {
                    UnityEngine.Debug.LogWarning("Effect " + effectID + " does not exist");
                    continue;
                }
                effect.StackSize = int.Parse(effectData[1]);
                effect.isBuff = bool.Parse(effectData[2]);
                effect.Expiration = Time.time + secondsLeft - (Time.time - effects_prop_time);
                effect.Active = active;
                effect.Length = (float)duration / 1000f;
                effect.startTime = long.Parse(effectData[9]);
                memberEffects.Add(effect);
            }
            memberEffects = memberEffects.OrderBy(x => x.startTime).ToList();
            UpdateEffects();
        }

        private void UpdateEffects()
        {
            for (int i = 0; i < effectButtons.Count; i++)
            {
                if (i < memberEffects.Count)
                {
                    if (activeEffect && memberEffects[i].Active == false)
                    {
                        effectButtons[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        effectButtons[i].gameObject.SetActive(true);
                        effectButtons[i].SetEffect(memberEffects[i], i, false);
                    }
                }
                else
                {
                    effectButtons[i].gameObject.SetActive(false);
                }
            }
        }

    }
}