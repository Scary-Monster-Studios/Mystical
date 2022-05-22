using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atavism
{
    public class AtavismHealthBar : MonoBehaviour
    {
        [SerializeField] float height = 2f;
        [SerializeField] float defaultHeight = 2f;
        [SerializeField] Vector3 coliderAdditionalHeight = Vector3.zero;
        [SerializeField] float renderDistance = 50f;
        public bool hideWhenFull = true;
        public bool hideWhenDead = true;

        public string prop;
        public string propMax;
        public Image fillImage;
        public TextMeshProUGUI fillText;
        int value = 1;
        int valueMax = 1;
        bool death = false;
        AtavismNode node;
        GameObject go = null;
        Coroutine coroutine = null;

        private ClaimObject cObject;
        // Start is called before the first frame update
        void Start()
        {
            node = GetComponent<AtavismNode>();
            if (node == null)
            {
                if (transform.parent != null)
                    node = transform.parent.GetComponent<AtavismNode>();
            }
            if (node != null)
            {

                if (node.gameObject.GetComponent<CharacterController>() != null)
                {
                    height = node.gameObject.GetComponent<CharacterController>().height > 2 * node.gameObject.GetComponent<CharacterController>().radius ? node.gameObject.GetComponent<CharacterController>().height : node.gameObject.GetComponent<CharacterController>().radius * 2;
                }
                else
                    height = defaultHeight;
                transform.localPosition = new Vector3(0f, height, 0f)+ coliderAdditionalHeight;
                node.RegisterObjectPropertyChangeHandler(prop, PropHandler);
                node.RegisterObjectPropertyChangeHandler(propMax, PropMaxHandler);
                node.RegisterObjectPropertyChangeHandler("deadstate", HandleDeadState);
                coroutine = StartCoroutine(UpdateTimer());
            }
            else
            {
                transform.localPosition = transform.localPosition + new Vector3(0f, height, 0f)+coliderAdditionalHeight;
                coroutine = StartCoroutine(UpdateTimer());
            }

        }

        public void setClaimObject(ClaimObject co)
        {
            //  Debug.LogError("setClaimObject " + co);
            cObject = co;
            node = null;
            // transform.localPosition = cObject.transform.localPosition + new Vector3(0f, height, 0f) + coliderAdditionalHeight;
            transform.position = cObject.transform.position + cObject.positionProgressBar;

        }

        public void HandleDeadState(object sender, PropertyChangeEventArgs args)
        {
            death = (bool)node.GetProperty("deadstate");
        }

        public void PropHandler(object sender, PropertyChangeEventArgs args)
        {
            value = (int)node.GetProperty(prop);
            UpdateProgressBar();
        }

        public void PropMaxHandler(object sender, PropertyChangeEventArgs args)
        {
            valueMax = (int)node.GetProperty(propMax);
            UpdateProgressBar();
        }
        private void OnDestroy()
        {
            if (node != null)
            {
                node.RemoveObjectPropertyChangeHandler(prop, PropHandler);
                node.RemoveObjectPropertyChangeHandler(propMax, PropMaxHandler);
                node.RemoveObjectPropertyChangeHandler("deadstate", HandleDeadState);
            }
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        void UpdateProgressBar()
        {
            try
            {
                if (cObject != null)
                {
                    value = cObject.Health;
                    valueMax = cObject.MaxHealth;
                }
                else if (node != null && cObject == null)
                {
                    value = (int) node.GetProperty(prop);
                    valueMax = (int) node.GetProperty(propMax);
                }
            }
            catch (System.Exception e)
            {
              //  Debug.LogError("UpdateProgressBar Exception " + e, gameObject);

            }


            if (fillImage != null)
                fillImage.fillAmount = (float)value / (float)valueMax;
            if (fillText != null)
                fillText.text = value + " / " + valueMax;
        }

        IEnumerator UpdateTimer()
        {
            WaitForSeconds delay = new WaitForSeconds(0.02f);
            WaitForSeconds delay2 = new WaitForSeconds(0.1f);
            while (Camera.main == null)
            {
                yield return delay2;
            }
            while (Camera.main != null)
            {
                 transform.rotation = Camera.main.transform.rotation;
                UpdateProgressBar();
                 float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
                 if (distance < renderDistance)
                 {
                     transform.rotation = Camera.main.transform.rotation;

                     if (!death)
                     {
                         if (hideWhenFull && value == valueMax)
                         {
                            if (GetComponent<Canvas>() != null)
                                transform.GetComponent<Canvas>().enabled = false;
                         }
                         else
                         {
                            if (GetComponent<Canvas>() != null)
                                transform.GetComponent<Canvas>().enabled = true;
                             UpdateProgressBar();
                         }

                     }
                     else
                     {
                         if (hideWhenDead)
                            if (GetComponent<Canvas>() != null)
                                transform.GetComponent<Canvas>().enabled = false;
                     }
                 }
                 else
                 {
                    if (GetComponent<Canvas>() != null)
                        transform.GetComponent<Canvas>().enabled = false;
                 }
                yield return delay;
            }
            yield return delay;

        }
    }
}