using System;
using UnityEngine;
using System.Collections;
using EasyBuildSystem.Features.Scripts.Core.Base.Area;

namespace Atavism
{
    public class ClaimScript : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            string[] args = new string[1];
            args[0] = gameObject.name;
            AtavismEventSystem.DispatchEvent("CLAIM_ADDED", args);
        }

        void ObjectNodeReady()
        {
            GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("scale", ScaleHandler);
            if (GetComponent<AtavismNode>().PropertyExists("scale"))
            {
                AtavismLogger.LogDebugMessage("Got scale");
                Vector3 scaleObj = (Vector3)GetComponent<AtavismNode>().GetProperty("scale");
                gameObject.transform.localScale = scaleObj;
                AreaBehaviour ab =gameObject.GetComponent<AreaBehaviour>();
                if (ab != null)
                {
                    ab.Bounds.extents = scaleObj;
                }
            }
            GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("loc", LocHandler);
            if (GetComponent<AtavismNode>().PropertyExists("loc"))
            {
                AtavismLogger.LogDebugMessage("Got loc");
                Vector3 loc = (Vector3)GetComponent<AtavismNode>().GetProperty("loc");
                gameObject.transform.position = loc;
                AreaBehaviour ab =gameObject.GetComponent<AreaBehaviour>();
               
            }
        }

        private void OnEnable()
        {
            if (GetComponent<AtavismNode>() != null)
            {
                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("scale", ScaleHandler);
                if (GetComponent<AtavismNode>().PropertyExists("scale"))
                {
                    AtavismLogger.LogDebugMessage("Got scale");
                    Vector3 scaleObj = (Vector3) GetComponent<AtavismNode>().GetProperty("scale");
                    gameObject.transform.localScale = scaleObj;
                    AreaBehaviour ab = gameObject.GetComponent<AreaBehaviour>();
                    if (ab != null)
                    {
                        ab.Bounds.extents = scaleObj;
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("loc", LocHandler);
                if (GetComponent<AtavismNode>().PropertyExists("loc"))
                {
                    AtavismLogger.LogDebugMessage("Got loc");
                    Vector3 loc = (Vector3) GetComponent<AtavismNode>().GetProperty("loc");
                    gameObject.transform.position = loc;
                    AreaBehaviour ab = gameObject.GetComponent<AreaBehaviour>();

                }
            }
        }

        void OnDestroy()
        {
            string[] args = new string[1];
            args[0] = gameObject.name;
            AtavismEventSystem.DispatchEvent("CLAIM_REMOVED", args);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ScaleHandler(object sender, PropertyChangeEventArgs args)
        {
            AtavismLogger.LogDebugMessage("Got scale");
            Vector3 scaleObj = (Vector3)GetComponent<AtavismNode>().GetProperty("scale");
            gameObject.transform.localScale = scaleObj;
            AreaBehaviour ab =gameObject.GetComponent<AreaBehaviour>();
            if (ab != null)
            {
                ab.Bounds.extents = scaleObj;
            }
        }
        public void LocHandler(object sender, PropertyChangeEventArgs args)
        {
            AtavismLogger.LogDebugMessage("Got loc");
            Vector3 loc = (Vector3)GetComponent<AtavismNode>().GetProperty("loc");
            gameObject.transform.position = loc;
        }
    }
}