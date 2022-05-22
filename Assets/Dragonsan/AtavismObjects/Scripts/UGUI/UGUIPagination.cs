using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{
    public class UGUIPagination : MonoBehaviour
    {


        public Button buttonPrev;
        public Button buttonNext;
        public Transform pagesParent;

        private int activePage = 0;

        void Start()
        {
            if (this.buttonPrev != null)
                this.buttonPrev.onClick.AddListener(OnPrevClick);
            if (this.buttonNext != null)
                this.buttonNext.onClick.AddListener(OnNextClick);
            if (this.pagesParent != null && this.pagesParent.childCount > 0)
            {
                for (int i = 0; i < this.pagesParent.childCount; i++)
                {
                    if (this.pagesParent.GetChild(i).gameObject.activeSelf)
                    {
                        this.activePage = i;
                        break;
                    }
                }
            }
            this.UpdatePages();
        }

        private void UpdatePages()
        {
            if (this.pagesParent != null && this.pagesParent.childCount > 0)
            {
                for (int i = 0; i < this.pagesParent.childCount; i++)
                    this.pagesParent.GetChild(i).gameObject.SetActive((i == activePage) ? true : false);
            }
        }

        private void OnPrevClick()
        {
            if (!this.enabled || this.pagesParent == null)
                return;
            if (this.activePage == 0)
                this.activePage = this.pagesParent.childCount - 1;
            else
                this.activePage -= 1;
            this.UpdatePages();
        }

        private void OnNextClick()
        {
            if (!this.enabled || this.pagesParent == null)
                return;
            if (this.activePage == (this.pagesParent.childCount - 1))
                this.activePage = 0;
            else
                this.activePage += 1;
            this.UpdatePages();
        }
    }



}