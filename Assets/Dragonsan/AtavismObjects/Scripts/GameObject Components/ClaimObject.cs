using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Extensions;
using UnityEngine.EventSystems;

namespace Atavism
{
    public enum BuildObjectPlacementType
    {
        Terrain,
        Floor,
        TerrainOrFloor,
        Wall,
        Ceiling
    }
    public delegate void ClaimObjectProgressUpdate();

    public class ClaimObject : MonoBehaviour
    {

        int id;
        int templateID;
        string templateName;
        [HideInInspector]
          public string prefab="";
        [HideInInspector]
        public string status="";
        int claimID;
        [HideInInspector]
        public BuildObjectPlacementType placementType;
        [HideInInspector]
        public float horizontalSnapDistance = 0f;
        [HideInInspector]
        public float verticalSnapDistance = 0f;
        [HideInInspector]
        public bool autoRotateToHitNormal = false;
        [HideInInspector]
        public bool canHaveParent = true;
        [HideInInspector]
        public bool requiresParent = false;
        [HideInInspector]
        public bool allowChildren = true;
        [HideInInspector]
        public bool canBeMoved = true;
        [HideInInspector]
        public List<Collider> blockingCollisionVolumes;
        [HideInInspector]
        public List<Renderer> placementMeshes;
        public Texture2D cursorIcon;
        public Color selectedColor = Color.cyan;
        public List<GameObject> coordEffects;
        public bool interactable = false;
        public float interactionDistance = 5;
        string currentState = "";
        [HideInInspector]
        public float currentTime;
        [HideInInspector]
        public float totalTime;
        [HideInInspector]
        public float timeSpeed;
        [HideInInspector]
        public float lastBuildTimeUpdate;
        int health;
        int maxHealth;
        bool complete;
        private bool finalStage;
        private bool attackable = false;
        private bool repairable = false;
        private bool solo = false;
        Dictionary<int, int> itemReqs = new Dictionary<int, int>();
        Color initialColor;
        bool active = false;
        Dictionary<Renderer, List<Color>> initialColours = new Dictionary<Renderer, List<Color>>();
        bool mouseOver = false;
        [HideInInspector]
        public GameObject damageGameObject;
        [HideInInspector]
        public string damage = "";
        [HideInInspector]
        public AtavismProgressBar progressBar;
        [HideInInspector]
        public AtavismHealthBar healthBar;
        public Vector3 positionProgressBar = Vector3.up; 
        public Vector3 positionHealthBar = Vector3.up; 
        ClaimObjectProgressUpdate claimObjectProgressUpdate;
        public GameObject destroySpawnPrefab;
        public float prefabDestroyDelay=5f;
        bool objectPlaced = false;

        public ClaimObjectProgressUpdate setProgressUpdate
        {
            set
            {
                claimObjectProgressUpdate = value;
            }
        }
        private void OnDrawGizmosSelected()
        {
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + positionProgressBar,0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + positionHealthBar,0.1f);

        }
        public void UpdateProgress()
        {
            if(claimObjectProgressUpdate != null)
                 claimObjectProgressUpdate();
        }
        // Use this for initialization
        void Start()
        {
            // Add child component to all children with colliders
            foreach (Collider child in GetComponentsInChildren<Collider>())
            {
                if (child.gameObject != gameObject)
                    child.gameObject.AddComponent<ClaimObjectChild>();
            }
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                List<Color> colours = new List<Color>();
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        colours.Add(mat.color);
                    }
                }
                initialColours[r] = colours;
            }

            gameObject.AddComponent<AtavismNode>();
        }

       

        void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if(mouseOver)
                    OnMouseExit();
                return;
            }

            mouseOver = true;
            AtavismCursor.Instance.SetMouseOverObject(GetComponent<AtavismNode>(), cursorIcon, 4);
            
            if (active)
            {
                if(WorldBuilder.Instance.BuildingState == WorldBuildingState.EditItem)
                //if(BuilderBehaviour.Instance.CurrentMode == BuildMode.Edition)
                    Highlight();
            }
        }

        void OnMouseExit()
        {
            AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
            if (WorldBuilder.Instance.SelectedObject != this)
                ResetHighlight();
            mouseOver = false;
        }

        void OnMouseDown()
        {
            WorldBuilder.Instance.SelectedClaimObject = this;
         //   if (  EventSystem.current.IsPointerOverGameObject())
         //       Debug.LogError("OnMouseDown");
            if (AtavismCursor.Instance.IsMouseOverUI() ||  EventSystem.current.IsPointerOverGameObject())
                return;




            if (WorldBuilder.Instance.BuildingState == WorldBuildingState.CreateObject || WorldBuilder.Instance.BuildingState == WorldBuildingState.PlaceItem)
            {
                return;
            }

            if (WorldBuilder.Instance.ActiveClaim == null)
            {
                return;
            }

            sendNextState(coordEffects, gameObject);
        }

        public void sendNextState(List<GameObject> coordinatedEffects, GameObject target)
        {
            
       //     Debug.LogError(" Attackable = "+attackable+" reparable="+repairable+" id="+id+" claimId="+claimID);
       //     Debug.LogError(" Attackable = "+attackable+" reparable="+repairable+" stance="+(claimID>0 ?WorldBuilder.Instance.GetClaim(claimID)!=null?WorldBuilder.Instance.GetClaim(claimID).stance+"":"NA":"NA"));
            WorldBuilder.Instance.SelectedClaimObject = this;
            // Check user has permission to interact with the object
            if (WorldBuilder.Instance.GetClaim(claimID).permissionlevel < 1 && !WorldBuilder.Instance.GetClaim(claimID).playerOwned)
            {
                return;
            }
            // Check player is close enough to interact with the object
            if (Vector3.Distance(ClientAPI.GetPlayerObject().Position, target.transform.position) > interactionDistance)
            {
                // Send error message
                string[] args = new string[1];
#if AT_I2LOC_PRESET
            args[0] = I2.Loc.LocalizationManager.GetTranslation("You are too far away from the object");
#else
                args[0] = "You are too far away from the object";
#endif
                AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                return;
            }
            if (WorldBuilder.Instance.BuildingState == WorldBuildingState.SelectItem)
            {
                // Don't run any interaction code while in select mode
                return;
            }

            if (WorldBuilder.Instance.BuildingState == WorldBuildingState.None || (totalTime>0 && WorldBuilder.Instance.GetBuildObjectTemplate(templateID).buildTaskReqPlayer))
            {
                if (coordinatedEffects.Count == 0 && interactable)
                {
                    Dictionary<string, object> props = new Dictionary<string, object>();
                    props.Add("action", "use");
                    props.Add("claimID", claimID);
                    props.Add("objectID", id);
                    NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM_OBJECT", props);
                }
            }

            int nextPos = 0;
            foreach (GameObject coordEffect in coordinatedEffects)
            {
                nextPos++;
                if (coordEffect.name == currentState || currentState == "")
                {
                    if (nextPos == coordinatedEffects.Count)
                    {
                        nextPos = 0;
                    }
                    //currentState = coordEffects[nextPos].name;
                    Dictionary<string, object> props = new Dictionary<string, object>();
                    props.Add("action", "state");
                    props.Add("claimID", claimID);
                    props.Add("objectID", id);
                    props.Add("state", coordinatedEffects[nextPos].name);
                    NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM_OBJECT", props);
                    return;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            
            if (mouseOver && (/*Input.GetMouseButtonDown(1) ||*/ (WorldBuilder.Instance.BuildingState == WorldBuildingState.SelectItem && Input.GetMouseButtonDown(0))) && UGUIWorldBuilder.Instance.Showing)
            {
                if (BuilderBehaviour.Instance.CurrentMode == BuildMode.None)
                {
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
                    BuilderBehaviour.Instance.EditPrefab();
                }
                //    Debug.LogError("SelectedObject id="+id);
                WorldBuilder.Instance.SelectedObject = this;
                
             /*   WorldBuilder.Instance.SelectedObject = this;
                if (AtavismCursor.Instance.CursorHasItem())
                {
                    WorldBuilder.Instance.ImproveBuildObject(gameObject,
                            AtavismCursor.Instance.GetCursorItem(), 1);
                }*/
            }
            if (WorldBuilder.Instance.SelectedObject != null)
            {
                if (WorldBuilder.Instance.SelectedObject.ObjectPlaced == true)
                {
                    UGUIWorldBuilder.Instance.SaveObjectChanges();
                }
            }
            if (UGUIWorldBuilder.Instance.Showing == true)
            {
             //   Debug.LogError("I'ma showin!");
            }

        }

        public void StateUpdated(string state)
        {
            if (state == null || state == "null" || state == currentState)
                return;
            currentState = state;
            Dictionary<string, object> props = new Dictionary<string, object>();
            if (GetComponentInChildren<ClaimInteractiveObjectChild>() != null)
            {
                props["gameObject"] = GetComponentInChildren<ClaimInteractiveObjectChild>().gameObject;
                CoordinatedEffectSystem.ExecuteCoordinatedEffect(currentState, props);
            }
            else
            {
                props["gameObject"] = gameObject;
                CoordinatedEffectSystem.ExecuteCoordinatedEffect(currentState, props);
            }
        }

        public void Highlight()
        {
            foreach (Renderer renderer in initialColours.Keys)
            {
                foreach (Material mat in renderer.materials)
                {
                  //  mat.color = Color.cyan;
                }
            }
        }

        public void HighlightError()
        {
            foreach (Renderer renderer in initialColours.Keys)
            {
                foreach (Material mat in renderer.materials)
                {
                 //   mat.color = Color.red;
                }
            }
        }

        public void ResetHighlight()
        {
          /*  foreach (Renderer renderer in initialColours.Keys)
            {
                int pos = 0;
                if(renderer)
                foreach (Material mat in renderer.materials)
                {
                    if (mat.HasProperty("_Color") && initialColours[renderer].Count > pos)
                    {
                        mat.color = initialColours[renderer][pos];
                        pos++;
                    }
                }
            }*/
        }

        public int ID
        {
            set
            {
                id = value;
            }
            get
            {
                return id;
            }
        }
        
        public bool ObjectPlaced
        {
            set
            {
                objectPlaced = value;
            }
            get
            {
                return objectPlaced;
            }

        }

        public int TemplateID
        {
            set
            {
                templateID = value;
            }
            get
            {
                return templateID;
            }
        }

        public string TemplateName
        {
            get
            {
                return WorldBuilder.Instance.GetBuildObjectTemplate(templateID).buildObjectName;
            }
        }
        public string Status
        {
            set
            {
                            switch (value)
                            {
                                case "REPAIR":
                                    status = "REPAIRING";
                                    break;
                                case "BUILD":
                                    status = "BUILDING";
                                    break;
                                default:
                                status="";
                                break;
                                 
                            }
               
            }
            get
            {
                return status;
            }
        }
        
        public int ClaimID
        {
            set
            {
                claimID = value;
            }
            get
            {
                return claimID;
            }
        }

        public bool Active
        {
            set
            {
                active = value;
                if (!active && WorldBuilder.Instance.SelectedObject != this)
                {
                    if (GetComponent<Renderer>() != null)
                        ResetHighlight();
                }
            }
            get
            {
                return active;
            }
        }

        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;
            }
        }

        public int MaxHealth
        {
            get
            {
                return maxHealth;
            }
            set
            {
                maxHealth = value;
            }
        }

        public bool Complete
        {
            get
            {
                return complete;
            }
            set
            {
                complete = value;
            }
        }
        public bool FinalStage
        {
            get
            {
                return finalStage;
            }
            set
            {
                finalStage = value;
            }
        }
        public bool Attackable
        {
            get
            {
                return attackable;
            }
            set
            {
                attackable = value;
            }
        }

        public bool Repairable
        {
            get
            {
                return repairable;
            }
            set
            {
                repairable = value;
            }
        }
        
        public bool Solo
        {
            get
            {
                return solo;
            }
            set
            {
                solo = value;
            }
        }

        public Dictionary<int, int> ItemReqs
        {
            set
            {
                itemReqs = value;
            }
            get
            {
                return itemReqs;
            }
        }

        public void Destroy()
        {
            AtavismLogger.LogDebugMessage("Destroy Claim Object "+id);
            if (destroySpawnPrefab)
            {
                GameObject go = Instantiate(destroySpawnPrefab, transform.position, transform.rotation);
                
                Destroy(go , prefabDestroyDelay);
            }
            var pb = GetComponent<PieceBehaviour>();
            if (pb != null)
            {
                Rigidbody Rigidbody = gameObject.AddRigibody(false, false);
                for (int i = 0; i < pb.Colliders.Count; i++)
                {
                    if (pb.Colliders[i].GetComponent<MeshCollider>() != null)
                    {
                        pb.Colliders[i].GetComponent<MeshCollider>().convex = true;
                    }
                }

                Rigidbody.useGravity = true;
                Rigidbody.isKinematic = false;
                Rigidbody.AddForce(UnityEngine.Random.insideUnitSphere, ForceMode.Impulse);
            }
            
            
        }
    }
}