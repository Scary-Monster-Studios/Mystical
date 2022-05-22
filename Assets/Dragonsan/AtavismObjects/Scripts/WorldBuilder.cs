using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using EasyBuildSystem.Features.Scripts.Extensions;
using UnityEngine.SceneManagement;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
namespace Atavism
{

/*   [System.Flags]
    public enum ClaimType
    {
        Any = (1 << 0),
        Residential = (1 << 1),
        Farm = (1 << 2),
    }*/

  public class ClaimPermission
  {
      public OID playerOid;
      public string playerName;
      public int permissionLevel;
  }
  
  public struct OptionChoice
  {
      public string name;
      public int id;
  }

    public class Claim
    {
        public int id;
        //public ClaimType claimType;
        public int claimType;
        public int stance = 0;
        public string name = "";
        public string ownerName = "";
        public Vector3 loc;
        public Vector3 diffloc;
        public int sizeX = 30;
        public int sizeY = 30;
        public int sizeZ = 30;
        public Bounds bounds;
        public bool playerOwned;
        public bool forSale;
        public long cost;
        public int currency = 1;
        public int purchaseItemReq = -1;
        public int permissionlevel = 0;
        public float taxTime = 0f;
        public  long taxAmount = -1L;
        public long taxInterval = -1L;
        public long taxPeriodPay = -1L;
        public  int taxCurrency = -1;
        public Dictionary<int, int> resources = new Dictionary<int, int>();
        public Dictionary<int, int> limits = new Dictionary<int, int>();
        public Dictionary<int, int> limitsCount = new Dictionary<int, int>();
        public Dictionary<int, ClaimObject> claimObjects = new Dictionary<int, ClaimObject>();
        public List<ClaimPermission> permissions = new List<ClaimPermission>();
        

        public void GenerateBounds()
        {
            bounds = new Bounds(loc+diffloc, new Vector3(sizeX, sizeY, sizeZ));
        }

        public bool IsObjectFullyInsideClaim(Bounds b)
        {
            //Debug.Log("Bounds: " + bounds.ToString() + " against b: " + b.ToString());
            if (!bounds.Contains(b.min))
            {
                return false;
            }
            if (!bounds.Contains(b.max))
            {
                return false;
            }
            return true;
        }
    }

    public struct ClaimListEntry
    {
        public int id;
        public float time;
        public string name;
        public long taxAmount ;
        public long taxInterval;
        public long taxPeriodPay;
        public int taxCurrency;

    }
    
    public enum WorldBuildingState
    {
        PlaceItem,
        SelectItem,
        EditItem,
        MoveItem,
        SellClaim,
        PurchaseClaim,
        CreateClaim,
        CreateObject,
        Standard,
        Admin,
        None
    }

    public struct ClaimObjectData
    {
        public int objectID;
        public int templateID;
        public int claimID;
        public string prefabName;
        public string damage;
        public Vector3 loc;
        public Quaternion orient;
        public string state;
        public int health;
        public int maxHealth;
        public bool complete;
        public bool finalStage;
        public float currentBuildTime;
        public float totalBuildTime;
        public float lastBuildTimeUpdate;
        public double timeSpeed;
        public bool buildingRuning;
        public bool attackable;
        public bool repairable;
        public bool solo;
        public string status;
        public string interactionType;
        public int lockTemplateId;
        public int lockDurability;

    }

    public class WorldBuilder : MonoBehaviour
    {

        static WorldBuilder instance;

        public float deselectDistance = 6f;

        Dictionary<int, AtavismBuildObjectTemplate> buildObjectTemplates = new Dictionary<int, AtavismBuildObjectTemplate>();
        WorldBuildingState buildingState = WorldBuildingState.None;
        private List<Claim> claims = new List<Claim>();
        private Claim activeClaim = null;
        bool showClaims = false;
        Dictionary<string,GameObject> claimGameObjects = new Dictionary<string,GameObject>();
        Dictionary<int, int> buildingResources = new Dictionary<int, int>();
        List<AtavismInventoryItem> itemsPlacedToUpgrade = new List<AtavismInventoryItem>();
        ClaimObject selectedObject;
        int selectedID = -1;

        List<ClaimObjectData> objectsToLoad = new List<ClaimObjectData>();
        int frameCount = 0;
        public bool showInConstructMaterialsFromBackpack = false;
        public bool itemsForUpgradeMustBeInserted = true;

        public float claimObjectDestroyDelay = 0f;
        public float claimObjectDestroyDependentDelay = 5f;

        private List<ClaimListEntry> playerClaims = new List<ClaimListEntry>();

        public AtavismProgressBar objectTimerPrefab;
        public AtavismHealthBar objectHealthPrefab;

        private ClaimObject selectedClaimObject;
        Dictionary<int,OptionChoice> claimTypes = new Dictionary<int,OptionChoice>();        
        Dictionary<int,OptionChoice> buildingCategory = new Dictionary<int,OptionChoice>();        
        // Use this for initialization
        void Start()
        {
           
            if (instance != null)
            {
                return;
            }
            instance = this;
            AtavismEventSystem.RegisterEvent("CLAIM_TARGET_CLEAR", this);
            AtavismEventSystem.RegisterEvent("TARGET_CLEAR", this);
            AtavismEventSystem.RegisterEvent("CLAIM_ADDED", this);
            AtavismEventSystem.RegisterEvent("CLAIMED_REMOVED", this);
            AtavismEventSystem.RegisterEvent("LOGGED_OUT", this);

            // Register for messages relating to the claim system 
            NetworkAPI.RegisterExtensionMessageHandler("claim_own", ClaimOwnMessage);
            NetworkAPI.RegisterExtensionMessageHandler("claim_data", ClaimIDMessage);
            NetworkAPI.RegisterExtensionMessageHandler("remove_claim_data", ClaimRemoveDataMessage);
            NetworkAPI.RegisterExtensionMessageHandler("claim_updated", ClaimUpdatedMessage);
            NetworkAPI.RegisterExtensionMessageHandler("remove_claim", RemoveClaimMessage);
            NetworkAPI.RegisterExtensionMessageHandler("claim_deleted", RemoveClaimMessage);
            NetworkAPI.RegisterExtensionMessageHandler("claim_made", ClaimMadeMessage);
            NetworkAPI.RegisterExtensionMessageHandler("claim_upgrade", ClaimUpgradeMessage);
            NetworkAPI.RegisterExtensionMessageHandler("claim_tax_pay", ClaimTaxPaymentMessage);

            NetworkAPI.RegisterExtensionMessageHandler("claim_object", ClaimObjectMessage);
            NetworkAPI.RegisterExtensionMessageHandler("claim_object_bulk", ClaimObjectBulkMessage);
            NetworkAPI.RegisterExtensionMessageHandler("move_claim_object", MoveClaimObjectMessage);
            NetworkAPI.RegisterExtensionMessageHandler("update_claim_object_state", UpdateClaimObjectStateMessage);
            NetworkAPI.RegisterExtensionMessageHandler("claim_object_info", ClaimObjectInfoMessage);
            NetworkAPI.RegisterExtensionMessageHandler("remove_claim_object", RemoveClaimObjectMessage);
            NetworkAPI.RegisterExtensionMessageHandler("buildingResources", HandleBuildingResources);
            NetworkAPI.RegisterExtensionMessageHandler("start_build_object", HandleStartBuildObject);
            NetworkAPI.RegisterExtensionMessageHandler("start_build_task", HandleStartBuildTask);
            NetworkAPI.RegisterExtensionMessageHandler("build_task_interrupted", HandleInterruptBuildTask);

            NetworkAPI.RegisterExtensionMessageHandler("BuildObjPrefabData", HandleBuildObjectPrefabData);
            NetworkAPI.RegisterExtensionMessageHandler("BuildObjIcon", HandleBuilObjIcon);
            NetworkAPI.RegisterExtensionMessageHandler("remove_claim_object_confirm", RemoveClaimObjectConfirmMessage);
            NetworkAPI.RegisterExtensionMessageHandler("move_claim_object_confirm", MoveClaimObjectConfirmMessage);

            NetworkAPI.RegisterExtensionMessageHandler("builder_settings", BuilderSettingsMessage);

        }

     

        void ClientReady()
        {
            AtavismLogger.LogDebugMessage("WorldBuilder ClientReady");
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("BuildingObject", HandleBuildObjectPrefabData);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("BuildingObjectIcon", HandleBuilObjIcon);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
            void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CLAIM_TARGET_CLEAR", this);
            AtavismEventSystem.UnregisterEvent("TARGET_CLEAR", this);
            AtavismEventSystem.UnregisterEvent("CLAIM_ADDED", this);
            AtavismEventSystem.UnregisterEvent("CLAIMED_REMOVED", this);
            AtavismEventSystem.UnregisterEvent("LOGGED_OUT", this);
        }

            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (scene.name == "Login" || scene.name == ClientAPI.Instance.characterSceneName)
                {
                    foreach (Claim claim in claims)
                    {
                        foreach (ClaimObject cObject in claim.claimObjects.Values)
                        {
                            DestroyImmediate(cObject.gameObject);
                        }
                    }
                    claims.Clear();
                }

            }

            // Update is called once per frame
        void Update()
        {
            if (activeClaim == null /*&& buildingState != WorldBuildingState.None*/)
            {
                // Check against claims to see if a region has been entered
                foreach (Claim claim in claims)
                {
                    //   Debug.LogWarning("WorldBuilder "+claim.id+" "+claim.name+ " "+claim.permissionlevel+" "+claim.permissions+" | scena="+UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                    if (ClientAPI.GetPlayerObject() != null && InsideClaimArea(claim, ClientAPI.GetPlayerObject().Position))
                    {
                        //     Debug.LogWarning("WorldBuilder Selected ClaimId="+claim.id+" "+claim.name );
                        Claim c = activeClaim;
                        activeClaim = claim;
                        // dispatch a ui event to tell the rest of the system
                        string[] args = new string[1];
                        AtavismEventSystem.DispatchEvent("CLAIM_CHANGED", args);
                       /* if (activeClaim == null && WorldBuilder.Instance.ShowClaims)
                        {

                            WorldBuilder.Instance.BuildingState = WorldBuildingState.SelectItem;
                            BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
                        }*/

                        break;
                    }
                }
            }
            else /*if (buildingState != WorldBuildingState.None) */
            {
                // Check if the player has left the claim
                if (ClientAPI.GetPlayerObject() != null && !InsideClaimArea(activeClaim, ClientAPI.GetPlayerObject().Position))
                {
                    activeClaim = null;
                    WorldBuilder.Instance.BuildingState = WorldBuildingState.None;
                    if(BuilderBehaviour.Instance!=null)
                        BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                    // dispatch a ui event to tell the rest of the system
                    string[] args = new string[1];
                    AtavismEventSystem.DispatchEvent("CLAIM_CHANGED", args);
                }
            }

            // Check distance for selected object - if too far away, unselect it
            if (selectedObject != null && buildingState != WorldBuildingState.MoveItem)
            {
                if (ClientAPI.GetPlayerObject() != null)
                {
                    if (Vector3.Distance(ClientAPI.GetPlayerObject().Position, selectedObject.transform.position) > deselectDistance)
                    {
                        SelectedObject = null;
                        BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                        BuildingState  = WorldBuildingState.SelectItem;
                    }
                    else
                    {
                        string[] args = new string[1];
                        AtavismEventSystem.DispatchEvent("CLAIM_OBJECT_DESELECT", args);
                      //  ClientAPI.Write("Selected object is too far away");
                    }
                }
                else
                {
                    SelectedObject = null;
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                    BuildingState  = WorldBuildingState.SelectItem;
                }
            }

            if (SceneManager.GetActiveScene().name == AtavismClient.instance.LoadLevelName)
            {
                if (frameCount == 1 && objectsToLoad.Count > 0)
                {
                    SpawnClaimObject(objectsToLoad[0]);
                    objectsToLoad.RemoveAt(0);
                }

                frameCount++;
                if (frameCount > 1)
                    frameCount = 0;
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CLAIM_ADDED")
            {
                GameObject claim = GameObject.Find(eData.eventArgs[0]);
                if (claim != null)
                {
                    if (claimGameObjects.ContainsKey(eData.eventArgs[0]))
                        if (claimGameObjects[eData.eventArgs[0]] == null)
                            claimGameObjects.Remove(eData.eventArgs[0]);
                    if (!claimGameObjects.ContainsKey(eData.eventArgs[0]))
                        claimGameObjects.Add(eData.eventArgs[0], claim);
                    claim.SetActive(showClaims);
                }
            }
            else if (eData.eventType == "CLAIM_REMOVED")
            {
                //GameObject claim = GameObject.Find(eData.eventArgs[0]);
                if (claimGameObjects.ContainsKey(eData.eventArgs[0]))
                    claimGameObjects.Remove(eData.eventArgs[0]);
            }
            else if (eData.eventType == "LOGGED_OUT")
            {
                foreach (Claim claim in claims)
                {
                    if (claim != null)
                        foreach (ClaimObject cObject in claim.claimObjects.Values)
                        {
                            if (cObject != null)
                                Destroy(cObject.gameObject);
                        }
                }

                claims.Clear();
            }else if (eData.eventType == "TARGET_CLEAR"|| eData.eventType == "CLAIM_TARGET_CLEAR")
            {
                selectedClaimObject = null;
            }
        }

        public bool InsideClaimArea(Claim claim, Vector3 point)
        {
            if (InRange(point.x, claim.loc.x + claim.diffloc.x - claim.sizeX / 2, claim.loc.x + claim.diffloc.x + claim.sizeX / 2) &&
                InRange(point.z, claim.loc.z + claim.diffloc.z - claim.sizeZ / 2, claim.loc.z + claim.diffloc.z + claim.sizeZ / 2))
            {
                return true;
            }

            return false;
        }

        bool InRange(float val, float min, float max)
        {
            return ((val >= min) && (val <= max));
        }

        public Claim GetClaim(int claimID)
        {
            foreach (Claim claim in claims)
            {
                if (claim.id == claimID)
                    return claim;
            }
            return null;
        }


        #region Message Senders
        
        public void CreateClaim(string name, int size, bool playerOwned, bool forSale, int currencyID, long cost , int ct, string ctn, int taxCurrency, long taxAmount, long taxInterval, long taxTimePay, long taxTimeSell)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("loc", ClientAPI.GetPlayerObject().Position);
            props.Add("name", name);
            props.Add("size", size);
            props.Add("owned", playerOwned);
            props.Add("forSale", forSale);
            props.Add("cost", cost);
            props.Add("claimType", ct);
            props.Add("currency", currencyID);
            props.Add("claimTypeName", ctn);
            props.Add("taxCurrency", taxCurrency);
            props.Add("taxAmount", taxAmount);
            props.Add("taxInterval", taxInterval);
            props.Add("taxTimePay", taxTimePay);
            props.Add("taxTimeSell", taxTimeSell);
            props.Add("c", true);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.CREATE_CLAIM", props);
        }

        
        
        
        public void SendGetUpgradeClaim()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("claimID", activeClaim.id);
            props.Add("action", "get");
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.UPGRADE_CLAIM", props);
        }

        public void SendUpgradeClaim()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("claimID", activeClaim.id);
            props.Add("action", "upgrade");
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.UPGRADE_CLAIM", props);
        }

        public void AddPermission(string playerName, int permissionLevel)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("claimID", activeClaim.id);
            props.Add("playerName", playerName);
            props.Add("action", "Add");
            props.Add("permissionLevel", permissionLevel);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.CLAIM_PERMISSION", props);
            //ClientAPI.Write("Sent add permission message");
        }

        public void RemovePermission(string playerName)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("claimID", activeClaim.id);
            props.Add("playerName", playerName);
            props.Add("action", "Remove");
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.CLAIM_PERMISSION", props);
            //ClientAPI.Write("Sent remove permission message");
        }

        public void SendEditClaim()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("claimID", activeClaim.id);
            props.Add("name", activeClaim.name);
            props.Add("forSale", activeClaim.forSale);
            props.Add("cost", activeClaim.cost);
            props.Add("currency", activeClaim.currency);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM", props);
            //ClientAPI.Write("Sent edit claim message");
        }

        public void PurchaseClaim()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("claimID", activeClaim.id);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.PURCHASE_CLAIM", props);
            //ClientAPI.Write("Sent purchase claim message");
        }

        public void SendPlaceClaimObject(int buildObjectID, AtavismInventoryItem item, Transform transform, int parent)
        {
            SendPlaceClaimObject(buildObjectID, item, transform, parent, "");
        }

        public void SendPlaceClaimObject(int buildObjectID, AtavismInventoryItem item, Transform transform, int parent, string parents)
        {
            AtavismLogger.LogDebugMessage("SendPlaceClaimObject parent="+parent+" claim="+activeClaim.id+" buildObjectID="+buildObjectID+" pos="+transform.position+" rot="+transform.rotation);

            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("claim", activeClaim.id);
            props.Add("loc", transform.position);
            props.Add("orient", transform.rotation);
            props.Add("parent", parent);
            props.Add("parents", parents);

            if (item != null)
            {
                props.Add("buildObjectTemplateID", buildObjectID);
                props.Add("itemID", item.templateId);
                props.Add("itemOID", item.ItemId);
            }
            else
            {
                props.Add("buildObjectTemplateID", buildObjectID);
                props.Add("itemID", -1);
                props.Add("itemOID", null);
            }

            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.PLACE_CLAIM_OBJECT", props);
        }

        public void RequestClaimObjectInfo(int claimObjectID)
        {
            AtavismLogger.LogDebugMessage("RequestClaimObjectInfo");
            if (activeClaim != null)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("claimID", activeClaim.id);
                props.Add("objectID", claimObjectID);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.GET_CLAIM_OBJECT_INFO", props);
            }
        }

        public void SendEditObjectPosition(ClaimObject cObject, int parent)
        {
            AtavismLogger.LogDebugMessage("!!! SendEditObjectPosition parent="+parent+" claim="+activeClaim.id+" oId="+cObject.ID+" pos="+cObject.gameObject.transform.position+" rot="+cObject.gameObject.transform.rotation);
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("action", "save");
            props.Add("claimID", activeClaim.id);
            props.Add("objectID", cObject.ID);
            props.Add("loc", cObject.gameObject.transform.position);
            props.Add("orient", cObject.gameObject.transform.rotation);
            props.Add("parent", parent);
            props.Add("confirmed", false);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM_OBJECT", props);
        }
        
        public void SendEditObjectPosition(ClaimObject cObject, Transform trans,int parent)
        {
            AtavismLogger.LogDebugMessage("!!! SendEditObjectPosition parent="+parent+" claim="+activeClaim.id+" oId="+cObject.ID+
                                          " pos="+cObject.gameObject.transform.position+" rot="+
                                          cObject.gameObject.transform.rotation);
           if (activeClaim != null)
           {
               Dictionary<string, object> props = new Dictionary<string, object>();
               props.Add("action", "save");
               props.Add("claimID", activeClaim.id);
               props.Add("objectID", cObject.ID);
               props.Add("loc", trans.position);
               props.Add("orient", trans.rotation);
               props.Add("parent", parent);
               props.Add("confirmed", false);
               NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM_OBJECT", props);
           }
        }
        public void SendEditObjectPosition(int ObjectId,Vector3 position,Quaternion rotation,int parent)
        {
            AtavismLogger.LogDebugMessage("!!! SendEditObjectPosition parent="+parent+" claim="+activeClaim.id+" oId="+ObjectId+
                                          " pos="+position+" rot="+ rotation);
            if (activeClaim != null)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("action", "save");
                props.Add("claimID", activeClaim.id);
                props.Add("objectID", ObjectId);
                props.Add("loc", position);
                props.Add("orient", rotation);
                props.Add("confirmed", true);
                props.Add("parent", parent);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM_OBJECT", props);
            }
        }



        public void ImproveBuildObject(GameObject objectBeingEdited, AtavismInventoryItem item, int count)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("claimID", activeClaim.id);
            props.Add("objectID", objectBeingEdited.GetComponent<ClaimObject>().ID);
            props.Add("itemID", item.templateId);
            props.Add("itemOID", item.ItemId);
            props.Add("count", count);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.UPGRADE_BUILDING_OBJECT", props);
        }

        public void ImproveBuildObject()
        {
            if (selectedObject == null || activeClaim == null)
            {
                
                return;
            }

            if (itemsForUpgradeMustBeInserted && (itemsPlacedToUpgrade.Count == 0  || selectedObject.ItemReqs.Count > itemsPlacedToUpgrade.Count))
                return;
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("claimID", activeClaim.id);
            props.Add("objectID", selectedObject.ID);
            props.Add("itemsCount", itemsPlacedToUpgrade.Count);
            for (int i = 0; i < itemsPlacedToUpgrade.Count; i++)
            {
                props.Add("itemID" + i, itemsPlacedToUpgrade[i].templateId);
                props.Add("itemOID" + i, itemsPlacedToUpgrade[i].ItemId);
                props.Add("count" + i, itemsPlacedToUpgrade[i].Count);
            }
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.UPGRADE_BUILDING_OBJECT", props);

            AtavismLogger.LogDebugMessage("Sending " + itemsPlacedToUpgrade.Count + " items");
        }

        public void PickupClaimObject()
        {
            AtavismLogger.LogDebugMessage("PickupClaimObject");
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("action", "convert");
            props.Add("claimID", activeClaim.id);
            props.Add("objectID", SelectedObject.ID);
            props.Add("confirmed", true); 
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM_OBJECT", props);
            BuildingState = WorldBuildingState.Standard;
            WorldBuilderInterface.Instance.ClearCurrentReticle(false);
        }

        public void PickupClaimObject(int id)
        {
            AtavismLogger.LogDebugMessage("PickupClaimObject id="+id);

            PickupClaimObject(id, false);
        }
        public void PickupClaimObject(int id, bool confirm)
        {
            AtavismLogger.LogDebugMessage("PickupClaimObject id="+id+" confirm="+confirm);

            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("action", "convert");
            props.Add("claimID", activeClaim.id);
            props.Add("objectID", id);
            props.Add("confirmed", confirm);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM_OBJECT", props);
            BuildingState = WorldBuildingState.Standard;
            WorldBuilderInterface.Instance.ClearCurrentReticle(false);
        }
        
        public void SendPayTaxForClaim(int id, bool confirmed)
        {
            AtavismLogger.LogDebugMessage("SendPayTaxForClaim id="+id+" confirm="+confirmed);

            Dictionary<string, object> props = new Dictionary<string, object>();
            if(confirmed)
                props.Add("action", "pay");
            else
                props.Add("action", "get");
            props.Add("claimID", id);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.PAY_TAX_CLAIM", props);
        }
        
        
        #endregion Message Senders

        public void StartPlaceClaimObject(UGUIAtavismActivatable activatable)
        {
            AtavismInventoryItem item = (AtavismInventoryItem)activatable.ActivatableObject;
            StartPlaceClaimObject(item);
        }

        public void StartPlaceClaimObject(AtavismInventoryItem item)
        {
            if (activeClaim == null)
                return;
            string[] args = new string[1];
            args[0] = item.ItemId.ToString();
            AtavismEventSystem.DispatchEvent("PLACE_CLAIM_OBJECT", args);
        }

        void SpawnClaimObject(ClaimObjectData objectData)
        {
            AtavismLogger.LogDebugMessage("SpawnClaimObject id="+objectData.objectID+" in claim "+objectData.claimID);
            // Add the gameObject to the claim
            Claim claim = GetClaim(objectData.claimID);
            if (claim == null)
            {
                AtavismLogger.LogWarning("No Claim found for Claim Object");
                return;
            }

            AtavismProgressBar apb = null;
            AtavismHealthBar ahb = null;
            int objId = -1;
            if (claim.claimObjects.ContainsKey(objectData.objectID))
            {
                ClaimObject co = claim.claimObjects[objectData.objectID];
                if (co != null)
                {

                    if (selectedClaimObject != null)
                        objId = selectedClaimObject.ID;
                    apb = co.progressBar;
                    ahb = co.healthBar;
                    if (co.prefab != objectData.prefabName)
                    {
                        if(apb!=null)
                            apb.transform.SetParent(null);
                        if(ahb!=null)
                            ahb.transform.SetParent(null);
                        Destroy(co.gameObject);
                        claim.claimObjects.Remove(objectData.objectID);
                    }
                    else
                    {

                        claim.claimObjects[objectData.objectID].Health = objectData.health;
                        claim.claimObjects[objectData.objectID].MaxHealth = objectData.maxHealth;
                        claim.claimObjects[objectData.objectID].currentTime = objectData.currentBuildTime;
                        claim.claimObjects[objectData.objectID].totalTime = objectData.totalBuildTime;
                        claim.claimObjects[objectData.objectID].lastBuildTimeUpdate = objectData.lastBuildTimeUpdate;
                        claim.claimObjects[objectData.objectID].timeSpeed = (float)objectData.timeSpeed;
                        claim.claimObjects[objectData.objectID].Attackable = objectData.attackable;
                        claim.claimObjects[objectData.objectID].Repairable = objectData.repairable;
                        claim.claimObjects[objectData.objectID].Solo = objectData.solo;
                        claim.claimObjects[objectData.objectID].Status = objectData.status;

                        claim.claimObjects[objectData.objectID].UpdateProgress();
                    //    Debug.LogError("SpawnClaimObject: damage "+claim.claimObjects[objectData.objectID].damage+" | "+objectData.damage);
                        if (!claim.claimObjects[objectData.objectID].damage.Equals(objectData.damage))
                        {
                            
                            if (claim.claimObjects[objectData.objectID].damageGameObject != null)
                            {
                                Destroy(claim.claimObjects[objectData.objectID].damageGameObject);
                            }

                            claim.claimObjects[objectData.objectID].damage = objectData.damage;
                            if (objectData.damage.Length > 0)
                            {
                                string damagePrefabName = objectData.damage;
                                int resourcePathPosDamage = damagePrefabName.IndexOf("Resources/");
                                if (damagePrefabName.Length > 10)
                                {
                                    damagePrefabName = damagePrefabName.Substring(resourcePathPosDamage + 10);
                                    damagePrefabName = damagePrefabName.Remove(damagePrefabName.Length - 7);
                                }

                                GameObject damagePprefab = (GameObject) Resources.Load(damagePrefabName);
                                if (damagePprefab == null)
                                {
                                    Debug.LogError("Builder prefab " + damagePprefab + " not found in the resources");
                                    return;
                                }

                                claim.claimObjects[objectData.objectID].damageGameObject = (GameObject) UnityEngine.Object.Instantiate(damagePprefab, claim.claimObjects[objectData.objectID].gameObject.transform);
                            }
                        }
                        
                        
                        if (selectedClaimObject!=null && selectedClaimObject.ID == objectData.objectID)
                        {
                            string[] _args = new string[2];
                            AtavismEventSystem.DispatchEvent("OBJECT_TARGET_CHANGED", _args);                           
                        }
                        return;
                    }
                }
                else
                {
                   return;
                }
            }
            // Spawn the object in the world
            string prefabName = objectData.prefabName;
            int resourcePathPos = prefabName.IndexOf("Resources/");
            if (prefabName.Length > 10)
            {
                prefabName = prefabName.Substring(resourcePathPos + 10);
                prefabName = prefabName.Remove(prefabName.Length - 7);
            }

            GameObject prefab = (GameObject)Resources.Load(prefabName);
            if (prefab == null)
            {
                Debug.LogError("Builder prefab " + prefabName + " not found in the resources");
                return;
            }

            GameObject claimObject = (GameObject)UnityEngine.Object.Instantiate(prefab, objectData.loc + claim.loc, objectData.orient);
            // TEMP: set claim object to don't delete on load
            DontDestroyOnLoad(claimObject);
            // Get the Claim Object Component
            ClaimObject cObject = claimObject.GetComponentInChildren<ClaimObject>();
            if (cObject == null)
            {
                cObject = claimObject.AddComponent<ClaimObject>();
            }
            
            cObject.prefab = objectData.prefabName;
            cObject.ClaimID = objectData.claimID;
            cObject.StateUpdated(objectData.state);
            cObject.ID = objectData.objectID;
            cObject.TemplateID = objectData.templateID;
            cObject.Health = objectData.health;
            cObject.MaxHealth = objectData.maxHealth;
            cObject.Complete = objectData.complete;
            cObject.FinalStage = objectData.finalStage;
            cObject.Health = objectData.health;
            cObject.MaxHealth = objectData.maxHealth;
            cObject.Attackable = objectData.attackable;
            cObject.Repairable = objectData.repairable;
            cObject.Solo = objectData.solo;
            cObject.Status = objectData.status;
            cObject.currentTime = objectData.currentBuildTime;
            cObject.totalTime = objectData.totalBuildTime;
            cObject.lastBuildTimeUpdate = objectData.lastBuildTimeUpdate;
            cObject.timeSpeed = (float)objectData.timeSpeed;
            if (apb != null)
            {
                cObject.progressBar = apb;
                cObject.progressBar.setClaimObject(cObject);
                cObject.progressBar.transform.SetParent(cObject.transform);
            }
            else
            {
                if (objectTimerPrefab != null)
                {
                    cObject.progressBar = Instantiate(objectTimerPrefab, objectData.loc + claim.loc, objectData.orient);
                    cObject.progressBar.setClaimObject(cObject);
                    cObject.progressBar.transform.SetParent(cObject.transform);
                }
            }
            if (ahb != null)
            {
                cObject.healthBar = ahb;
                cObject.healthBar.setClaimObject(cObject);
                cObject.healthBar.transform.SetParent(cObject.transform);
            }
            else
            {
                if (objectHealthPrefab != null)
                {
                    cObject.healthBar = Instantiate(objectHealthPrefab, objectData.loc + claim.loc, objectData.orient);
                    cObject.healthBar.setClaimObject(cObject);
                    cObject.healthBar.transform.SetParent(cObject.transform);
                }
            }

            cObject.UpdateProgress();
            claim.claimObjects.Add(objectData.objectID, cObject);
            cObject.damage = objectData.damage;
            if (cObject.damage.Length > 0)
            {
                string damagePrefabName1 = cObject.damage;
                int resourcePathPosDamage1 = damagePrefabName1.IndexOf("Resources/");
                if (damagePrefabName1.Length > 10)
                {
                    damagePrefabName1 = damagePrefabName1.Substring(resourcePathPosDamage1 + 10);
                    damagePrefabName1 = damagePrefabName1.Remove(damagePrefabName1.Length - 7);
                }

                GameObject damagePprefab1 = (GameObject) Resources.Load(damagePrefabName1);
                if (damagePprefab1 == null)
                {
                    Debug.LogError("Builder prefab " + damagePprefab1 + " not found in the resources");
                    return;
                }

                cObject.damageGameObject = (GameObject) UnityEngine.Object.Instantiate(damagePprefab1, cObject.gameObject.transform);
            }

            if (cObject.ID == objId)
            {
                SelectedClaimObject = cObject;
            }
            if (cObject.ID == selectedID)
            {
                SelectedObject = cObject;
            }
            string[] args = new string[1];
            args[0] = "";
            AtavismEventSystem.DispatchEvent("CLAIM_OBJECT_UPDATED", args);
        }

        public void AttackClaimObject(GameObject obj)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("claimID", obj.GetComponent<ClaimObject>().ClaimID);
            props.Add("objectID", obj.GetComponent<ClaimObject>().ID);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.ATTACK_BUILDING_OBJECT", props);
            //ClientAPI.Write("Sent attack claim object");
        }

        public List<AtavismBuildObjectTemplate> GetBuildObjectsOfCategory(int category, bool nonItemsOnly)
        {
            List<AtavismBuildObjectTemplate> buildObjects = new List<AtavismBuildObjectTemplate>();
            foreach (AtavismBuildObjectTemplate template in buildObjectTemplates.Values)
            {
                //Debug.LogError("GetBuildObjectsOfCategory "+template.buildObjectName+" BCategory="+template.category+" category="+category+" Limits count="+(ActiveClaim!=null?ActiveClaim.limits.Count+"":"BD"));
                if (template != null && (template.category == category || (category == -2 && ActiveClaim != null && ActiveClaim.limits.Count == 0) || (category == -2 && ActiveClaim != null && ActiveClaim.limits.ContainsKey(template.category))) &&
                    (ActiveClaim != null && (template.validClaimTypes.Contains((ActiveClaim.claimType)) || ActiveClaim.claimType == GetAnyClaimType() || template.validClaimTypes.Contains(GetAnyClaimType()))) &&
                    (!nonItemsOnly || !template.onlyAvailableFromItem))
                {
                   // Debug.LogError("GetBuildObjectsOfCategory " + template.buildObjectName + " added");
                    buildObjects.Add(template);
                }
            }

            return buildObjects;
        }

        #region Message Handlers

        private void BuilderSettingsMessage(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("BuilderSettingsMessage ");
            claimTypes.Clear();
            buildingCategory.Clear();
            int cTypeNum = (int) props["cTypeNum"];
            for (int i = 0; i < cTypeNum; i++)
            {
                string cTypeName = (string) props["cTName" + i];
                int cTypeId = (int) props["cTId" + i];
                claimTypes.Add(i, new OptionChoice() {name = cTypeName, id = cTypeId});
            }
            int catNum = (int) props["catNum"];
            for (int i = 0; i < catNum; i++)
            {
                string cName = (string) props["cName" + i];
                int cId = (int) props["cId" + i];
                buildingCategory.Add(i, new OptionChoice() {name = cName, id = cId});
            }
            
            AtavismLogger.LogDebugMessage("BuilderSettingsMessage ");
        }

        private void ClaimTaxPaymentMessage(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("ClaimTaxPamentMessage ");
            int claimID = (int) props["claimID"];
            int currency = (int) props["currency"];
            long amount = (long) props["cost"];
            long time = (long) props["time"];
          //  Debug.LogError("ClaimTaxPamentMessage claimID="+claimID+" currency="+currency+" amount="+amount+" time="+time);

            long days = 0;
            long hour = 0;
            if (time > 24)
            {
                days = (long) (time / 24F);
                hour =  (time - (days * 24));
            }
            else
            {
                hour = time;
            }
                
            string cost = Inventory.Instance.GetCostString(currency, amount);
           
                 
#if AT_I2LOC_PRESET
            string msg = I2.Loc.LocalizationManager.GetTranslation("Do you want to pay land tax of")+" " +cost+ " "+I2.Loc.LocalizationManager.GetTranslation("for")+ (days > 0 ? days + " "+I2.Loc.LocalizationManager.GetTranslation("days")+" " : "") + (hour > 0 ? hour + " "+I2.Loc.LocalizationManager.GetTranslation("hour")+" " : "") + "?";
#else
            string msg = "Do you want to pay land tax of " + cost + " for " + (days > 0 ? days + " days " : "") + (hour > 0 ? hour + " hour " : "") + "?";
#endif   
            UGUIConfirmationPanel.Instance.ShowConfirmationBox(msg, claimID, PayTaxResponse);
           /* string[] args = new string[2];
            args[0] = "" + claimID;
            args[1] = "" + cost;
            AtavismEventSystem.DispatchEvent("CLAIM_TAX_SHOW", args);*/
           AtavismLogger.LogDebugMessage("ClaimTaxPamentMessage END");
        }


        public void PayTaxResponse(object obj, bool accepted)
        {
            if (accepted)
            {
                int claimId = (int) obj;
                SendPayTaxForClaim(claimId, true);
            }

        }

        private void ClaimUpgradeMessage(Dictionary<string, object> props)
        {
          //  Debug.LogError("ClaimUpgradeMessage ");
            int claimID = (int) props["claimID"];
            int currency = (int) props["currency"];
            long cost = (long) props["cost"];
            string itemsString = (string) props["items"];
            float px = (float) props["locX"];
            float py = (float) props["locY"];
            float pz = (float) props["locZ"];
            float sx = (float) props["sizeX"];
            float sy = (float) props["sizeY"];
            float sz = (float) props["sizeZ"];

            string[] args = new string[7];
            args[0] = "" + claimID;
            args[1] = "" + currency;
            args[2] = "" + cost;
            args[3] = itemsString;
            args[4] = "" + sx;
            args[5] = "" + sy;
            args[6] = "" + sz;
            AtavismEventSystem.DispatchEvent("CLAIM_UPGRADE_SHOW", args);
            AtavismLogger.LogDebugMessage("ClaimUpgradeMessage END");
        }
        
        /// <summary>
        /// Handles the Claim Action Message from the server. Passes the data onto the voxel editor.
        /// </summary>
        /// <param name="props">Properties.</param>
        public void ClaimObjectMessage(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("ClaimObjectMessage ");
            try
            {
                int objectID = (int) props["id"];
                int templateID = (int) props["templateID"];
                string prefabName = (string) props["gameObject"];
                string damage = (string) props["damage"];
                Vector3 loc = (Vector3) props["loc"];
                Quaternion orient = (Quaternion) props["orient"];
                int claimID = (int) props["claimID"];
                //string state = (string)props["state"];
         //       Debug.LogError("ClaimObjectMessage objectID=" + objectID + " templateID=" + templateID + " prefabName=" + prefabName + " claimID=" + claimID+" damage="+damage);
            //    AtavismLogger.LogDebugMessage("Got claim object: " + gameObject);
                //SpawnClaimObject(objectID, claimID, prefabName, loc, orient);
                ClaimObjectData objectData = new ClaimObjectData();
                objectData.objectID = objectID;
                objectData.claimID = claimID;
                objectData.templateID = templateID;
                objectData.prefabName = prefabName;
                objectData.damage = damage;
                objectData.loc = loc;
                objectData.orient = orient;
                objectData.state = "";
                objectData.health = (int) props["health"];
                objectData.maxHealth = (int) props["maxHealth"];
                objectData.complete = (bool) props["complete"];
                objectData.finalStage = (bool) props["fstage"];
                objectData.solo = (bool) props["solo"];
                objectData.status = (string) props["status"];
                long dTime = (long) props["dTime"];

                objectData.currentBuildTime = (long) props["cTime"] + dTime ;
                objectData.totalBuildTime = (long) props["tTime"];
                objectData.timeSpeed = (double) props["sTime"];
                objectData.lastBuildTimeUpdate = Time.time;
                objectData.buildingRuning = (bool) props["buildRun"];
                objectData.attackable = (bool) props["attackable"];
                objectData.repairable = (bool) props["repairable"];
          //      Debug.LogError("ClaimObjectMessage objectID=" + objectID + " currentBuildTime=" + objectData.currentBuildTime + " totalBuildTime=" + objectData.totalBuildTime + " claimID=" + claimID+" damage="+damage);

                objectsToLoad.Add(objectData);
            }
            catch (System.Exception e)
            {

                Debug.LogError("ClaimObjectMessage Exception=" + e);
            }

            AtavismLogger.LogDebugMessage("ClaimObjectMessage objectsToLoad=" + objectsToLoad.Count);
        }

        /// <summary>
        /// Handles the Claim Action Bulk Message which is used to transfer large amounts of actions at once.
        /// </summary>
        /// <param name="props">Properties.</param>
        public void ClaimObjectBulkMessage(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("ClaimObjectBulkMessage ");

            string slog = "";
            foreach (var key in props.Keys)
            {
                slog += key + "=" + props[key] + "\n";
            }
            AtavismLogger.LogDebugMessage(slog);
            
            try
            {
                int numObjects = (int)props["numObjects"];
                AtavismLogger.LogDebugMessage("Got numObjects: " + numObjects);
                AtavismLogger.LogDebugMessage("ClaimObjectBulkMessage numObjects=" + numObjects);
                for (int i = 0; i < numObjects; i++)
                {
                    string actionString = (string)props["object_" + i];
                 //   Debug.LogError("ClaimObjectBulkMessage  actionString=" + actionString);
                    string[] actionData = actionString.Split(';');
                    string objectID = actionData[0];
                    string templateID = actionData[1];
                    string gameObject = actionData[2];
                    string[] locData = actionData[3].Split(',');
                    AtavismLogger.LogDebugMessage("ClaimObjectBulkMessage Loc=>"+string.Join("|",locData) +"< string="+ actionData[3]);
                    Vector3 loc = new Vector3(float.Parse(locData[0]), float.Parse(locData[1]), float.Parse(locData[2]));
                    string[] normalData = actionData[4].Split(',');
                    AtavismLogger.LogDebugMessage("ClaimObjectBulkMessage Rot=>" + string.Join("|", normalData) + "< string=" + actionData[4]);
                    Quaternion orient = new Quaternion(float.Parse(normalData[0]), float.Parse(normalData[1]), float.Parse(normalData[2]), float.Parse(normalData[3]));
                    string state = actionData[5];
                    int health = int.Parse(actionData[6]);
                    int maxHealth = int.Parse(actionData[7]);
                    bool complete = bool.Parse(actionData[8]);
                    
                    string interactionType = actionData[9];
                    int lockTemplateId = int.Parse(actionData[10]);
                    int lockDurability = int.Parse(actionData[11]); 
                    bool attackable = bool.Parse(actionData[12]);
                    bool repairable = bool.Parse(actionData[13]);
                    string damage = actionData[14];
                    bool solo = bool.Parse(actionData[15]);
                    string status = actionData[16];
                   // Debug.LogError("ClaimObjectBulkMessage objectID="+objectID+" gameObject="+gameObject+" damage="+damage);

                   long tTime = long.Parse(actionData[17]);
                   long cTime = long.Parse(actionData[18]);
                   double sTime = double.Parse(actionData[19]);
                   bool buildRun = bool.Parse(actionData[20]);
                   long dTime = long.Parse(actionData[21]);

                   
                    //SpawnClaimObject(int.Parse(objectID), int.Parse(claimID), gameObject, loc, orient);
                    ClaimObjectData objectData = new ClaimObjectData();
                    objectData.objectID = int.Parse(objectID);
                    objectData.templateID = int.Parse(templateID);
                    objectData.claimID = (int)props["claimID"];
                    objectData.prefabName = gameObject;
                    objectData.damage = damage;
                    objectData.loc = loc;
                    objectData.orient = orient;
                    objectData.state = state;
                    objectData.health = health;
                    objectData.maxHealth = maxHealth;
                    objectData.complete = complete;
                    
                    objectData.attackable = attackable;
                    objectData.repairable = repairable;
                    objectData.interactionType = interactionType;
                    objectData.lockTemplateId = lockTemplateId;
                    objectData.lockDurability = lockDurability;
                    objectData.solo = solo;    
                    objectData.status = status;    

                    objectData.currentBuildTime = cTime + dTime ;
                    objectData.totalBuildTime = tTime;
                    objectData.timeSpeed = sTime;
                    objectData.lastBuildTimeUpdate = Time.time;
                    objectData.buildingRuning = buildRun;
                   // Debug.LogError("ClaimObjectBulkMessage objectID=" + objectID + " currentBuildTime=" + objectData.currentBuildTime + " totalBuildTime=" + objectData.totalBuildTime + " claimID=" + objectData.claimID+" damage="+damage);

                    
                    
                    
                    objectsToLoad.Add(objectData);
                }
            }
            catch (System.Exception e)
            {

                Debug.LogError("ClaimObjectBulkMessage Exception=" + e);
            }
            AtavismLogger.LogDebugMessage("ClaimObjectBulkMessage objectsToLoad=" + objectsToLoad.Count);
        }

        public void MoveClaimObjectMessage(Dictionary<string, object> props)
        {
            int objectID = (int)props["id"];
            Vector3 loc = (Vector3)props["loc"];
            Quaternion orient = (Quaternion)props["orient"];
            int claimID = (int)props["claimID"];

            Claim claim = GetClaim(claimID);
            if (claim != null)
            {
                var pb = claim.claimObjects[objectID].GetComponent<PieceBehaviour>();
                if (pb != null)
                {
                    pb.UpdateOccupancy(false);
                }
                claim.claimObjects[objectID].transform.position = loc + claim.loc;
                claim.claimObjects[objectID].transform.rotation = orient;
                if (pb != null)
                {
                    pb.UpdateOccupancy(true);
                }
            }
            
            AtavismLogger.LogDebugMessage("Got claim object: " + gameObject);
            //SpawnClaimObject(objectID, claimID, prefabName, loc, orient);
        }

        public void UpdateClaimObjectStateMessage(Dictionary<string, object> props)
        {
            int objectID = (int)props["id"];
            string state = (string)props["state"];
            int claimID = (int)props["claimID"];

            Claim claim = GetClaim(claimID);
            if (claim != null)
            {
                claim.claimObjects[objectID].StateUpdated(state);
            }

            //Debug.Log("Got claim object: " + gameObject);
            //SpawnClaimObject(objectID, claimID, prefabName, loc, orient);
        }

        public void ClaimObjectInfoMessage(Dictionary<string, object> props)
        {
         //  Debug.LogError("ClaimObjectInfoMessage");
            string ks = " [ ";
            foreach (var it in props.Keys)
            {
                ks += " ; " + it + " => " + props[it];
            }
         //   Debug.LogWarning("HandleBuildObjectPrefabData: keys:" + ks);
            itemsPlacedToUpgrade.Clear();
            int objectID = (int)props["id"];
            int claimID = (int)props["claimID"];
            int health = (int)props["health"];
            int maxHealth = (int)props["maxHealth"];
            int itemCount = (int)props["itemCount"];
            bool complete = (bool)props["complete"];
            bool finalStage = (bool)props["fstage"];
            string  damage = (string)props["damage"];
            bool solo = (bool)props["solo"];
            string  status = (string)props["status"];

            long dTime = (long) props["dTime"];

            long currentBuildTime = (long) props["cTime"] + dTime ;
            long totalBuildTime = (long) props["tTime"];
            double timeSpeed = (double) props["sTime"];
            float lastBuildTimeUpdate = Time.time;
            Claim claim = GetClaim(claimID);
            if (claim != null)
            {
                if (!claim.claimObjects.ContainsKey(objectID))
                {
                    Debug.LogError("ClaimObjectInfoMessage claim dont have object " + objectID);
                    return;
                }
                claim.claimObjects[objectID].Health = health;
                claim.claimObjects[objectID].MaxHealth = maxHealth;
                claim.claimObjects[objectID].Complete = complete;
                claim.claimObjects[objectID].FinalStage = finalStage;
                claim.claimObjects[objectID].Solo = solo;
                claim.claimObjects[objectID].currentTime = currentBuildTime;
                claim.claimObjects[objectID].totalTime = totalBuildTime;
                claim.claimObjects[objectID].timeSpeed = (float)timeSpeed;
                claim.claimObjects[objectID].lastBuildTimeUpdate = lastBuildTimeUpdate;
                claim.claimObjects[objectID].Status = status;
                
                
                if (!claim.claimObjects[objectID].damage.Equals(damage))
                {
                    if (claim.claimObjects[objectID].damageGameObject != null)
                    {
                        Destroy(claim.claimObjects[objectID].damageGameObject);
                    }

                    claim.claimObjects[objectID].damage = damage;
                    string damagePrefabName = damage;
                    int resourcePathPosDamage = damagePrefabName.IndexOf("Resources/");
                    if (damagePrefabName.Length > 10)
                    {
                        damagePrefabName = damagePrefabName.Substring(resourcePathPosDamage + 10);
                        damagePrefabName = damagePrefabName.Remove(damagePrefabName.Length - 7);
                    }

                    GameObject damagePprefab = (GameObject)Resources.Load(damagePrefabName);
                    if (damagePprefab == null)
                    {
                        Debug.LogError("Builder prefab " + damagePprefab + " not found in the resources");
                        
                    }
                    else
                    {
                        claim.claimObjects[objectID].damageGameObject = (GameObject) UnityEngine.Object.Instantiate(damagePprefab, claim.claimObjects[objectID].gameObject.transform);
                    }
                }
                if (selectedClaimObject!=null && selectedClaimObject.ID == objectID)
                {
                    string[] _args = new string[2];
                    AtavismEventSystem.DispatchEvent("OBJECT_TARGET_CHANGED", _args);                           
                }
                Dictionary<int, int> itemReqs = new Dictionary<int, int>();
                for (int i = 0; i < itemCount; i++)
                {
                    itemReqs.Add((int)props["item" + i], (int)props["itemCount" + i]);
                }
                claim.claimObjects[objectID].ItemReqs = itemReqs;

                // Work out how much time is left in the current task
                long timeCompleted = (long)props["timeCompleted"];
                if (timeCompleted > 0)
                {
                    long timeLeft = (long)props["timeLeft"];
                    float secondsCompleted = (float)timeCompleted / 1000f;
                    float secondsLeft = (float)timeLeft / 1000f;
                    string[] csArgs = new string[2];
                    csArgs[0] = secondsCompleted.ToString();
                    csArgs[1] = secondsLeft.ToString();
                    AtavismEventSystem.DispatchEvent("GROWING_STARTED", csArgs);

                }
                else
                {
                    string[] csArgs = new string[2];
                    csArgs[0] = "0";
                    csArgs[1] = "0";
                    AtavismEventSystem.DispatchEvent("GROWING_STARTED", csArgs);
                }
            }
           // Debug.LogError("ClaimObjectInfoMessage selectedClaimObject="+selectedClaimObject+" "+(selectedClaimObject!=null?selectedClaimObject.ID+"":"NA")+" "+objectID);

           
            // Dispatch event to be picked up by UI
            string[] args = new string[1];
            args[0] = "";
            AtavismEventSystem.DispatchEvent("CLAIM_OBJECT_UPDATED", args);
        }


        Vector3 moveloc = Vector3.zero;
        Quaternion moveorient = Quaternion.identity;
        
        
        public void MoveClaimObjectConfirmMessage(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("MoveClaimObjectConfirmMessage");
            int objectID = (int) props["id"];
            int claimID = (int) props["claimID"];
            moveloc = (Vector3) props["loc"];
            moveorient = (Quaternion) props["orient"];
            int num = (int) props["num"];
            AtavismLogger.LogDebugMessage("MoveClaimObjectConfirmMessage claimID="+claimID+" objectID="+objectID+" num="+num);
            objToDelete.Clear();
            objToDelete.Add(objectID);
            Claim claim = GetClaim(claimID);
            var pbg = claim.claimObjects[objectID].GetComponent<PieceBehaviour>();
            if (pbg != null)
            {
                //claim.claimObjects[objectID].gameObject.ChangeAllMaterialsInChildren(pbg.Renderers.ToArray(), pbg.PreviewMaterial);
                claim.claimObjects[objectID].gameObject.ChangeAllMaterialsColorInChildren(pbg.Renderers.ToArray(), pbg.PreviewDeniedColor);
               // claim.claimObjects[objectID].gameObject.ChangeAllMaterialsInChildren(pbg.Renderers.ToArray(), pbg.InitialRenderers);
            }
            if (claim != null)
            {
                for (int i = 0; i < num;i++)
                {
                    int chaildId = (int) props["chaildId" + i];
                    objToDelete.Add(chaildId);
                    if (claim.claimObjects.ContainsKey(chaildId) && claim.claimObjects[chaildId]!=null)
                    {
                        var pb = claim.claimObjects[chaildId].GetComponent<PieceBehaviour>();
                        AtavismLogger.LogDebugMessage("MoveClaimObjectConfirmMessage: chaildId=" + chaildId + " pb=" + pb);
                        if (pb != null)
                        {
                            AtavismLogger.LogDebugMessage("MoveClaimObjectConfirmMessage: chaildId=" + chaildId + " pb=" + pb.Renderers.ToArray());
                            //claim.claimObjects[chaildId].gameObject.ChangeAllMaterialsInChildren(pbg.Renderers.ToArray(), pbg.PreviewMaterial);
                            claim.claimObjects[chaildId].gameObject.ChangeAllMaterialsColorInChildren(pbg.Renderers.ToArray(), pbg.PreviewDeniedColor);

                        }
                    }
                }
            }


#if AT_I2LOC_PRESET
            string msg = I2.Loc.LocalizationManager.GetTranslation("Move of this element requires deletes dependent objects")+"\n " + I2.Loc.LocalizationManager.GetTranslation("Do you really want to delete this item?");
#else
            string msg = "Move of this element requires deletes dependent objects.\nDo you really want move object and delete dependent objects?";
#endif
            UGUIConfirmationPanel.Instance.ShowConfirmationBox(msg, objectID, MoveObjectResponse);
        }
        public void MoveObjectResponse(object obj, bool accepted)
        {
            if (accepted)
            {
                int objectID = (int) obj;
                SendEditObjectPosition(objectID, moveloc, moveorient, -1);
            }
            else
            {
                Claim claim = activeClaim;
                if (claim != null)
                    foreach (var v in objToDelete)
                    {
                        var pb = claim.claimObjects[v].GetComponent<PieceBehaviour>();
                        claim.claimObjects[v].gameObject.ChangeAllMaterialsInChildren(pb.Renderers.ToArray(), pb.InitialRenderers);
                    }
            }
        }
        
        List<int> objToDelete = new List<int>();
        public void RemoveClaimObjectConfirmMessage(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("RemoveClaimObjectConfirmMessage");
            int objectID = (int) props["id"];
            int claimID = (int) props["claimID"];
            int num = (int) props["num"];
            AtavismLogger.LogDebugMessage("RemoveClaimObjectConfirmMessage claimID="+claimID+" objectID="+objectID+" num="+num);
            objToDelete.Clear();
            objToDelete.Add(objectID);
            Claim claim = GetClaim(claimID);
            var pbg = claim.claimObjects[objectID].GetComponent<PieceBehaviour>();
            if (pbg != null)
            {
               // claim.claimObjects[objectID].gameObject.ChangeAllMaterialsInChildren(pbg.Renderers.ToArray(), pbg.PreviewMaterial);
                claim.claimObjects[objectID].gameObject.ChangeAllMaterialsColorInChildren(pbg.Renderers.ToArray(), pbg.PreviewDeniedColor);
               // claim.claimObjects[objectID].gameObject.ChangeAllMaterialsInChildren(pbg.Renderers.ToArray(), pbg.InitialRenderers);
            }
            if (claim != null)
            {
                for (int i = 0; i < num;i++)
                {
                    int chaildId = (int) props["chaildId" + i];
                    objToDelete.Add(chaildId);
                    var pb = claim.claimObjects[chaildId].GetComponent<PieceBehaviour>();
                    AtavismLogger.LogDebugMessage("RemoveClaimObjectConfirmMessage: chaildId="+chaildId+" pb="+pb);
                    if (pb != null)
                    {
                        AtavismLogger.LogDebugMessage("RemoveClaimObjectConfirmMessage: chaildId=" + chaildId + " pb=" + pb.Renderers.ToArray());
                      //  claim.claimObjects[chaildId].gameObject.ChangeAllMaterialsInChildren(pbg.Renderers.ToArray(), pbg.PreviewMaterial);
                        claim.claimObjects[chaildId].gameObject.ChangeAllMaterialsColorInChildren(pbg.Renderers.ToArray(), pbg.PreviewDeniedColor);

                    }
                }
            }


#if AT_I2LOC_PRESET
            string msg = I2.Loc.LocalizationManager.GetTranslation("Deletion of this element deletes dependent objects")+"\n " + I2.Loc.LocalizationManager.GetTranslation("Do you really want to delete this object?");
#else
            string msg = "Deletion of this element deletes dependent objects.\nDo you really want to delete this object?";
#endif
            UGUIConfirmationPanel.Instance.ShowConfirmationBox(msg, objectID, DeleteObjectResponse);
        }


        public void DeleteObjectResponse(object obj, bool accepted)
        {
            if (accepted)
            {
                int objectID = (int) obj;
                PickupClaimObject(objectID, true);
            }
            else
            {
                Claim claim = activeClaim;
                if (claim != null)
                    foreach (var v in objToDelete)
                    {
                        var pb = claim.claimObjects[v].GetComponent<PieceBehaviour>();
                        claim.claimObjects[v].gameObject.ChangeAllMaterialsInChildren(pb.Renderers.ToArray(), pb.InitialRenderers);
                    }

                //     claim.claimObjects[objectID].gameObject.ChangeAllMaterialsInChildren(pbg.Renderers.ToArray(), pbg.InitialRenderers);
            }

            //  RespondToGuildInvitation(inviterOid, guildID, accepted);
        }

        public void RemoveClaimObjectMessage(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("RemoveClaimObjectMessage Start");
            int objectID = (int) props["id"];
            int claimID = (int) props["claimID"];
            bool dependent = (bool) props["dependent"];

            AtavismLogger.LogDebugMessage("RemoveClaimObjectMessage claimID=" + claimID + " objectID=" + objectID + " dependent=" + dependent);

            Claim claim = GetClaim(claimID);
            if (claim != null)
            {
                if (selectedObject != null && objectID == selectedObject.ID)
                {
                    selectedID = objectID;
                    selectedObject = null;
                    // Dispatch event to be picked up by UI
                    string[] args = new string[1];
                    args[0] = "";

                    AtavismEventSystem.DispatchEvent("CLAIM_OBJECT_UPDATED", args);

                }

                if (selectedClaimObject != null && selectedClaimObject.ID == objectID)
                {
                    SelectedClaimObject = null;
                }

                if (claim.claimObjects[objectID].healthBar != null)
                {
                    Destroy(claim.claimObjects[objectID].healthBar.gameObject);
                }

                if (claim.claimObjects[objectID].progressBar != null)
                {
                    Destroy(claim.claimObjects[objectID].progressBar.gameObject);
                }

                claim.claimObjects[objectID].gameObject.SendMessage("Destroy");

                if (dependent)
                {
                    if (claimObjectDestroyDependentDelay > 0)
                    {
                        Destroy(claim.claimObjects[objectID].gameObject, claimObjectDestroyDependentDelay);
                    }
                    else
                    {
                        Destroy(claim.claimObjects[objectID].gameObject);
                    }
                }
                else
                {
                    Destroy(claim.claimObjects[objectID].gameObject, claimObjectDestroyDelay);
                }

                claim.claimObjects.Remove(objectID);
            }
            else
            {
                Debug.LogError("RemoveClaimObjectMessage claim not found");

            }

            AtavismLogger.LogDebugMessage("RemoveClaimObjectMessage End");
        }

        public void ClaimOwnMessage(Dictionary<string, object> props)
        {
         //   Debug.LogError("ClaimOwnMessage");
            int num = (int) props["num"];
            playerClaims.Clear();
            //   Debug.LogWarning("WorldBuilder Got Cliam id="+claimID);
            for (int i = 0; i < num; i++)
            {
                int id = (int) props["id" + i];
                string name = (string) props["name" + i];
                long time = (long) props["time" + i];
                long taxAmount = (long) props["taxAmount"+ i];
                long taxInterval = (long) props["taxInterval"+ i];
                long taxPeriodPay = (long) props["taxPeriodPay"+ i];
                int taxCurrency = (int) props["taxCurrency"+ i];

                //Debug.LogError("ClaimOwnMessage: name="+name+" claimId="+id+" time="+time+" taxAmount="+taxAmount+" taxCurrency="+taxCurrency+" taxInterval="+taxInterval+" taxPeriodPay="+taxPeriodPay);

              
              //  Debug.LogError("ClaimOwnMessage id=" + id + " name=" + name + " time=" + time + " " + Time.time);
                ClaimListEntry cle = new ClaimListEntry();
                cle.name = name;
                cle.id = id;
                cle.time = Time.time + time / 1000F;
                cle.taxAmount = taxAmount;
                cle.taxCurrency = taxCurrency;
                cle.taxPeriodPay = taxPeriodPay;
                cle.taxInterval = taxInterval;
                playerClaims.Add(cle);
            }
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("CLAIM_LIST_UPDATE", args);

            //  AtavismLogger.LogDebugMessage("Got new claim data: " + claim.id);
        }


        public void ClaimIDMessage(Dictionary<string, object> props)
        {
       //     Debug.LogError("ClaimIDMessage");

            int claimID = (int)props["claimID"];
         //   Debug.LogWarning("WorldBuilder Got Cliam id="+claimID);
            Claim claim = new Claim();
            if (GetClaim(claimID) != null)
            {
                claim = GetClaim(claimID);
            }
            else
            {
                claim.id = claimID;
            }

            claim.name = (string)props["claimName"];
            claim.sizeX = (int)props["claimSizeX"];
            claim.sizeZ = (int)props["claimSizeZ"];
            claim.sizeY = claim.sizeX * 3;
            Vector3 pos = Vector3.zero;
            if (props.ContainsKey("ulocX"))
            {
                float px = (float) props["ulocX"];
                float py = (float) props["ulocY"];
                float pz = (float) props["ulocZ"];
                float sx = (float) props["usizeX"];
                float sy = (float) props["usizeY"];
                float sz = (float) props["usizeZ"];
                pos = new Vector3(px, py, pz);
                claim.diffloc = pos;
                claim.sizeX = (int) sx;
                claim.sizeY = (int) sy;
                claim.sizeZ = (int) sz;
                Vector3 size = new Vector3(sx, sy, sz);
                AtavismLogger.LogDebugMessage("WorldBuilder Got Cliam id=" + claimID + " pos " + pos + " size " + size);
                if (claimGameObjects.ContainsKey("Claim" + claimID))
                {
                    if(claimGameObjects["Claim" + claimID]!=null)
                        claimGameObjects["Claim" + claimID].transform.localScale = new Vector3(sx, sy, sz);
                }
            }
            else
            {
                claim.diffloc = Vector3.zero;
            }

            Vector3 loc = (Vector3) props["claimLoc"];
            AtavismLogger.LogDebugMessage("WorldBuilder Got Cliam id=" + claimID + " loc " + loc + " pos " + pos + " " + (loc + pos));
            claim.loc = loc ; 
          
            claim.GenerateBounds();
            claim.stance = (int)props["stance"];
            claim.claimType = (int)props["claimType"];
            claim.ownerName = (string)props["ownerName"];
            claim.forSale = (bool)props["forSale"];
            claim.permissionlevel = (int)props["permissionLevel"];
            if (claim.forSale)
            {
                claim.cost = (long)props["cost"];
                claim.currency = (int)props["currency"];
            }
            claim.purchaseItemReq = (int)props["purchaseItemReq"];
            claim.playerOwned = (bool)props["myClaim"];
            // Player permissions
            claim.permissions.Clear();
            int permissionCount = (int)props["permissionCount"];
            for (int i = 0; i < permissionCount; i++)
            {
                ClaimPermission per = new ClaimPermission();
                per.playerName = (string)props["permission_" + i];
                per.permissionLevel = (int)props["permissionLevel_" + i];
                claim.permissions.Add(per);
            }

            long time = (long) props["taxTime"];
          //  Debug.LogWarning("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ time = "+time);
            if(time>0)
                claim.taxTime = time /1000F+ Time.time;

            long taxAmount = (long) props["taxAmount"];
            long taxInterval = (long) props["taxInterval"];
            long taxPeriodPay = (long) props["taxPeriodPay"];
            int taxCurrency = (int) props["taxCurrency"];

            claim.limits.Clear();
            int limitnum = (int)props["limitnum"];
            for (int i = 0; i < limitnum; i++)
            {
                int id = (int)props["lId" + i];
                int count = (int)props["lc" + i];
                claim.limits.Add(id,count);
            }
            claim.limitsCount.Clear();
            int climitnum = (int)props["climitnum"];
            for (int i = 0; i < climitnum; i++)
            {
                int id = (int)props["clId" + i];
                int count = (int)props["clc" + i];
                claim.limitsCount.Add(id,count);
            }

            
            
            claim.taxAmount = taxAmount;
            claim.taxCurrency = taxCurrency;
            claim.taxPeriodPay = taxPeriodPay;
            claim.taxInterval = taxInterval;
            
            
            if (GetClaim(claimID) == null)
            {
               // Debug.LogWarning("WorldBuilder add to Cliam list id=" + claim.id);
                claims.Add(claim);
            }

            if (claim == activeClaim)
            {
                string[] args = new string[1];
                AtavismEventSystem.DispatchEvent("CLAIM_CHANGED", args);
            }
            AtavismLogger.LogDebugMessage("Got new claim data: " + claim.id);
        }

        public void ClaimRemoveDataMessage(Dictionary<string, object> props)
        {
            int claimID = (int)props["claimID"];
            if (GetClaim(claimID) != null)
            {
                claims.Remove(GetClaim(claimID));
            }
            AtavismLogger.LogDebugMessage("Removed claim data: " + claimID);
        }

        public void ClaimUpdatedMessage(Dictionary<string, object> props)
        {
            int claimID = (int)props["claimID"];
            Claim claim = GetClaim(claimID);
            claim.forSale = (bool)props["forSale"];
            if (claim.forSale)
            {
                claim.cost = (long)props["cost"];
                claim.currency = (int)props["currency"];
            }
            claim.playerOwned = (bool)props["myClaim"];
            claim.purchaseItemReq = (int)props["purchaseItemReq"];
            claim.permissionlevel = (int)props["permissionLevel"];
            claim.limits.Clear();
            int limitnum = (int)props["limitnum"];
            for (int i = 0; i < limitnum; i++)
            {
                int id = (int)props["lId" + i];
                int count = (int)props["lc" + i];
                claim.limits.Add(id,count);
            }
            claim.limitsCount.Clear();
            int climitnum = (int)props["climitnum"];
            for (int i = 0; i < climitnum; i++)
            {
                int id = (int)props["clId" + i];
                int count = (int)props["clc" + i];
                claim.limitsCount.Add(id,count);
            }
            
            AtavismLogger.LogDebugMessage("Got claim update data");
            if (claim == activeClaim)
            {
                string[] args = new string[1];
                AtavismEventSystem.DispatchEvent("CLAIM_CHANGED", args);
            }
        }

        /// <summary>
        /// Handles the Remove Claim Message which means a player is no longer in the radius for a claim
        /// so the client no longer needs to check if they are in its edit radius.
        /// </summary>
        /// <param name="props">Properties.</param>
        public void RemoveClaimMessage(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("Got remove claim data "+ claims.Count);
            try
            {
                int claimID = (int)props["claimID"];
             //   Debug.LogWarning("WorldBuilder Got Cliam to remove id="+claimID);
                Claim claimToRemove = null;
                foreach (Claim claim in claims)
                {
                    if (claim.id == claimID)
                    {
                     //   Debug.LogWarning("WorldBuilder Cliam to remove id=" + claimID);
                        /*int itemID = (int)props["resource"];
                                 int count = (int)props["resourceCount"];
                                 claim.resources[itemID] = count;*/
                        foreach (ClaimObject cObject in claim.claimObjects.Values)
                        {
                          //  Debug.LogWarning("WorldBuilder Cliam to remove id=" + claimID+" COid"+cObject.ID+" name="+ cObject.name);
                            DestroyImmediate(cObject.gameObject);
                        }
                        claimToRemove = claim;
                        break;
                    }
                }
                if (claimToRemove != null)
                {
                    if (claimToRemove == activeClaim)
                    {
                        activeClaim = null;
                        string[] args = new string[1];
                        AtavismEventSystem.DispatchEvent("CLAIM_CHANGED", args);
                    }
                    claims.Remove(claimToRemove);
                    if (claimGameObjects.ContainsKey("Claim" + claimID))
                    {
                        DestroyImmediate(claimGameObjects["Claim" + claimID]);
                        claimGameObjects.Remove("Claim" + claimID);
                    }
                }

            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Got remove claim data Exception "+e.Message+" "+e);
            }
            AtavismLogger.LogDebugMessage("Got remove claim data end " + claims.Count);
        }

        /// <summary>
        /// Temporary hack to remove the claim deed item
        /// </summary>
        /// <param name="props">Properties.</param>
        public void ClaimMadeMessage(Dictionary<string, object> props)
        {
            // Something to be doing?

        }

        public void HandleBuildingResources(Dictionary<string, object> props)
        {
            buildingResources.Clear();
            int numResources = (int)props["numResources"];
            for (int i = 0; i < numResources; i++)
            {
                string resourceID = (string)props["resource" + i + "ID"];
                int resourceCount = (int)props["resource" + i + "Count"];
                buildingResources.Add(int.Parse(resourceID), resourceCount);
            }
            // dispatch a ui event to tell the rest of the system
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("RESOURCE_UPDATE", args);
        }

        public void HandleStartBuildObject(Dictionary<string, object> props)
        {
            // Make sure player is in a claim they own
            if (activeClaim == null)
                return;

            // Get props and send event out to start the placement of the object
            int buildObjectTemplate = (int)props["buildObjectTemplate"];
            // dispatch a ui event to tell the rest of the system
            string[] args = new string[1];
            args[0] = "" + buildObjectTemplate;
            AtavismEventSystem.DispatchEvent("START_BUILD_CLAIM_OBJECT", args);
        }

        public void HandleStartBuildTask(Dictionary<string, object> props)
        {
            //ClientAPI.Write("Starting build task with length: " + (float)props["length"]);
           // Debug.LogError("HandleStartBuildTask");
            float length = (float)props["length"];
         /*   string[] csArgs = new string[3];
            csArgs[0] = length.ToString();
            csArgs[2] = "-1";
            if (selectedObject == null || GetBuildObjectTemplate(selectedObject.TemplateID).buildTaskReqPlayer)
            {
                csArgs[1] = OID.fromLong(ClientAPI.GetPlayerOid()).ToString();
                AtavismEventSystem.DispatchEvent("CASTING_STARTED", csArgs);
            }
            else
            {
                csArgs = new string[2];
                csArgs[0] = "0";
                csArgs[1] = length.ToString();
                AtavismEventSystem.DispatchEvent("GROWING_STARTED", csArgs);
            }*/
            
        }

        public void HandleInterruptBuildTask(Dictionary<string, object> props)
        {
            int objectID = (int)props["id"];
            int claimID = (int)props["claimID"];

            // What do we do now?
            string[] args = new string[2];
            args[0] = "";
            args[1] = OID.fromLong(ClientAPI.GetPlayerOid()).ToString();
            if (selectedObject == null || GetBuildObjectTemplate(selectedObject.TemplateID).buildTaskReqPlayer)
            {
                AtavismEventSystem.DispatchEvent("CASTING_CANCELLED", args);
            }
            else
            {
                AtavismEventSystem.DispatchEvent("GROWING_CANCELLED", args);
            }

            ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismMobController>().PlayAnimation("", 0);
        }

        public void HandleBuilObjIcon(Dictionary<string, object> props)
        {
         //   Debug.LogError("HandleBuilObjIcon " + Time.time);
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                    int id = (int)props["i" + i + "id"];
                    string icon = (string)props["i" + i + "icon"];
                    string icon2 = (string)props["i" + i + "icon2"];
                    Texture2D tex = new Texture2D(2, 2);
                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(icon2));
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

                    AtavismPrefabManager.Instance.SaveBuildObjectIcon(id, sprite, icon2);
                    if (buildObjectTemplates.ContainsKey(id))
                    {
                        buildObjectTemplates[id].icon = sprite;
                    }
                }
                string[] args = new string[1];
               AtavismEventSystem.DispatchEvent("BUILDER_ICON_UPDATE", args);

            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading build obj icon prefab data " + e);
            }
        //    Debug.LogError("HandleBuilObjIcon End");
        }


        public void HandleBuildObjectPrefabData(Dictionary<string, object> props)
        {
        //    Debug.LogError("HandleBuildObjectPrefabData " + Time.time);
          //     Debug.LogWarning("HandleEquippedInventoryUpdate");
          
                  string ks = " [ ";
                  foreach (var it in props.Keys)
                  {
                      ks += " ; " + it + " => " + props[it];
                  }
                //  Debug.LogWarning("HandleBuildObjectPrefabData: keys:" + ks);
          
          int _id = -1;
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                   // Debug.LogError("HandleInvCurrPrefabData " + i);
                    BuildObjPrefabData cpd = new BuildObjPrefabData();
                    _id = (int)props["i" + i + "id"];
                    cpd.id = _id;
                    cpd.buildObjectName = (string)props["i" + i + "name"];
                    cpd.skill = (int)props["i" + i + "skill"];
                    cpd.skillLevelReq = (int)props["i" + i + "skillreqlev"];
                    cpd.category = (int)props["i" + i + "cat"];
                    cpd.reqWeapon = (string)props["i" + i + "reqWeapon"];
                    cpd.distanceReq = ((int)props["i" + i + "distreq"])/1000f;
                    cpd.buildTimeReq = ((int)props["i" + i + "bTime"])/1000f;

                    cpd.buildTaskReqPlayer = (bool)props["i" + i + "taskreqply"];
                   // cpd.validClaimTypes = (ClaimType)(int)props["i" + i + "claimtype"];
                   string clailType = (string)props["i" + i + "claimtype"];
                   if (clailType.Length > 0)
                   {
                       string[] ids = clailType.Split(';');
                       foreach (var id in ids)
                       {
                           cpd.validClaimTypes.Add(int.Parse(id));
                           
                       }
                   }
                    cpd.onlyAvailableFromItem = (bool)props["i" + i + "itemOnly"];
                    cpd.gameObject = (string)props["i" + i + "lstage"];
                    cpd.date = (long)props["i" + i + "date"];


                    string its = (string)props["i" + i + "upditems"];
                   // Debug.LogError("HandleBuildObjectPrefabData: " + cpd.id + " " + cpd.buildObjectName + " upditems=" + its);
                    string[] its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if(s.Length>0)
                        cpd.upgradeItemsReq.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "reqitem"];
                   // Debug.LogError("HandleBuildObjectPrefabData: " + cpd.id + " " + cpd.buildObjectName + " reqitem=" + its);
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.itemsReq.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "reqitemC"];
                  //  Debug.LogError("HandleBuildObjectPrefabData: " + cpd.id + " " + cpd.buildObjectName + " reqitemC=" + its);
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.itemsReqCount.Add(int.Parse(s));
                    }


                    cpd.date = (long)props["i" + i + "date"];
                    AtavismPrefabManager.Instance.SaveBuildObject(cpd);
                }
                if (props.ContainsKey("toRemove"))
                {
                    string keys = (string)props["toRemove"];
                    if (keys.Length > 0)
                    {
                        string[] _keys = keys.Split(';');
                        foreach (string k in _keys)
                        {
                            if (k.Length > 0)
                            {
                                AtavismPrefabManager.Instance.DeleteBuildObject(int.Parse(k));
                            }
                        }
                    }
                }
                if (sendAll)
                {
                    buildObjectTemplates.Clear();
                    List<AtavismBuildObjectTemplate> list = AtavismPrefabManager.Instance.LoadAllBuildObject();
                    foreach (AtavismBuildObjectTemplate c in list)
                    {
                        buildObjectTemplates.Add(c.id, c);
                    }

                    string[] args = new string[1];
                    AtavismEventSystem.DispatchEvent("BUILDER_UPDATE", args);

                    //  itemdataloaded = true;
                    
                    AtavismPrefabManager.Instance.reloaded++;
                    if(AtavismLogger.logLevel <= LogLevel.Debug) 

                    Debug.Log("All data received. Running Queued Build Objects update message.");
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadBuldingObjectsPrefabData();
                    Debug.LogWarning("Not Running Queued Item update message. Not all data sent");
                }
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading Building prefab data for id = "+ _id +" "  + e);
            }
         //   Debug.LogError("HandleInvCurrPrefabData End");
        }



        #endregion Message Handlers

        public void StartPlaceBuildObject(int buildObjectTemplateID)
        {
            // Make sure player is in a claim they own
            if (activeClaim == null)
                return;

            // dispatch a ui event to tell the rest of the system
            string[] args = new string[1];
            args[0] = "" + buildObjectTemplateID;
            AtavismEventSystem.DispatchEvent("START_BUILD_CLAIM_OBJECT", args);
        }

        public AtavismBuildObjectTemplate GetBuildObjectTemplate(int templateID)
        {
            if (!buildObjectTemplates.ContainsKey(templateID))
            {
                AtavismBuildObjectTemplate it = AtavismPrefabManager.Instance.LoadBuildObject(templateID);
                if (it != null)
                {
                    buildObjectTemplates.Add(templateID, it);
                }
            }
            if (!buildObjectTemplates.ContainsKey(templateID))
            {
                Debug.LogError("Build Object Template not found " + templateID);
                return null;
            }

            return buildObjectTemplates[templateID];
        }

        public int GetBuildingMaterialCount(int materialID)
        {
            if (buildingResources.ContainsKey(materialID))
            {
                return buildingResources[materialID];
            }
            else
            {
                return 0;
            }
        }

        public void ClaimAppeared(GameObject claim)
        {
            claimGameObjects.Add(claim.name,claim);
            claim.SetActive(showClaims);
        }

        public void ClaimRemoved(GameObject claim)
        {
            claimGameObjects.Remove(claim.name);
        }

        public void AddItemPlacedForUpgrade(AtavismInventoryItem item)
        {
            itemsPlacedToUpgrade.Add(item);
        }

        public void RemoveItemPlacedForUpgrade(AtavismInventoryItem item)
        {
            itemsPlacedToUpgrade.Remove(item);
        }

        #region Properties
        public static WorldBuilder Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ClaimListEntry> PlayerClaims
        {
            get
            {
                return playerClaims;
            }
        }
        public List<Claim> Claims
        {
            get
            {
                return claims;
            }
        }

        public Claim ActiveClaim
        {
            get
            {
                return activeClaim;
            }
        }

        public ClaimObject SelectedClaimObject
        {
            get
            {
                return selectedClaimObject;
            }
            set
            {
                
                if(value!=null && ClientAPI.GetTargetObject()!=null)
                    ClientAPI.SetTarget(-1);
                selectedClaimObject = value;
                string[] _args = new string[2];
                //_args[0] = ""+id;
               // _args[1] = ""+claimID;
                AtavismEventSystem.DispatchEvent("OBJECT_TARGET_CHANGED", _args);
            }
        }
        public bool ShowClaims
        {
            get
            {
                return showClaims;
            }
            set
            {
                showClaims = value;
                foreach (GameObject claim in claimGameObjects.Values)
                {
                    if (claim != null)
                    {
                        claim.SetActive(showClaims);
                    }
                }
            }
        }

        public WorldBuildingState BuildingState
        {
            get
            {
                return buildingState;
            }
            set
            {
              //  Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! BuildingState: buildingState="+buildingState+" to "+value);
                buildingState = value;
            }
        }

        public Dictionary<int, AtavismBuildObjectTemplate> BuildObjectTemplates
        {
            get
            {
                return buildObjectTemplates;
            }
        }
        
        public Dictionary<int, OptionChoice> BuildingCategory
        {
            get
            {
                return buildingCategory;
            }
        }
        public String GetBuildingCategory(int id)
        {
            foreach (var cat in buildingCategory.Values)
            {
                if (cat.id == id)
                    return cat.name;
            }

            return "";
        }
        public Dictionary<int, OptionChoice> ClaimTypes
        {
            get
            {
                return claimTypes;
            }
        }

        public int GetAnyClaimType()
        {
            foreach (var ct in claimTypes.Values)
            {
                if (ct.name.Equals("Any"))
                    return ct.id;
            }
            return -1;
        }
        
        public ClaimObject SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                if (selectedObject != null)
                {
                    selectedObject.ResetHighlight();
                }
                if (buildingState == WorldBuildingState.EditItem && selectedObject != value)
                {
                    BuildingState = WorldBuildingState.None;
                }
              /*  if(value!=null)
                   BuilderBehaviour.Instance. ChangeMode(BuildMode.Edition);*/
                selectedObject = value;

       //         Debug.LogError("SelectedObject Set " + value + " ID=" + (value != null ? value.ID + "" : "bd"));
                if (selectedObject != null)
                {
                 //   selectedObject.Highlight();
                    RequestClaimObjectInfo(selectedObject.ID);
                   // if (buildingState == WorldBuildingState.SelectItem)
                   // {
                   BuildingState = WorldBuildingState.EditItem;
                  //  }

                    // Dispatch event to be picked up by UI
                    string[] args = new string[1];
                    args[0] = "";
                    AtavismEventSystem.DispatchEvent("CLAIM_OBJECT_SELECTED", args);
                }
            }
        }

        #endregion Properties

        public const int PERMISSION_INTERACTION = 1;
        public const int PERMISSION_ADD_ONLY = 2;
        public const int PERMISSION_ADD_DELETE = 3;
        public const int PERMISSION_ADD_USERS = 4;
        public const int PERMISSION_MANAGE_USERS = 5;
        public const int PERMISSION_OWNER = 6;
    }
}