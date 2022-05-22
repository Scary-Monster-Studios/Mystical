using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUIAnnouncementText : MonoBehaviour
    {

        class AnnouncementTextInfo
        {
            public RectTransform rectTransform;
            public float endTime;
            public Vector2 position;
            public float offset;
        }

        public Text announcementPrefab;
        public TextMeshProUGUI announcementTextMeshPrefab;

        public float displayTime = 3f;
        public float verticalTravelDistance = 100f;
        public float spacingReq = 20f;
        float stopDisplay;
        //bool showing = false;
        //  int position = -1;
        string lastmessage = "";
        float lasttime = 0;
        List<AnnouncementTextInfo> activeAnnouncements = new List<AnnouncementTextInfo>();
        AnnouncementTextInfo transformToRemove = null;

        // Use this for initialization
        void Start()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("ANNOUNCEMENT", this);
            AtavismEventSystem.RegisterEvent("INVENTORY_EVENT", this);

            NetworkAPI.RegisterExtensionMessageHandler("announcement", HandleAnnouncementMessage);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ANNOUNCEMENT", this);
            AtavismEventSystem.UnregisterEvent("INVENTORY_EVENT", this);

            NetworkAPI.RemoveExtensionMessageHandler("announcement", HandleAnnouncementMessage);
        }

        // Update is called once per frame
        void Update()
        {
            /*if (showing && Time.time > stopDisplay) {
                Hide ();
            }*/

            foreach (AnnouncementTextInfo announcementInfo in activeAnnouncements)
            {
                if (announcementInfo.endTime < Time.time)
                {
                    transformToRemove = announcementInfo;
                }/* else {
				// work out movement
				float timePercent = (announcementInfo.endTime - Time.time) / displayTime;
				
				announcementInfo.position = new Vector2(0, verticalTravelDistance * (1.0f - timePercent) + announcementInfo.offset);
				announcementInfo.rectTransform.anchoredPosition = announcementInfo.position;
			}*/
            }

            if (transformToRemove != null)
            {
                DestroyImmediate(transformToRemove.rectTransform.gameObject);
                activeAnnouncements.Remove(transformToRemove);
                transformToRemove = null;
            }
            if (activeAnnouncements.Count > 5)
            {
                Destroy(activeAnnouncements[0].rectTransform.gameObject);
                activeAnnouncements.RemoveAt(0);
            }

            // Check if any elements are overlapping
            //  bool overlapping = true;
            //while (overlapping) {
            //	overlapping = false;

            for (int i = 1; i < activeAnnouncements.Count; i++)
            {
                if (Mathf.Abs(activeAnnouncements[i].position.y - activeAnnouncements[i - 1].position.y) < spacingReq)
                {
                    activeAnnouncements[i - 1].offset += spacingReq;
                    activeAnnouncements[i - 1].position = new Vector2(0, activeAnnouncements[i - 1].position.y + activeAnnouncements[i - 1].offset);
                    activeAnnouncements[i - 1].rectTransform.anchoredPosition = activeAnnouncements[i - 1].position;
                    //	overlapping = true;
                }
            }
            //}
        }

        void Show(string message)
        {
            if (string.IsNullOrEmpty(message) || (message.Equals(lastmessage) && (lasttime + 0.5f) > Time.time))
                return;
            lastmessage = message;
            lasttime = Time.time;
            //   if (activeAnnouncements.Count < 5) position = -1;
            if (activeAnnouncements.Count < 5)
            {
                GameObject announcementGO = null;
                if (announcementPrefab != null)
                    announcementGO = Instantiate(announcementPrefab.gameObject, transform, false);
                if (announcementTextMeshPrefab != null)
                    announcementGO = Instantiate(announcementTextMeshPrefab.gameObject, transform, false);
                if (announcementGO != null)
                {
                    RectTransform announcementText = announcementGO.GetComponent<RectTransform>();
                    //   announcementText.parent = transform;
                    announcementText.anchoredPosition = Vector2.zero;
                    announcementGO.transform.SetSiblingIndex(0);
                    if (announcementText.GetComponent<Text>() != null)
                        announcementText.GetComponent<Text>().text = message;
                    if (announcementText.GetComponent<TextMeshProUGUI>() != null)
                        announcementText.GetComponent<TextMeshProUGUI>().text = message;
                    //           announcementText.GetComponent<TextMeshProUGUI>().text = message;
                    AnnouncementTextInfo announcementInfo = new AnnouncementTextInfo();
                    announcementInfo.rectTransform = announcementText;
                    announcementInfo.endTime = Time.time + displayTime;
                    announcementInfo.position = Vector2.zero;
                    activeAnnouncements.Add(announcementInfo);
                }
                //GetComponent<CanvasGroup>().alpha = 1f;
                //stopDisplay = Time.time + 3;
                //showing = true;
            }
            else
            {
                AnnouncementTextInfo announcementInfo = activeAnnouncements[0];
                activeAnnouncements.RemoveAt(0);
                if (announcementInfo.rectTransform.GetComponent<Text>() != null)
                    announcementInfo.rectTransform.GetComponent<Text>().text = message;
                if (announcementInfo.rectTransform.GetComponent<TextMeshProUGUI>() != null)
                    announcementInfo.rectTransform.GetComponent<TextMeshProUGUI>().text = message;
                announcementInfo.rectTransform.transform.SetSiblingIndex(0);
                announcementInfo.endTime = Time.time + displayTime;
                announcementInfo.position = Vector2.zero;
                activeAnnouncements.Add(announcementInfo);
            }

        }

        public void Hide()
        {
            //GetComponent<CanvasGroup>().alpha = 0f;
            //showing = false;
        }

        public void HandleAnnouncementMessage(Dictionary<string, object> props)
        {
            Show((string)props["AnnouncementText"]);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ANNOUNCEMENT")
            {
                Show(eData.eventArgs[0]);
            }
            else if (eData.eventType == "INVENTORY_EVENT")
            {
                if (eData.eventArgs[0] == "ItemHarvested")
                {
                    //	string[] args = new string[1];
                    AtavismInventoryItem item = Inventory.Instance.GetItemByTemplateID(int.Parse(eData.eventArgs[1]));
#if AT_I2LOC_PRESET
                string message = I2.Loc.LocalizationManager.GetTranslation("Received") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name) + " x" + eData.eventArgs[2];
#else
                    string message = "Received " + item.name + " x" + eData.eventArgs[2];
#endif
                    Show(message);
                }
            }
        }
    }
}