using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class InteractiveObject : MonoBehaviour
    {

        public int id = -1;
        public string interactionType;
        public int interactionID = -1;
        public string interactionData1;
        public string interactionData2;
        public string interactionData3;
        public int questReqID = -1;
        public float interactTimeReq = 0;
        public int refreshDuration = 0;
        public Texture2D cursorIcon;
        public bool highlight = true;
        public Color highlightColour = Color.cyan;
        float cooldown = 0.5f;
        float cooldownEnds;
        public int minLevel = 1;
        public int maxLevel = 99;

        public GameObject interactCoordEffect;
        public List<GameObject> activateCoordEffects;
        public GameObject deactivateCoordEffect;

        public bool isLODChild = false;

        string currentState = "";
        Color initialColor;
        bool active = false;
        bool selected = false;
        Renderer[] renderers;
        Color[] initialColors;
        bool mouseOver = false;
        float timeClick = 0f;

        // Use this for initialization
        void Start()
        {
            cooldownEnds = Time.time;
            gameObject.AddComponent<AtavismNode>();
            GetComponent<AtavismNode>().AddLocalProperty("targetable", false);
            GetComponent<AtavismNode>().AddLocalProperty("active", active);

            if (highlight)
                if (GetComponent<Renderer>() != null)
                {
                    initialColor = GetComponent<Renderer>().material.color;
                }
                else
                {
                    renderers = GetComponentsInChildren<Renderer>();
                    initialColors = new Color[renderers.Length];
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        if (renderers[i].material.HasProperty("_Color"))
                            initialColors[i] = renderers[i].material.color;
                    }
                }

            // Add child component to all children with colliders
            foreach (Collider child in GetComponentsInChildren<Collider>())
            {
                if (child.gameObject != gameObject)
                    child.gameObject.AddComponent<ObjectChildMouseDetector>();
            }

            InteractiveObjectsManager.Instance.RegisterInteractiveObject(this);
        }

        void OnDestroy()
        {
            if (ClientAPI.ScriptObject != null)
                InteractiveObjectsManager.Instance.RemoveInteractiveObject(id);

            AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
        }

        void OnDisable()
        {
            AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
        }

        // Update is called once per frame
        void Update()
        {
            if (mouseOver)
            {
                if (AtavismCursor.Instance.IsMouseOverUI())
                {
                    ResetHighlight();
                    AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
                }
                else
                {
                    Highlight();
                    AtavismCursor.Instance.SetMouseOverObject(GetComponent<AtavismNode>(), cursorIcon, 4);
                }
                if (Input.GetMouseButtonDown(1) && !AtavismCursor.Instance.IsMouseOverUI())
                {
                    timeClick = Time.time + 0.5f;
                }
                if (/*Input.GetMouseButtonDown(1)*/Input.GetMouseButtonUp(1) && !AtavismCursor.Instance.IsMouseOverUI() && timeClick > Time.time)
                {
                    StartInteraction();
                }
            }
        }

        void OnMouseDown()
        {
            if (!AtavismCursor.Instance.IsMouseOverUI())
                StartInteraction();
        }

        void OnMouseOver()
        {
            if (active)
            {
                AtavismCursor.Instance.SetMouseOverObject(GetComponent<AtavismNode>(), cursorIcon, 4);
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

        public void StartInteraction()
        {
            int level = 1;
            if (ClientAPI.GetPlayerObject().PropertyExists("level"))
                level = (int)ClientAPI.GetPlayerObject().GetProperty("level");
            if (interactionType.Equals("InstancePortal") && level > maxLevel)
            {
                string[] args = new string[1];
#if AT_I2LOC_PRESET
            args[0] = I2.Loc.LocalizationManager.GetTranslation("You cannot permit enter to instance becouse you have level to heigh.");
#else
                args[0] = "You cannot permit enter to instance becouse you have level to heigh.";
#endif
                AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
            }
            else if (interactionType.Equals("InstancePortal") && level < minLevel)
            {
                string[] args = new string[1];
#if AT_I2LOC_PRESET
            args[0] = I2.Loc.LocalizationManager.GetTranslation("You cannot permit enter to instance becouse you have level to low.");
#else
                args[0] = "You cannot permit enter to instance becouse you have level to low.";
#endif
                AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
            }
            else if (Time.time < cooldownEnds)
            {
                // Send error message
                string[] args = new string[1];
#if AT_I2LOC_PRESET
            args[0] = I2.Loc.LocalizationManager.GetTranslation("You cannot perform that action yet.");
#else
                args[0] = "You cannot perform that action yet.";
#endif
                AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
            }
            else
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("objectID", id);
                props.Add("state", MoveToNextState());
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.INTERACT_WITH_OBJECT", props);
                cooldownEnds = Time.time + cooldown;
            }
        }

        string MoveToNextState()
        {
            int nextPos = 0;
            foreach (GameObject coordEffect in activateCoordEffects)
            {
                nextPos++;
                if (coordEffect != null)
                    if (coordEffect.name == currentState || currentState == "")
                    {
                        if (nextPos == activateCoordEffects.Count)
                        {
                            nextPos = 0;
                        }
                        return activateCoordEffects[nextPos].name;
                    }
            }
            return "";
        }

        public void StateUpdated(string state)
        {
            if (state == null || state == "" || state == "null" || state == currentState)
                return;
            currentState = state;
            Dictionary<string, object> props = new Dictionary<string, object>();
            props["gameObject"] = gameObject;
            CoordinatedEffectSystem.ExecuteCoordinatedEffect(currentState, props);
        }

        public void Highlight()
        {
            if (!highlight)
                return;
            if (GetComponent<Renderer>() != null)
            {
                GetComponent<Renderer>().material.color = highlightColour;
            }
            else
            {
                for (int i = 0; i < renderers.Length; i++)
                {
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
                GetComponent<Renderer>().material.color = initialColor;
            }
            else
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.color = initialColors[i];
                }
            }
        }

        public int ID
        {
            set
            {
                id = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ResourceNode"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                if (active == value)
                    return;
                active = value;
                if (!value)
                    OnMouseExit();

                GetComponent<AtavismNode>().AddLocalProperty("active", active);
                if (GetComponent<MeshRenderer>() != null)
                {
                    GetComponent<MeshRenderer>().enabled = active;
                }
                if (GetComponent<Collider>() != null)
                    GetComponent<Collider>().enabled = active;
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
}