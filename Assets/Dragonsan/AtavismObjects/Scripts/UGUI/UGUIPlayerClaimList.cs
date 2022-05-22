using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{
    public class UGUIPlayerClaimList : MonoBehaviour
    {
        [SerializeField] UGUIClaimListEntry prefab;
        [SerializeField] List<UGUIClaimListEntry> claims = new List<UGUIClaimListEntry>();
        private bool showing = false;
        [SerializeField] Transform grid;


        [SerializeField] bool autoShowHide = false;

        // Start is called before the first frame update
        void Start()
        {
            AtavismEventSystem.RegisterEvent("CLAIM_LIST_UPDATE", this);
            UpdateDisplay();
            Hide();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CLAIM_LIST_UPDATE", this);
        }


        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CLAIM_LIST_UPDATE")
            {

                if (!showing && autoShowHide)
                    Show();

                UpdateDisplay();
            }
        }

        void UpdateDisplay()
        {
            int i = 0;
            foreach (ClaimListEntry cle in WorldBuilder.Instance.PlayerClaims)
            {
                if (i >= claims.Count)
                    claims.Add(Instantiate(prefab, grid));
                claims[i].gameObject.SetActive(true);
                claims[i].UpdateDisplay(cle.name, cle.id, cle.time,cle.taxAmount,cle.taxCurrency,cle.taxInterval,cle.taxPeriodPay);
                i++;
            }

            for (int j = i; j < claims.Count; j++)
                claims[i].gameObject.SetActive(false);

            if (WorldBuilder.Instance.PlayerClaims.Count == 0 && autoShowHide)
            {
                Hide();

            }
        }

        public void Show()
        {
            AtavismUIUtility.BringToFront(gameObject);
            UpdateDisplay();
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            showing = true;

        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
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