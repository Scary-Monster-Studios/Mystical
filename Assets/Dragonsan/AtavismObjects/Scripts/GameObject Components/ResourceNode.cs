using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    /* public enum HarvestType
     {
         Axe,
         Pickaxe,
         None
     }*/

    public class ResourceNode : MonoBehaviour
    {

        public int id = -1;

        //Profile for Resource Node defined in Atavism Editor
        public int profileId = -1;

        //id of the sub profile that was assigned
        private int settingId = -1;
        public float cooldown = 2;

        float cooldownEnds;

        public  float maxDistanceInteraction = 4f;
        // public float refreshDuration = 60;
        public Texture2D cursorIcon;
        public string highlightParam = "_Color";
        public bool highlight = true;
        public Color highlightColour = Color.cyan;
        public Sprite selectedIcon;

        public List<GameObject> subProfileGameObjects;

        public string activateCoordEffect;
        public string deactivateCoordEffect;

        public bool isLODChild = false;
        public float deactivateDelay = 0f;
        Color initialColor;
        bool active = true;
        bool selected = false;
        Renderer[] renderers;
        Color[] initialColors;
        bool mouseOver = false;

        private bool setIcons = false;
        // Use this for initialization
        void Start()
        {
            var rnd = AtavismPrefabManager.Instance.LoadResourceNodeData(profileId, settingId);
            if(rnd!=null)
            {
                selectedIcon = rnd.selectedIcon;
                cursorIcon = rnd.cursorIcon;
                setIcons = true;
            }
            cooldownEnds = Time.time;
            gameObject.AddComponent<AtavismNode>();
            GetComponent<AtavismNode>().AddLocalProperty("targetable", false);
            GetComponent<AtavismNode>().AddLocalProperty("active", active);
            if (highlight)
                if (GetComponent<Renderer>() != null)
                {
                    if (GetComponent<Renderer>().material.HasProperty(highlightParam))
                        initialColor = GetComponent<Renderer>().material.color;
                }
                else
                {
                    renderers = GetComponentsInChildren<Renderer>();
                    initialColors = new Color[renderers.Length];
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        if (renderers[i].material.HasProperty(highlightParam))
                            initialColors[i] = renderers[i].material.color;
                    }
                }

            // Add child component to all children with colliders
            foreach (Collider child in GetComponentsInChildren<Collider>())
            {
                if (child.gameObject != gameObject)
                    child.gameObject.AddComponent<ObjectChildMouseDetector>();
            }

            Crafting.Instance.RegisterResourceNode(this);
        }

        void OnDestroy()
        {
            if (ClientAPI.ScriptObject != null)
                Crafting.Instance.RemoveResourceNode(id);
            AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
            mouseOver = false;
        }

        void OnDisable()
        {
            AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
            mouseOver = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (mouseOver )
            {
                if (AtavismCursor.Instance.IsMouseOverUI())
                {
                    ResetHighlight();
                    AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
                }
                else
                {
                    Highlight();
                    AtavismCursor.Instance.SetMouseOverObject(GetComponent<AtavismNode>(), cursorIcon, maxDistanceInteraction);
                }

                if (Input.GetMouseButtonDown(1) && !AtavismCursor.Instance.IsMouseOverUI())
                {
                    HarvestResource();
                }
            }
        }

        void OnMouseDown()
        {
            if (!AtavismCursor.Instance.IsMouseOverUI() && !(ClientAPI.GetInputController() is ClickToMoveInputController))
                HarvestResource();
        }

        void OnMouseOver()
        {
            if (active)
            {
                AtavismCursor.Instance.SetMouseOverObject(GetComponent<AtavismNode>(), cursorIcon, maxDistanceInteraction);
                Highlight();
            }

            mouseOver = true;
        }

        void OnMouseExit()
        {
            AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
            if (!selected)
                ResetHighlight();
            mouseOver = false;
        }

        public void HarvestResource()
        {
         //   Debug.LogError("RN HarvestResource");
            if (Time.time < cooldownEnds)
            {
                // Send error message
                string[] args = new string[1];
#if AT_I2LOC_PRESET
            args[0] = I2.Loc.LocalizationManager.GetTranslation("You cannot perform that action yet");
#else
                args[0] = "You cannot perform that action yet.";
#endif
                AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
            }
            else
            {
                Crafting.Instance.HarvestResource(id);
                cooldownEnds = Time.time + cooldown;
                AtavismLogger.LogInfoMessage("Sending harvest resource");
            }
        }

        public void Highlight()
        {
            if (!highlight)
                return;
            if (GetComponent<Renderer>() != null)
            {
                if (GetComponent<Renderer>().material.HasProperty(highlightParam))
                    GetComponent<Renderer>().material.color = highlightColour;
            }
            else
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].material.HasProperty(highlightParam))
                        renderers[i].material.color = highlightColour;
                }
            }
        }

        public void ResetHighlight()
        {
            if (!highlight)
                return;
            if (GetComponent<Renderer>() != null)
            {
                if (GetComponent<Renderer>().material.HasProperty(highlightParam))
                    GetComponent<Renderer>().material.color = initialColor;
            }
            else
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].material.HasProperty(highlightParam))
                        renderers[i].material.color = initialColors[i];
                }
            }
        }

        public int ID
        {
            set { id = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ResourceNode"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active
        {
            get { return active; }
            set
            {
                //Debug.LogError("ResouceNode id:"+id+" profileId:"+profileId+" settingId:"+settingId+" active:"+active);

                if (value != active)
                {
                    //Debug.LogError("ResourceNode: harvestCoordEffect="+ harvestCoordEffect+" activateCoordEffect = " + activateCoordEffect + " deactivateCoordEffect=" + deactivateCoordEffect);
                    if (value && activateCoordEffect != null)
                    {
                        Dictionary<string, object> props = new Dictionary<string, object>();
                        props.Add("gameObject", gameObject);
                        CoordinatedEffectSystem.ExecuteCoordinatedEffect(activateCoordEffect, props);
                    }
                    else if (!value && deactivateCoordEffect != null)
                    {
                        Dictionary<string, object> props = new Dictionary<string, object>();
                        props.Add("gameObject", gameObject);
                        CoordinatedEffectSystem.ExecuteCoordinatedEffect(deactivateCoordEffect, props);
                    }

                    if (GetComponent<bl_MiniMapItem>() != null)
                        if (active == false)
                        {
                            GetComponent<bl_MiniMapItem>().HideItem();
                        }
                        else
                        {
                            GetComponent<bl_MiniMapItem>().ShowItem();
                        }

                }


                if (active && value != active)
                {
                    active = value;

                    StartCoroutine(Deactivate());
                }
                else
                {
                    active = value;
                    if (isLODChild)
                    {
                        transform.parent.gameObject.SetActive(active);
                    }
                    else
                    {
                        int i = 0;
                        if (subProfileGameObjects.Count > 0)
                        {
                            foreach (var go in subProfileGameObjects)
                            {
                              //  Debug.LogError("ResourceNode go:"+go+" i:"+i+" settingId:"+settingId+" id:"+id+" active:"+active,gameObject);
                                if (go == null)
                                    continue;
                                if (i == settingId)
                                {
                                   // Debug.LogError("ResourceNode go:"+go+" i:"+i+" settingId:"+settingId+" id:"+id+" set active",gameObject);

                                    go.SetActive(active);
                                }
                                else
                                {
                                  //  Debug.LogError("ResourceNode go:"+go+" i:"+i+" settingId:"+settingId+" id:"+id+" set deactive",gameObject);
                                    if (go.activeSelf)
                                        go.SetActive(false);
                                }

                                i++;
                            }
                        }else
                          gameObject.SetActive(active);
                    }

                    GetComponent<AtavismNode>().AddLocalProperty("active", active);
                    if (GetComponent<MeshRenderer>() != null)
                    {
                        GetComponent<MeshRenderer>().enabled = active;
                        GetComponent<Collider>().enabled = active;
                    }
                    if (subProfileGameObjects.Count == 0)
                    foreach (Transform child in GetComponent<Transform>())
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            child.GetComponent<MeshRenderer>().enabled = active;
                        }

                        if (child.GetComponent<Collider>() != null)
                        {
                            child.GetComponent<Collider>().enabled = active;
                        }

                        child.gameObject.SetActive(active);
                    }

                }
            }
        }

        IEnumerator Deactivate()
        {
            yield return new WaitForSeconds(deactivateDelay);
            GetComponent<AtavismNode>().AddLocalProperty("active", active);

            if (GetComponent<MeshRenderer>() != null)
            {
                GetComponent<MeshRenderer>().enabled = active;
                GetComponent<Collider>().enabled = active;
            }
            if (subProfileGameObjects.Count == 0)
            foreach (Transform child in GetComponent<Transform>())
            {
                if (child.GetComponent<MeshRenderer>() != null)
                {
                    child.GetComponent<MeshRenderer>().enabled = active;
                }

                if (child.GetComponent<Collider>() != null)
                {
                    child.GetComponent<Collider>().enabled = active;
                }

                child.gameObject.SetActive(active);
            }

            if (isLODChild)
            {
                transform.parent.gameObject.SetActive(active);
            }
            else
            {
                int i = 0;
                if (subProfileGameObjects.Count > 0)
                {
                    foreach (var go in subProfileGameObjects)
                    {
                        if (i == settingId)
                        {
                            go.SetActive(active);
                        }
                        else
                        {
                            if (go.activeSelf)
                                go.SetActive(false);
                        }

                        i++;
                    }
                }else
                    gameObject.SetActive(active);
            }
            mouseOver = false;
            AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
           
        }

        public int SettingId
        {
            get
            {
                return settingId;
            }
            set
            {
                settingId = value;
               
               // Debug.LogError("ResouceNode id:"+id+" profileId:"+profileId+" settingId:"+settingId);
                    var rnd = AtavismPrefabManager.Instance.LoadResourceNodeData(profileId, settingId);
                    if (rnd != null)
                    {
                        selectedIcon = rnd.selectedIcon;
                        cursorIcon = rnd.cursorIcon;
                        setIcons = true;
                    }
                
            }
        }
    }
}