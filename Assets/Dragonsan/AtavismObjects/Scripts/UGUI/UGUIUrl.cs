using UnityEngine;
using System.Collections;

namespace Atavism
{

    public class UGUIUrl : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void ClickCreateUrl()
        {
            AtavismSettings.Instance.ClickCreateUrl();
        }
        public void ClickForgotUrl()
        {
            AtavismSettings.Instance.ClickForgotUrl();
        }
        public void ClickWebPageUrl()
        {
            AtavismSettings.Instance.ClickWebPageUrl();
        }
        public void ClickShopWebPageUrl()
        {
            AtavismSettings.Instance.ClickShopWebPageUrl();
        }
        public void ClickClose()
        {
            Application.Quit();
        }

    }
}