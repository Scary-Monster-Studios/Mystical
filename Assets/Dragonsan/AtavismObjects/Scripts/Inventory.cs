using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Atavism
{

    public class Bag
    {
        public int slotNum;
        public int id = 0;
        public bool isActive = false;
        public AtavismInventoryItem itemTemplate;
        public string name = "";
        public Sprite icon = null;
        public int numSlots = 0;
        public Dictionary<int, AtavismInventoryItem> items = new Dictionary<int, AtavismInventoryItem>();
    }

    public class CurrencyDisplay
    {
        public Sprite icon;
        public string name;
        public long amount;
    }

    public class ItemSlot
    {
        public string name = "";
        public string[] type = new string[0]{};
        public override string ToString()
        {
            return "[ItemSlot: name:"+name+" types:"+type+"]";
        }
    }

    public class ItemGroupSlot
    {
        public string name = "";
        public List<ItemSlot> slots = new List<ItemSlot>();
        public bool all = false;
        public override string ToString()
        {
            return "[ItemGroupSlot: name:"+name+" slots:"+slots.Count+"]";
        }
    }
    
    public class Inventory : MonoBehaviour
    {

        static Inventory instance;
        Dictionary<string, EquipmentDisplay> equipmentDisplays;
        Dictionary<int, AtavismInventoryItem> items = new Dictionary<int, AtavismInventoryItem>();
        Dictionary<int, AtavismInventoryItemSet> itemSets = new Dictionary<int, AtavismInventoryItemSet>();
        Dictionary<int, Currency> currencies = new Dictionary<int, Currency>();
        Dictionary<int, AtavismCraftingRecipe> craftingRecipes = new Dictionary<int, AtavismCraftingRecipe>();
        public UGUIAtavismActivatable uguiAtavismItemPrefab;
        public int mainCurrencyGroup = 1;
        public float sellFactor = 0.25f;
        Vector3 storageOpenLoc = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        // These are filled by messages from the server
        Dictionary<int, Bag> bags = new Dictionary<int, Bag>();
        Dictionary<int, AtavismInventoryItem> equippedItems = new Dictionary<int, AtavismInventoryItem>();
        AtavismInventoryItem equippedAmmo;
        Dictionary<int, Bag> storageItems = new Dictionary<int, Bag>();
        Dictionary<int, AtavismInventoryItem> loot = new Dictionary<int, AtavismInventoryItem>();
        Dictionary<int, int> lootCurr = new Dictionary<int, int>();
        Queue<AtavismInventoryItem> pool = new Queue<AtavismInventoryItem>();
        
        
        public Dictionary<string, ItemSlot> itemSlots = new Dictionary<string, ItemSlot>();
        public Dictionary<string, ItemGroupSlot> itemGroupSlots = new Dictionary<string, ItemGroupSlot>();

        OID lootTarget;
        bool itemdataloaded = false;
        bool currencydataloaded = false;
        protected float lootdistance = 5F;
        void Start()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;
            // Listen for messages from the server 
            NetworkAPI.RegisterExtensionMessageHandler("InvPrefabData", HandleInvPrefabData);
            NetworkAPI.RegisterExtensionMessageHandler("InvItemIcon", HandleInvItemIcon);

            NetworkAPI.RegisterExtensionMessageHandler("InvSetsPrefabData", HandleInvSetPrefabData);

            NetworkAPI.RegisterExtensionMessageHandler("CraftRecPrefabData", HandleCraftRecipePrefabData);
            NetworkAPI.RegisterExtensionMessageHandler("CraftingRecipeIcon", HandleCraftingRecipeIcon);

            NetworkAPI.RegisterExtensionMessageHandler("InvCurrPrefabData", HandleInvCurrPrefabData);
            NetworkAPI.RegisterExtensionMessageHandler("InvCurrIcon", HandleInvCurrIcon);

            NetworkAPI.RegisterExtensionMessageHandler("BagInventoryUpdate", HandleBagInventoryUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("EquippedInventoryUpdate", HandleEquippedInventoryUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("BankInventoryUpdate", HandleBankInventoryUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("StorageInventoryUpdate", HandleBankInventoryUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("StorageInventoryClose", HandleCloseBankInventory);
            NetworkAPI.RegisterExtensionMessageHandler("currencies", HandleCurrencies);
            NetworkAPI.RegisterExtensionMessageHandler("LootList", HandleLootList);
            NetworkAPI.RegisterExtensionMessageHandler("inventory_event", HandleInventoryEvent);
            NetworkAPI.RegisterExtensionMessageHandler("inventory_definition", HandleSlotsData);
        }

        private void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("InvPrefabData", HandleInvPrefabData);
            NetworkAPI.RemoveExtensionMessageHandler("InvItemIcon", HandleInvItemIcon);

            NetworkAPI.RemoveExtensionMessageHandler("InvSetsPrefabData", HandleInvSetPrefabData);

            NetworkAPI.RemoveExtensionMessageHandler("CraftRecPrefabData", HandleCraftRecipePrefabData);
            NetworkAPI.RemoveExtensionMessageHandler("CraftingRecipeIcon", HandleCraftingRecipeIcon);

            NetworkAPI.RemoveExtensionMessageHandler("InvCurrPrefabData", HandleInvCurrPrefabData);
            NetworkAPI.RemoveExtensionMessageHandler("InvCurrIcon", HandleInvCurrIcon);

            NetworkAPI.RemoveExtensionMessageHandler("BagInventoryUpdate", HandleBagInventoryUpdate);
            NetworkAPI.RemoveExtensionMessageHandler("EquippedInventoryUpdate", HandleEquippedInventoryUpdate);
            NetworkAPI.RemoveExtensionMessageHandler("BankInventoryUpdate", HandleBankInventoryUpdate);
            NetworkAPI.RemoveExtensionMessageHandler("StorageInventoryUpdate", HandleBankInventoryUpdate);
            NetworkAPI.RemoveExtensionMessageHandler("StorageInventoryClose", HandleCloseBankInventory);
            NetworkAPI.RemoveExtensionMessageHandler("currencies", HandleCurrencies);
            NetworkAPI.RemoveExtensionMessageHandler("LootList", HandleLootList);
            NetworkAPI.RemoveExtensionMessageHandler("inventory_event", HandleInventoryEvent);
            NetworkAPI.RemoveExtensionMessageHandler("inventory_definition", HandleSlotsData);
        
        }

        void ClientReady()
        {
            AtavismLogger.LogDebugMessage("Inventory ClientReady");
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("Item", HandleInvPrefabData);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("ItemIcon", HandleInvItemIcon);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("ItemSet", HandleInvSetPrefabData);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("CraftingRecipe", HandleCraftRecipePrefabData);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("CraftingRecipeIcon", HandleCraftingRecipeIcon);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("Currency", HandleInvCurrPrefabData);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("CurrencyIcon", HandleInvCurrIcon);
        }
        void Update()
        {
            
            if (Input.GetKeyDown(AtavismSettings.Instance.GetKeySettings().loot) && !ClientAPI.UIHasFocus())
            {
                List<AtavismMobNode> list = AtavismClient.Instance.WorldManager.GetMobNodes();
                List<long> oids = new List<long>();
                foreach (var node in list)
                {
                    if (node != null)
                    {
                        if (node.GetProperty("lootable")!=null && (bool)node.GetProperty("lootable") )
                        {
                            float dist = (node.Position - ClientAPI.GetPlayerObject().Position).magnitude;
                            //Debug.LogError("Atavismnode "+node.Oid+" Distance "+dist+" limit "+lootdistance);
                            if (dist < lootdistance)
                            {
                               // Debug.LogError("Atavismnode " + node.Oid + " Loot");
                               // NetworkAPI.SendTargetedCommand(node.Oid, "/lootAll");
                                oids.Add(node.Oid);
                            }
                        }
                    }
                }

                if (oids.Count > 0)
                {
                   // Debug.LogError("Atavismnode  Loot count "+oids.Count);
                    //NetworkAPI.SendTargetedCommand(0L, "/lootAll "+string.Join(" ", oids));
                    Dictionary<string, object> props = new Dictionary<string, object>();
                   // props.Add("oids", oids.ToArray());
                    int i = 0;
                    foreach (var o in oids)
                    {
                        props.Add("o"+i, o);
                        i++;
                    }
                    props.Add("num", i);
                    NetworkAPI.SendExtensionMessage(0, false, "inventory.LOOT_ALL_F", props);
                }
            }
            
            if (lootTarget != null && ClientAPI.GetObjectNode(lootTarget.ToLong()) != null)
            {
                if (Vector3.Distance(ClientAPI.GetPlayerObject().Position, ClientAPI.GetObjectNode(lootTarget.ToLong()).GameObject.transform.position) > 5)
                {
                    lootTarget = null;
                    string[] args = new string[1];
                    AtavismEventSystem.DispatchEvent("CLOSE_LOOT_WINDOW", args);
                }
            }
            if (lootTarget != null && ClientAPI.GetObjectNode(lootTarget.ToLong()) == null)
            {
                lootTarget = null;
                string[] args = new string[1];
                AtavismEventSystem.DispatchEvent("CLOSE_LOOT_WINDOW", args);
            }
            if (storageOpenLoc != new Vector3(float.MinValue, float.MinValue, float.MinValue))
            {
                if(ClientAPI.GetPlayerObject()!=null)
                if (Vector3.Distance(ClientAPI.GetPlayerObject().Position, storageOpenLoc) > 5)
                {
                    storageOpenLoc = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                    string[] args = new string[1];
                    AtavismEventSystem.DispatchEvent("CLOSE_STORAGE_WINDOW", args);
                }
            }
        }

        #region Inventory API
        //Sets
        AtavismInventoryItemSet LoadItemSetPrefabData(int id)
        {
            if (!itemSets.ContainsKey(id))
            {
                AtavismInventoryItemSet it = AtavismPrefabManager.Instance.LoadItemSet(id);
                if (it != null)
                {
                    itemSets.Add(id, it);
                }
            }
            if (!itemSets.ContainsKey(id))
            {
                return null;
            }
            else
            {
                return itemSets[id].Clone();
            }
        }
        public AtavismInventoryItemSet GetItemBySetID(int itemID)
        {
            if (!itemSets.ContainsKey(itemID))
            {
                AtavismInventoryItemSet it = AtavismPrefabManager.Instance.LoadItemSet(itemID);
                if (it != null)
                {
                    itemSets.Add(itemID, it);
                }
            }
            if (itemSets.ContainsKey(itemID))
                return itemSets[itemID];
            return null;
        }

        //Items
        AtavismInventoryItem LoadItemPrefabData(string itemBaseName)
        {
            return AtavismPrefabManager.Instance.LoadItem(itemBaseName);
        }

        public AtavismInventoryItem LoadItemPrefabData(int templateID)
        {
                return AtavismPrefabManager.Instance.LoadItem(templateID);
        }

        public AtavismInventoryItem GetItemByTemplateID(int itemID)
        {
            return AtavismPrefabManager.Instance.LoadItem(itemID);
        }

        public bool DoesPlayerHaveSufficientItems(int templateID, int count)
        {
            int totalCount = 0;
            foreach (Bag bag in bags.Values)
            {
                foreach (AtavismInventoryItem item in bag.items.Values)
                {
                    if (item.templateId == templateID)
                    {
                        totalCount += item.Count;
                        if (totalCount >= count)
                            return true;
                    }
                }
            }
            return false;
        }

        public AtavismInventoryItem GetInventoryItem(int templateID)
        {
            foreach (Bag bag in bags.Values)
            {
                foreach (AtavismInventoryItem item in bag.items.Values)
                {
                    if (item.templateId == templateID)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public AtavismInventoryItem GetInventoryItem(OID itemOID)
        {
            foreach (Bag bag in bags.Values)
            {
                foreach (AtavismInventoryItem item in bag.items.Values)
                {
                    if (item.ItemId == itemOID)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public int GetCountOfItem(int templateID)
        {
            int totalCount = 0;
            foreach (Bag bag in bags.Values)
            {
                foreach (AtavismInventoryItem item in bag.items.Values)
                {
                    if (item.templateId == templateID)
                    {
                        totalCount += item.Count;
                    }
                }
            }
            return totalCount;
        }

        public void PlaceItemInBag(int bagNum, int slotNum, AtavismInventoryItem item, int count)
        {
            PlaceItemInBag(bagNum, slotNum, item, count, false);
        }

        public void PlaceItemInBag(int bagNum, int slotNum, AtavismInventoryItem item, int count, bool swap)
        {
            //Debug.LogError("PlaceItemInBag: bagNum="+bagNum+" slotNum="+slotNum+" itemOid="+item.ItemId+" count="+count+" swap="+swap);
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("bagNum", bagNum);
            props.Add("slotNum", slotNum);
            props.Add("itemOid", item.ItemId);
            props.Add("count", count);
            props.Add("swap", swap);
            NetworkAPI.SendExtensionMessage(0, false, "inventory.MOVE_ITEM", props);
            //ClientAPI.Write("Sending move item");
        }

        public void PlaceItemAsBag(AtavismInventoryItem item, int slot)
        {
            long targetOid = ClientAPI.GetPlayerOid();
            NetworkAPI.SendTargetedCommand(targetOid, "/placeBag " + item.ItemId.ToString() + " " + slot);
        }

        public void MoveBag(int bagSlot, int targetSlot)
        {
            //TODO: Change this to an ExtensionMessage
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/moveBag " + bagSlot + " " + targetSlot);
        }

        public void PlaceBagAsItem(int bagSlot, int bagNum, int slotNum)
        {
            //TODO: Change this to an ExtensionMessage
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/removeBag " + bagSlot + " " + bagNum + " " + slotNum);
        }

        public void DeleteItemWithName(string name)
        {
            AtavismInventoryItem itemToDelete = null;
            foreach (Bag bag in bags.Values)
            {
                foreach (AtavismInventoryItem item in bag.items.Values)
                {
                    if (item.name == name)
                    {
                        itemToDelete = item;
                        break;
                    }
                }
            }

            if (itemToDelete != null)
            {
                long targetOid = ClientAPI.GetPlayerObject().Oid;
                NetworkAPI.SendTargetedCommand(targetOid, "/deleteItem " + itemToDelete.ItemId.ToString());
            }
        }

        public void DeleteItemStack(object item, bool accepted)
        {
            if (accepted)
                DeleteItemStack((AtavismInventoryItem)item);
        }

        public void DeleteItemStack(AtavismInventoryItem item)
        {
            long targetOid = ClientAPI.GetPlayerObject().Oid;
            string itemOid = item.ItemId.ToString();
            NetworkAPI.SendTargetedCommand(targetOid, "/deleteItemStack " + itemOid);
        }

        public List<EquipmentDisplay> LoadEquipmentDisplay(string equipmentDisplayName)
        {
            List<EquipmentDisplay> equipmentDisplays = new List<EquipmentDisplay>();
            int resourcePathPos = equipmentDisplayName.IndexOf("Resources/");
            equipmentDisplayName = equipmentDisplayName.Substring(resourcePathPos + 10);
            equipmentDisplayName = equipmentDisplayName.Remove(equipmentDisplayName.Length - 7);
            GameObject eqPrefab = (GameObject)Resources.Load(equipmentDisplayName);
            if (eqPrefab == null)
            {
                if (Application.isEditor)
                    Debug.LogWarning("Could not load equipment display: " + equipmentDisplayName);

                return null;
            }
            foreach (EquipmentDisplay eqDisplay in eqPrefab.GetComponents<EquipmentDisplay>())
            {
                equipmentDisplays.Add(eqDisplay);
            }
            return equipmentDisplays;
        }

        public Bag GetBagData(int slot)
        {
            if (bags.ContainsKey(slot))
            {
                return bags[slot];
            }
            return null;
        }

        public bool CreateSplitStack(AtavismInventoryItem item, int amount)
        {
            // Check if the item has a count greater than one
            if (item.Count == 1)
            {
                return false;
            }

            // Check where the item is stored - it may be in storage
            if (IsItemInInventory(item))
            {
                // Find a free spot
                int bagNum = -1;
                int slotNum = -1;
                FindFreeInventorySlot(out bagNum, out slotNum);

                if (bagNum != -1)
                {
                    PlaceItemInBag(bagNum, slotNum, item, amount);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (IsItemInStorage(item))
            {
            }
            return false;
        }

        bool IsItemInInventory(AtavismInventoryItem item)
        {
            foreach (Bag bag in bags.Values)
            {
                foreach (AtavismInventoryItem itemInBag in bag.items.Values)
                {
                    if (itemInBag.ItemId == item.ItemId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool FindFreeInventorySlot(out int bagNum, out int slotNum)
        {
            for (int i = 0; i < bags.Count; i++)
            {
                for (int j = 0; j < bags[i].numSlots; j++)
                {
                    if (!bags[i].items.ContainsKey(j))
                    {
                        bagNum = i;
                        slotNum = j;
                        return true;
                    }
                }
            }
            bagNum = -1;
            slotNum = -1;
            return false;
        }

        public void UnequipAmmo(AtavismInventoryItem item)
        {
            if (item != null)
                item.Activate();
            //TODO
        }

        public void EquipItemInSlot(AtavismInventoryItem item, string slotName)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("itemOid", item.ItemId);
            props.Add("slotName", slotName);
            NetworkAPI.SendExtensionMessage(0, false, "ao.EQUIP_ITEM_IN_SLOT", props);
        }

        public void RequestBankInfo()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            NetworkAPI.SendExtensionMessage(0, false, "ao.GET_BANK_ITEMS", props);
        }

        public void PlaceItemInBank(AtavismInventoryItem item, int count)
        {
            for (int i = 0; i < storageItems.Count; i++)
            {
                for (int j = 0; j < storageItems[i].numSlots; j++)
                {
                    if (!storageItems[i].items.ContainsKey(j))
                    {
                        PlaceItemInBank(i, j, item, count, false);
                        return;
                    }
                }
            }
        }

        public void PlaceItemInBank(int bagNum, int slotNum, AtavismInventoryItem item, int count, bool swap)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("bagNum", bagNum);
            props.Add("bankSlot", slotNum);
            props.Add("itemOid", item.ItemId);
            props.Add("count", count);
            props.Add("swap", swap);
            NetworkAPI.SendExtensionMessage(0, false, "ao.STORE_ITEM_IN_BANK", props);
            //ClientAPI.Write("Sending move item");
        }

        public void RetrieveItemFromBank(AtavismInventoryItem item)
        {
            // Find first open space in the players bag
            int containerNum = -1;
            int slotNum = -1;
            if (!FindFreeInventorySlot(out containerNum, out slotNum))
            {
                //TODO: send an error
                return;
            }

            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("containerNum", containerNum);
            props.Add("slotNum", slotNum);
            props.Add("itemOid", item.ItemId);
            props.Add("count", item.Count);
            NetworkAPI.SendExtensionMessage(0, false, "ao.RETRIEVE_ITEM_FROM_BANK", props);
        }

        bool IsItemInStorage(AtavismInventoryItem item)
        {
            foreach (Bag bag in storageItems.Values)
            {
                foreach (AtavismInventoryItem itemInBag in bag.items.Values)
                {
                    if (itemInBag.ItemId == item.ItemId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void StorageClosed()
        {
            storageOpenLoc = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        }

#endregion Inventory API

#region Currency API
        public Currency GetCurrency(int currencyID)
        {
            if (!currencies.ContainsKey(currencyID))
            {
                Currency it = AtavismPrefabManager.Instance.LoadCurrency(currencyID);
                if (it != null)
                {
                    currencies.Add(currencyID, it);
                }
            }
            if (currencies.ContainsKey(currencyID))
            {
                return currencies[currencyID];
            }
            return null;
        }

        public string GetCurrencyName(int currencyID)
        {
            if (!currencies.ContainsKey(currencyID))
            {
                Currency it = AtavismPrefabManager.Instance.LoadCurrency(currencyID);
                if (it != null)
                {
                    currencies.Add(currencyID, it);
                }
            }
            if (currencies.ContainsKey(currencyID))
            {
#if AT_I2LOC_PRESET
   			return I2.Loc.LocalizationManager.GetTranslation(currencies[currencyID].name);
#else
                return currencies[currencyID].name;
#endif
            }
#if AT_I2LOC_PRESET
   			return I2.Loc.LocalizationManager.GetTranslation("Unknown Currency");
#else
            return "Unknown Currency";
#endif
        }

        public int GetCurrencyGroup(int currencyID)
        {
            if (!currencies.ContainsKey(currencyID))
            {
                Currency it = AtavismPrefabManager.Instance.LoadCurrency(currencyID);
                if (it != null)
                {
                    currencies.Add(currencyID, it);
                }
            }
            if (currencies.ContainsKey(currencyID))
            {
                return currencies[currencyID].group;
            }
            return -1;
        }

        public List<Currency> GetMainCurrencies()
        {
            return GetCurrenciesInGroup(mainCurrencyGroup);
        }

        public Currency GetMainCurrency(int pos)
        {
            foreach (Currency c in GetMainCurrencies())
            {
                if (c != null && c.position == pos)
                    return c;
            }
            return null;
        }

        public List<Currency> GetCurrenciesInGroup(int currencyGroup)
        {
          //  Debug.LogError("GetCurrenciesInGroup "+ currencyGroup);
            List<Currency> currenciesInGroup = new List<Currency>();

            foreach (Currency c in currencies.Values)
            {    if (c.group == currencyGroup)
                {
                  //  Debug.LogError("GetCurrenciesInGroup Currency=" + c);
                    while (currenciesInGroup.Count <= c.position)
                    {
                        currenciesInGroup.Add(null);
                    }
                    currenciesInGroup[c.position] = c;
                }
            }

            return currenciesInGroup;
        }

        /// <summary>
        /// Generates a list of currencies from the specified currencyID and amount. This can be
        /// used for drawing currency amounts and icons in the UI.
        /// </summary>
        /// <returns>The currency list from amount.</returns>
        /// <param name="currencyID">Currency ID.</param>
        /// <param name="currencyAmount">Currency amount.</param>
        /// 
        public List<CurrencyDisplay> GenerateCurrencyListFromAmount(int currencyID, long currencyAmount)
        {
            return GenerateCurrencyListFromAmount( currencyID,  currencyAmount, false);
        }



        public List<CurrencyDisplay> GenerateCurrencyListFromAmount(int currencyID, long currencyAmount,bool allFromGroup)
        {
            List<CurrencyDisplay> generatedCurrencyList = new List<CurrencyDisplay>();
            Currency c = GetCurrency(currencyID);
            if (c != null)
            {
                List<Vector2> currencyValues = GetConvertedCurrencyValues(currencyID, currencyAmount, allFromGroup);
                currencyValues.Reverse();
               // Debug.LogError("Inventory GetCost =" + currencyValues.Count);

                for (int i = 0; i < currencyValues.Count; i++)
                {
                  //  Debug.LogError("Inventory GetCost =" + currencyValues[i]);

                    CurrencyDisplay currencyDisplay = new CurrencyDisplay();
                    currencyDisplay.icon = GetCurrency((int)currencyValues[i].x).icon;
                    currencyDisplay.name = GetCurrencyName((int)currencyValues[i].x);
                    currencyDisplay.amount = (long)currencyValues[i].y;
                    generatedCurrencyList.Add(currencyDisplay);
                }
            }
            return generatedCurrencyList;
        }

        public CurrencyDisplay GenerateCurrencyDisplay(int currencyID, long currencyAmount)
        {
            Currency c = GetCurrency(currencyID);
            if (c != null)
            {
                CurrencyDisplay currencyDisplay = new CurrencyDisplay();
                currencyDisplay.icon = GetCurrency(currencyID).icon;
                currencyDisplay.name = GetCurrencyName(currencyID);
                currencyDisplay.amount = currencyAmount;
                return currencyDisplay;
            }
            return null;
        }

        /// <summary>
        /// Creates a readable string of the cost of an item based on the currencyID and amount passed in.
        /// </summary>
        /// <returns>The cost string.</returns>
        /// <param name="currencyID">Currency I.</param>
        /// <param name="currencyAmount">Currency amount.</param>
        public string GetCostString(int currencyID, long currencyAmount)
        {
            Currency c = GetCurrency(currencyID);
            if (c != null)
            {
                List<Vector2> currencyValues = GetConvertedCurrencyValues(currencyID, currencyAmount);
                currencyValues.Reverse();
                string costString = "";
                for (int i = 0; i < currencyValues.Count; i++)
                {
                    if ((int)currencyValues[i].y > 0)
                        costString += (int)currencyValues[i].y + " " + GetCurrencyName((int)currencyValues[i].x) + " ";
                }
                if(costString.Length > 0)
                costString = costString.Remove(costString.Length - 1);
                return costString;
            }
            else
            {
                return currencyAmount.ToString();
            }
        }

        /// <summary>
        /// Splits out a single currency and amount (generally a base currency) and returns the
        /// currencies and amounts it would convert to.
        /// </summary>
        /// <returns>The converted currency values.</returns>
        /// <param name="currencyID">Currency I.</param>
        /// <param name="currencyAmount">Currency amount.</param>
        /// 
        public List<Vector2> GetConvertedCurrencyValues(int currencyID, long currencyAmount)
        {
            return GetConvertedCurrencyValues(currencyID, currencyAmount, false);
        }


        public List<Vector2> GetConvertedCurrencyValues(int currencyID, long currencyAmount,bool allFromGroup)
        {
            List<Vector2> currencyValues = new List<Vector2>();
            Currency c = GetCurrency(currencyID);
            if (c.convertsTo > 0 && c.conversionAmountReq > 0 && currencyAmount >= c.conversionAmountReq && !allFromGroup)
            {
             //   Debug.LogError("GetConvertedCurrencyValues | "+c.id+" " + (currencyAmount % c.conversionAmountReq));
                currencyValues.Add(new Vector2(c.id, currencyAmount % c.conversionAmountReq));
                while (true)
                {
                    currencyAmount = currencyAmount / c.conversionAmountReq;
              //      Debug.LogError("GetConvertedCurrencyValues || "+c.id+" " + currencyAmount);
                    c = GetCurrency(c.convertsTo);
                    if (c == null)
                    {
                    //    Debug.LogError("GetConvertedCurrencyValues currency is null " );

                        break;
                    }
                 //   Debug.LogError("GetConvertedCurrencyValues currencyAmount=" + currencyAmount + " c.conversionAmountReq=" + c.conversionAmountReq);
                    if (c.conversionAmountReq > 1 && currencyAmount >= (long)c.conversionAmountReq)
                    {
                        currencyValues.Add(new Vector2(c.id, currencyAmount % c.conversionAmountReq));
                   //     Debug.LogError("GetConvertedCurrencyValues ||| " + c.id+" "+(currencyAmount % c.conversionAmountReq));
                    }
                    else
                    {
                        currencyValues.Add(new Vector2(c.id, currencyAmount));
                  //      Debug.LogError("GetConvertedCurrencyValues |||| "+ c.id+" " + currencyAmount);
                        break;
                    }
                }
            } else if (c.convertsTo > 0 && c.conversionAmountReq > 0 && allFromGroup)
            {
              //  Debug.LogError("GetConvertedCurrencyValues| | " + (currencyAmount % c.conversionAmountReq));
                currencyValues.Add(new Vector2(c.id, currencyAmount % c.conversionAmountReq));
                while (true)
                {
                    if(c.conversionAmountReq > 0)
                        currencyAmount = currencyAmount / c.conversionAmountReq;
                //    Debug.LogError("GetConvertedCurrencyValues| || " + currencyAmount);
                    c = GetCurrency(c.convertsTo);
                    if (c == null)
                        break;
                    if (c.conversionAmountReq > 1 && currencyAmount >= c.conversionAmountReq)
                    {
                        currencyValues.Add(new Vector2(c.id, currencyAmount % c.conversionAmountReq));
                  //      Debug.LogError("GetConvertedCurrencyValues| ||| " + (currencyAmount % c.conversionAmountReq));
                    }
                    else
                    {
                        currencyValues.Add(new Vector2(c.id, currencyAmount));
                   //     Debug.LogError("GetConvertedCurrencyValues| |||| " + currencyAmount);
                        //  break;
                    }
                }
            }
            else
            {
                currencyValues.Add(new Vector2(c.id, currencyAmount));
            }



            return currencyValues;
        }

        /// <summary>
        /// Converts the currencies to base currency. Assumes all currencies convert back to the same base currency
        /// </summary>
        /// <param name="currencies">Currencies.</param>
        /// <param name="currencyID">Currency I.</param>
        /// <param name="currencyAmount">Currency amount.</param>
        public void ConvertCurrenciesToBaseCurrency(List<Vector2> currencies, out int currencyID, out long currencyAmount)
        {
            currencyID = -1;
            currencyAmount = 0;
            // Loop through each currency
            foreach (Vector2 currencyInfo in currencies)
            {
                int currencyOutID;
                long currencyOutAmount;
                ConvertCurrencyToBaseCurrency((int)currencyInfo.x, (int)currencyInfo.y, out currencyOutID, out currencyOutAmount);
                if (currencyOutID == currencyID || currencyID == -1)
                {
                    currencyID = currencyOutID;
                    currencyAmount += currencyOutAmount;
                }
            }
        }
        /// <summary>
        /// Converts the currencies to base currency. Assumes all currencies convert back to the same base currency
        /// </summary>
        /// <param name="currencies">Currencies.</param>
        /// <param name="currencyID">Currency I.</param>
        /// <param name="currencyAmount">Currency amount.</param>
        public void ConvertCurrenciesToBaseCurrency(Dictionary<int,long> currencies, out int currencyID, out long currencyAmount)
        {
            currencyID = -1;
            currencyAmount = 0;
            // Loop through each currency
            foreach (int currencyId in currencies.Keys)
            {
                int currencyOutID;
                long currencyOutAmount;
                ConvertCurrencyToBaseCurrency(currencyId, currencies[currencyId], out currencyOutID, out currencyOutAmount);
                if (currencyOutID == currencyID || currencyID == -1)
                {
                    currencyID = currencyOutID;
                    currencyAmount += currencyOutAmount;
                }
            }
        }

        /// <summary>
        /// Converts the currency down to the base currency.
        /// </summary>
        /// <param name="currencyInID">Currency in ID.</param>
        /// <param name="currencyInAmount">Currency in amount.</param>
        /// <param name="currencyOutID">Currency out ID.</param>
        /// <param name="currencyOutAmount">Currency out amount.</param>
        public void ConvertCurrencyToBaseCurrency(int currencyInID, long currencyInAmount, out int currencyOutID, out long currencyOutAmount)
        {
            // Find a currency that converts to this currency
            foreach (Currency c in currencies.Values)
            {
                if (c.convertsTo == currencyInID)
                {
                    currencyOutID = c.id;
                    currencyOutAmount = c.conversionAmountReq * currencyInAmount;
                    // Check if there is a currency that converts into this new currency
                    foreach (Currency cu in currencies.Values)
                    {
                        if (c.convertsTo == currencyInID)
                        {
                            ConvertCurrencyToBaseCurrency(currencyOutID, currencyOutAmount, out currencyOutID, out currencyOutAmount);
                        }
                    }
                    return;
                }
            }
            currencyOutID = currencyInID;
            currencyOutAmount = currencyInAmount;
        }

        /// <summary>
        /// Checks if the player has enough currency to match the given list of currencies.
        /// </summary>
        /// <returns><c>true</c>, if player has enough currency  <c>false</c> otherwise.</returns>
        /// <param name="currencies">Currencies.</param>
        public bool DoesPlayerHaveEnoughCurrency(List<Vector2> currencies)
        {
            // Convert both the given amounts and the actual curreny amount the player has down to the base currency
            int givenCurrencyID;
            long givenCurrencyAmount;
            ConvertCurrenciesToBaseCurrency(currencies, out givenCurrencyID, out givenCurrencyAmount);

            int currencyGroup = GetCurrencyGroup(givenCurrencyID);
            if (currencyGroup == -1)
                return false;

            int playerBaseCurrencyID;
            long playerCurrencyAmount = GetPlayerBaseCurrencyAmount(currencyGroup, out playerBaseCurrencyID);

            if (givenCurrencyAmount <= playerCurrencyAmount)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the total amount of base currency for the given currency group.
        /// </summary>
        /// <returns>The player base currency amount.</returns>
        /// <param name="currencyID">Currency I.</param>
        public long GetPlayerBaseCurrencyAmount(int currencyGroup, out int baseCurrencyID)
        {
            List<Currency> playerCurrencies = GetCurrenciesInGroup(currencyGroup);
            List<Vector2> playerCurrenciesList = new List<Vector2>();
            foreach (Currency c in playerCurrencies)
            {
                playerCurrenciesList.Add(new Vector2(c.id, c.Current));
            }

            long playerCurrencyAmount;
            ConvertCurrenciesToBaseCurrency(playerCurrenciesList, out baseCurrencyID, out playerCurrencyAmount);
            return playerCurrencyAmount;
        }

#endregion Currency API
#region Gear Modification

        public void EmbedInTheSlot(AtavismInventoryItem item, AtavismInventoryItem socketItem)
        {
            if (item != null && socketItem != null)
            {
                AtavismLogger.LogDebugMessage("EmbedInTheSlot");
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("socketItemOid", socketItem.ItemId);
                props.Add("itemOid", item.ItemId);
                NetworkAPI.SendExtensionMessage(0, false, "inventory.INSERT_TO_SOCKET", props);
                AtavismLogger.LogDebugMessage("EmbedInTheSlot send " + props);
            }
        }
        public void SocketingCost(AtavismInventoryItem item, AtavismInventoryItem socketItem)
        {
            if (item != null && socketItem != null)
            {
                AtavismLogger.LogDebugMessage("SocketingCost");
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("socketItemOid", socketItem.ItemId);
                props.Add("itemOid", item.ItemId);
                NetworkAPI.SendExtensionMessage(0, false, "inventory.SOCKETING_DETAIL", props);
                AtavismLogger.LogDebugMessage("SocketingCost send " + props);
            }
        }
        //Reset
        public void ResetSloctsSlot(AtavismInventoryItem item)
        {
            if (item != null)
            {
                AtavismLogger.LogDebugMessage("ResetSloctsSlot");
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("itemOid", item.ItemId);
                NetworkAPI.SendExtensionMessage(0, false, "inventory.SOCKETING_RESET", props);
                AtavismLogger.LogDebugMessage("ResetSloctsSlot send " + props);
            }
        }
        public void ResetSocketsCost(AtavismInventoryItem item)
        {
            if (item != null)
            {
                AtavismLogger.LogDebugMessage("ResetSocketsCost");
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("itemOid", item.ItemId);
                NetworkAPI.SendExtensionMessage(0, false, "inventory.SOCKETING_RESET_DETAIL", props);
                AtavismLogger.LogDebugMessage("ResetSocketsCost send " + props);
            }
        }
        //Enchant
        public void EnchantItem(AtavismInventoryItem item)
        {
            if (item != null)
            {
                AtavismLogger.LogDebugMessage("EnchantItem");
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("itemOid", item.ItemId);
                NetworkAPI.SendExtensionMessage(0, false, "inventory.ENCHANT", props);
                AtavismLogger.LogDebugMessage("EnchantItem send " + props);
            }
        }
        public void EnchantCost(AtavismInventoryItem item)
        {
            if (item != null)
            {
                AtavismLogger.LogDebugMessage("EnchantCost");
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("itemOid", item.ItemId);
                NetworkAPI.SendExtensionMessage(0, false, "inventory.ENCHANTING_DETAIL", props);
                AtavismLogger.LogDebugMessage("EnchantCost send " + props);
            }
        }




#endregion Gear Modification
        public AtavismCraftingRecipe GetCraftingRecipe(int recipeID)
        {
            if (!craftingRecipes.ContainsKey(recipeID))
            {
                AtavismCraftingRecipe it = AtavismPrefabManager.Instance.LoadCraftingRecipe(recipeID);
                if (it != null)
                {
                    craftingRecipes.Add(recipeID, it);
                }
            }
            if (craftingRecipes.ContainsKey(recipeID))
            {
                return craftingRecipes[recipeID];
            }
            return null;
        }

        public List<AtavismCraftingRecipe> GetCraftingRecipeMatch(List<int> list, int skillId)
        {
            List<AtavismCraftingRecipe> _list = new List<AtavismCraftingRecipe>();
            foreach (int recId in list)
            {
                if (!craftingRecipes.ContainsKey(recId))
                {
                    AtavismCraftingRecipe it = AtavismPrefabManager.Instance.LoadCraftingRecipe(recId);
                    if (it != null)
                    {
                        craftingRecipes.Add(recId, it);
                    }
                }
                //  int _recId = int.Parse(recId);
                if (craftingRecipes.ContainsKey(recId))
                {
                    if (craftingRecipes[recId].skillID.Equals(skillId))
                    {
                        _list.Add(craftingRecipes[recId]);
                    }
                }
            }
            return _list;
        }
        public List<int> GetCraftingRecipeMatch(List<int> list, string station)
        {
            List<int> _list = new List<int>();
            foreach (int recId in list)
            {
                if (!craftingRecipes.ContainsKey(recId))
                {
                    AtavismCraftingRecipe it = AtavismPrefabManager.Instance.LoadCraftingRecipe(recId);
                    if (it != null)
                    {
                        craftingRecipes.Add(recId, it);
                    }
                }
                //  int _recId = int.Parse(recId);
                if (craftingRecipes.ContainsKey(recId))
                {
                    //    Debug.LogError("GetCraftingRecipeMatch " + craftingRecipes[recId].stationReq + " station:" + station);
                    if (craftingRecipes[recId].stationReq.Equals(station) || craftingRecipes[recId].stationReq.Equals("~ none ~") || craftingRecipes[recId].stationReq.Equals("none"))
                    {
                        //     Debug.LogError("GetCraftingRecipeMatch add skill :"+craftingRecipes[recId].skillID+ " _list:"+ _list);

                        if (!_list.Contains(craftingRecipes[recId].skillID))
                            _list.Add(craftingRecipes[recId].skillID);
                    }
                    if (station.Equals(""))
                    {
                        if (!_list.Contains(craftingRecipes[recId].skillID))
                            _list.Add(craftingRecipes[recId].skillID);
                    }
                }
            }
            return _list;
        }



        public Dictionary<int, int> GetItemGruped()
        {
            Dictionary<int, int> groupitem = new Dictionary<int, int>();
            //     int it = 0;
            for (int i = 0; i < bags.Count; i++)
            {
                for (int k = 0; k < bags[i].numSlots; k++)
                {
                    if (bags[i].items.ContainsKey(k))
                    {
                        if (!bags[i].items[k].isBound && bags[i].items[k].sellable)
                        {

                            if (groupitem.ContainsKey(bags[i].items[k].templateId))
                            {
                                groupitem[bags[i].items[k].templateId] += bags[i].items[k].Count;
                            }
                            else
                            {
                                groupitem.Add(bags[i].items[k].templateId, bags[i].items[k].Count);
                            }
                            //                        it++;
                        }

                    }

                }

            }
            return groupitem;
        }

        public List<string> GetWeaponSlots()
        {
            List<string> list = new List<string>();
            foreach (var slot in itemSlots.Values)
            {
                if(slot.type.Contains("Weapon"))
                    list.Add(slot.name);
            }

            return list;
        }

#region Message Handlers

private void HandleSlotsData(Dictionary<string, object> props)
{
    try
    {
        int numSlots = (int) props["num"];
        int lDist = (int) props["lDist"];
        lootdistance = lDist / 1000f;
        AtavismLogger.LogDebugMessage("Got Slots list with num : " + numSlots);
        for (int i = 0; i < numSlots; i++)
        {
            string name = (string) props["sN" + i];
            //  string socket = (string) props["sS" + i];
            string type = (string) props["sT" + i];
            ItemSlot slot = new ItemSlot();
            slot.name = name;
           // Debug.LogError("HandleSlotsData: name="+slot.name+" type="+type );
            string[] types = type.Split(';');
            slot.type = types;
            itemSlots[name] = slot;
        }

        int numSlotsGroup = (int) props["gnum"];
        AtavismLogger.LogDebugMessage("Got Slots Group list with num : " + numSlotsGroup);
        for (int i = 0; i < numSlotsGroup; i++)
        {
            string groupName = (string) props["gs" + i];
            bool all = (bool) props["gsa" + i];
            int numSlotsInGroup = (int) props["gs" + i + "num"];
          //  Debug.LogError("HandleSlotsData: groupName="+groupName+" numSlotsInGroup="+numSlotsInGroup +" all="+all);

            ItemGroupSlot igs = new ItemGroupSlot();
            igs.all = all;
            igs.name = groupName;
            for (int j = 0; j < numSlotsInGroup; j++)
            {
                string slotName = (string) props["gs" + i + "s" + j];
                igs.slots.Add(itemSlots[slotName]);
            }

            itemGroupSlots[groupName] = igs;
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError("HandleSlotsData  Exception:" + e);
    }
}

public void HandleInvCurrIcon(Dictionary<string, object> props)
        {
          //  Debug.LogError("HandleInvCurrPrefabData " + Time.time);
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
                    try
                    {
                        bool wyn = tex.LoadImage(System.Convert.FromBase64String(icon2));
                        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

                        AtavismPrefabManager.Instance.SaveCurrencyIcon(id, sprite, icon2);
                        if (currencies.ContainsKey(id))
                        {
                            currencies[id].icon = sprite;
                        }
                    }
                    catch (System.Exception e)
                    {
                        AtavismLogger.LogError("Exception loading currency icon prefab for "+id +" "+ e);
                    }
                }
                string[] args = new string[1];
                AtavismEventSystem.DispatchEvent("CURRENCY_ICON_UPDATE", args);
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading currency icon prefab data " + e);
            }
         //   Debug.LogError("HandleInvCurrPrefabData End");
        }

        public void HandleInvCurrPrefabData(Dictionary<string, object> props)
        {
       //     Debug.LogError("HandleInvCurrPrefabData " + Time.time);
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                 //   Debug.LogError("HandleInvCurrPrefabData " + i);
                    CurrencyPrefabData cpd = new CurrencyPrefabData();
                    cpd.id = (int)props["i" + i + "id"];
                    cpd.name = (string)props["i" + i + "name"];
                    cpd.description = (string)props["i" + i + "desc"];
                    cpd.max = (long)props["i" + i + "max"];
                    cpd.group = (int)props["i" + i + "group"];
                    cpd.position = (int)props["i" + i + "pos"];
                    cpd.date = (long)props["i" + i + "date"];
                    cpd.convertsTo = (int)props["i" + i + "convId"];
                    cpd.conversionAmountReq = (int)props["i" + i + "convAmo"];
                    AtavismPrefabManager.Instance.SaveCurrency(cpd);
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
                                AtavismPrefabManager.Instance.DeleteCurrency(int.Parse(k));
                            }
                        }
                    }
                }

                if (sendAll)
                {
                    currencydataloaded = true;

                    currencies.Clear();
                    List<Currency> list = AtavismPrefabManager.Instance.LoadAllCurrency();
                    foreach (Currency c in list)
                    {
                        currencies.Add(c.id, c);
                    }
                    // dispatch a ui event to tell the rest of the system
                          foreach (Dictionary<string, object> qProps in currMsgQueue)
                        {
                        //    Debug.LogError("Running Queued Item update message");
                            HandleCurrencies(qProps);
                        }
                
                    string[] args = new string[1];
                    AtavismEventSystem.DispatchEvent("CURRENCY_UPDATE", args);
                    AtavismPrefabManager.Instance.reloaded++;


                    if(AtavismLogger.logLevel <= LogLevel.Debug) 
                    Debug.Log("All data received. Running Queued Currency update message.");
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadCurrencyPrefabData();
                    Debug.LogWarning("Not Running Queued Item update message. Not all data sent");
                }
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading currency prefab data " + e);
            }
          //  Debug.LogError("HandleInvCurrPrefabData End");
        }


        public void HandleInvSetPrefabData(Dictionary<string, object> props)
        {
          //  Debug.LogError("HandleInvSetPrefabData " + Time.time);
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                  //  Debug.LogError("HandleInvSetPrefabData " + i);
                    ItemSetPrefabData cpd = new ItemSetPrefabData();
                    cpd.Setid = (int)props["i" + i + "id"];
                    cpd.Name = (string)props["i" + i + "name"];
                    string its = (string)props["i" + i + "items"];
                    string[] its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if(s.Length>0)
                        cpd.itemList.Add(int.Parse(s));
                    }
                    int lnum = (int)props["i" + i + "lnum"];
                    for (int l = 0; l < lnum; l++)
                    {
                        AtavismInventoryItemSetLevel aiisl = new AtavismInventoryItemSetLevel();
                        aiisl.NumerOfParts = (int)props["i" + i + "l" + l + "numParts"];
                        int snum = (int)props["i" + i + "l" + l + "snum"];
                        for (int s = 0; s < snum; s++)
                        {
                            string sn = (string)props["i" + i + "l" + l + "s" + s + "n"];
                            if (!sn.Equals("dmg-base") && !sn.Equals("dmg-max"))
                            {
                                aiisl.itemStatName.Add(sn);
                                aiisl.itemStatValues.Add((int)props["i" + i + "l" + l + "s" + s + "v"]);
                                aiisl.itemStatValuesPercentage.Add((int)(((int)props["i" + i + "l" + l + "s" + s + "p"])/1000f));
                            }
                            if (sn.Equals("dmg-base"))
                            {
                                aiisl.DamageValue = (int)props["i" + i + "l" + l + "s" + s + "v"];
                                aiisl.DamageValuePercentage = (int)(((int)props["i" + i + "l" + l + "s" + s + "p"])/1000f);
                            }
                        }
                        cpd.levelList.Add(aiisl);

                    }

                    cpd.date = (long)props["i" + i + "date"];
                    AtavismPrefabManager.Instance.SaveItemSet(cpd);
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
                                AtavismPrefabManager.Instance.DeleteItemSet(int.Parse(k));
                            }
                        }
                    }
                }

                if (sendAll)
                {
                    //craftingRecipes.Clear();
                    List<AtavismInventoryItemSet> list = AtavismPrefabManager.Instance.LoadAllItemSet();
                    itemSets.Clear();
                    foreach (AtavismInventoryItemSet c in list)
                    {
                        itemSets.Add(c.Setid, c);
                    }
                    AtavismPrefabManager.Instance.reloaded++;


                    if(AtavismLogger.logLevel <= LogLevel.Debug) 
                    Debug.Log("All data received. Running Queued Item Sets update message.");
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadItemSetPrefabData();
                    Debug.LogWarning("Not Running Queued Item update message. Not all data sent");
                }
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading itemset  prefab data " + e);
            }
        //    Debug.LogError("HandleInvSetPrefabData End");
        }

        public void HandleCraftingRecipeIcon(Dictionary<string, object> props)
        {
         //   Debug.LogError("HandleCraftingRecipeIcon " + Time.time);
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

                    AtavismPrefabManager.Instance.SaveCrafringRecipeIcon(id, sprite, icon2);
                    if (craftingRecipes.ContainsKey(id))
                    {
                        craftingRecipes[id].icon = sprite;
                    }
                }
                string[] args = new string[1];
                AtavismEventSystem.DispatchEvent("CRAFTING_RECIPE_UPDATE", args);
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading currency icon prefab data " + e);
            }
        //    Debug.LogError("HandleCraftingRecipeIcon End");
        }
        public void HandleCraftRecipePrefabData(Dictionary<string, object> props)
        {
          //  Debug.LogError("HandleCraftRecipePrefabData " + Time.time);
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                 //   Debug.LogError("HandleCraftRecipePrefabData " + i);
                    CraftingRecipePrefabData cpd = new CraftingRecipePrefabData();
                    cpd.recipeID = (int)props["i" + i + "id"];
                    cpd.recipeName = (string)props["i" + i + "name"];
                    cpd.skillID = (int)props["i" + i + "skillid"];
                    cpd.skillLevelReq = (int)props["i" + i + "skillreqlev"];
                    cpd.stationReq = (string)props["i" + i + "statreq"];
                    cpd.creationTime = (int)props["i" + i + "ctime"];

                    string its = (string)props["i" + i + "items1"];
                    string[] its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if(s.Length>0)
                        cpd.createsItems.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "items2"];
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.createsItems2.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "items3"];
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.createsItems3.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "items4"];
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.createsItems4.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "items1c"];
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.createsItemsCounts.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "items2c"];
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.createsItemsCounts2.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "items3c"];
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.createsItemsCounts3.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "items4c"];
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.createsItemsCounts4.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "itemsreq"];
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.itemsReq.Add(int.Parse(s));
                    }
                    its = (string)props["i" + i + "itemsreqc"];
                    its2 = its.Split(';');
                    foreach (string s in its2)
                    {
                        if (s.Length > 0)
                            cpd.itemsReqCounts.Add(int.Parse(s));
                    }





                    cpd.date = (long)props["i" + i + "date"];

                    AtavismPrefabManager.Instance.SaveCrafringRecipe(cpd);
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
                                AtavismPrefabManager.Instance.DeleteCrafringRecipe(int.Parse(k));
                            }
                        }
                    }
                }

                if (sendAll)
                {

                    craftingRecipes.Clear();
                    List<AtavismCraftingRecipe> list = AtavismPrefabManager.Instance.LoadAllCraftingRecipe();
                    foreach (AtavismCraftingRecipe c in list)
                    {
                        craftingRecipes.Add(c.recipeID, c);
                    }
                    AtavismPrefabManager.Instance.reloaded++;

                    if(AtavismLogger.logLevel <= LogLevel.Debug) 
                    Debug.Log("All data received. Running Queued Crafting Recipes update message.");
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadCraftingRecipePrefabData();
                    Debug.LogWarning("Not Running Queued Item update message. Not all data sent");
                }
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading crafting recipe prefab data " + e);
            }
        //    Debug.LogError("HandleCraftRecipePrefabData End");
        }

     
        List<Dictionary<string, object>> itemMsgQueue = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> itemEqMsgQueue = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> currMsgQueue = new List<Dictionary<string, object>>();

        public void HandleInvItemIcon(Dictionary<string, object> props)
        {
            //  Debug.LogError("HandleInvItemIcon " + Time.time);
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

                    AtavismPrefabManager.Instance.SaveItemsIcon(id, sprite, icon2);
                  /*  if (items.ContainsKey(id))
                    {
                        items[id].icon = sprite;
                    }*/
                    foreach (AtavismInventoryItem aii in equippedItems.Values)
                    {
                        if (aii.templateId == id)
                            aii.icon = sprite;
                    }
                    for (int b = 0; b < bags.Values.Count; b++)
                    {
                        if (bags[b].itemTemplate != null)
                            if (bags[b].itemTemplate.templateId == id)
                                bags[b].itemTemplate.icon = sprite;
                        foreach (AtavismInventoryItem aii in bags[b].items.Values)
                        {
                            if (aii.templateId == id)
                                aii.icon = sprite;
                        }
                    }

                    for (int b = 0; b < storageItems.Count; b++)
                    {
                        if (bags[b].itemTemplate != null)
                            if (bags[b].itemTemplate.templateId == id)
                                bags[b].itemTemplate.icon = sprite;
                        foreach (AtavismInventoryItem aii in storageItems[b].items.Values)
                        {
                            if (aii.templateId == id)
                                aii.icon = sprite;
                        }
                    }
                        foreach (AtavismInventoryItem aii in loot.Values)
                        {
                            if (aii.templateId == id)
                                aii.icon = sprite;
                        }
    
                }
                string[] args = new string[1];
                AtavismEventSystem.DispatchEvent("INVENTORY_UPDATE", args);
                AtavismEventSystem.DispatchEvent("QUEST_LOG_UPDATE", args);
                AtavismEventSystem.DispatchEvent("ITEM_ICON_UPDATE", args);
                if (loot.Count > 0)
                {
                    AtavismEventSystem.DispatchEvent("LOOT_UPDATE", args);
                }

            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading currency icon prefab data " + e);
            }
         //   Debug.LogError("HandleInvItemIcon End");
        }

        public void HandleInvPrefabData(Dictionary<string, object> props)
        {
        //    Debug.LogError("HandleInvPrefabData "+Time.time);
        int tmplId = 0;
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                  //  Debug.LogError("HandleInvPrefabData "+i);
                    ItemPrefabData ipd = new ItemPrefabData();
                    ipd.templateId = (int)props["i" + i + "id"];
                    tmplId = ipd.templateId;
                    ipd.name = (string)props["i" + i + "name"];
                    ipd.tooltip = (string)props["i" + i + "tooltip"];
                    ipd.itemType = (string)props["i" + i + "itemType"];
                    ipd.subType = (string)props["i" + i + "subType"];

                    ipd.slot = (string)props["i" + i + "slot"];
                    ipd.quality = (int)props["i" + i + "quality"];
                    ipd.currencyType = (int)props["i" + i + "currType"];
                    ipd.cost = (long)props["i" + i + "cost"];

                    ipd.binding = (int)props["i" + i + "binding"];
                    ipd.sellable = (bool)props["i" + i + "sellable"];

                    ipd.damageValue = (int)props["i" + i + "dValue"];
                    ipd.damageMaxValue = (int)props["i" + i + "dMValue"];

                    ipd.setId = (int)props["i" + i + "setId"];
                    ipd.enchantId = (int)props["i" + i + "enchId"];

                    ipd.weaponSpeed = (int)props["i" + i + "wSpeed"];
                    ipd.stackLimit = (int)props["i" + i + "sLimit"];

                    ipd.auctionHouse = (bool)props["i" + i + "aH"];
                    ipd.unique = (bool)props["i" + i + "unique"];

                    ipd.sockettype = (string)props["i" + i + "sockettype"];
                    ipd.durability = (int)props["i" + i + "durability"];
                    ipd.weight = (int)props["i" + i + "weight"];
                    ipd.autoattack = (int)props["i" + i + "autoattack"];
                    ipd.deathLoss = (int)props["i" + i + "deathLoss"];
                    ipd.ammoType = (int)props["i" + i + "ammotype"];
                    ipd.parry = (bool)props["i" + i + "parry"];
                    ipd.repairable = (bool)props["i" + i + "repairable"];


                    ipd.gear_score = (int)props["i" + i + "gear_score"];

                    int efnum = (int)props["i" + i + "enum"];
                    for (int k = 0; k < efnum; k++)
                    {
                        ipd.itemEffectTypes.Add((string)props["i" + i + "eT" + k]);
                        ipd.itemEffectNames.Add((string)props["i" + i + "eN" + k]);
                        ipd.itemEffectValues.Add((string)props["i" + i + "eV" + k]);

                    }
                    int rnum = (int)props["i" + i + "rnum"];
                    for (int k = 0; k < rnum; k++)
                    {
                        ipd.itemReqTypes.Add((string)props["i" + i + "rT" + k]);
                        ipd.itemReqNames.Add((string)props["i" + i + "rN" + k]);
                        ipd.itemReqValues.Add((string)props["i" + i + "rV" + k]);
                    }
                    ipd.date = (long)props["i" + i + "date"];

                    string its = (string)props["i" + i + "reqType"];
                    if (its.Length > 0)
                    {
                        string[] its2 = its.Split(';');
                        foreach (string s in its2)
                        {
                            ipd.itemReqTypes.Add(s);
                        }
                    }
                    its = (string)props["i" + i + "reqNames"];
                    if (its.Length > 0)
                    {
                        string[] its2 = its.Split(';');
                        foreach (string s in its2)
                        {
                            ipd.itemReqNames.Add(s);
                        }
                    }
                    its = (string)props["i" + i + "reqValues"];
                    if (its.Length > 0)
                    {
                        string[] its2 = its.Split(';');
                        foreach (string s in its2)
                        {
                            ipd.itemReqValues.Add(s);
                        }
                    }
                  
                    AtavismPrefabManager.Instance.SaveItem(ipd);
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
                                AtavismPrefabManager.Instance.DeleteItem(int.Parse(k));
                            }
                        }
                    }
                }
                
                if (sendAll)
                {
                  //  Debug.LogError("Item prefab loaded");
                    itemdataloaded = true;
                    foreach (Dictionary<string, object> qProps in itemMsgQueue)
                    {
                  //      Debug.LogError("Running Queued Item update message");
                        HandleBagInventoryUpdate(qProps);
                    }
                    foreach (Dictionary<string, object> qProps in itemEqMsgQueue)
                    {
                      //  Debug.LogError("Running Queued Item Equip update message");
                        HandleEquippedInventoryUpdate(qProps);
                    }
                    string[] args = new string[1];
                    AtavismEventSystem.DispatchEvent("ITEM_RELOAD", args);

                    if(AtavismLogger.logLevel <= LogLevel.Debug) 
                        Debug.Log("All data received. Running Queued Item update message.");
                    AtavismPrefabManager.Instance.reloaded++;
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadItemPrefabData();
                    Debug.LogWarning("Not Running Queued Item update message. Not all data sent");
                }
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading item prefab data item id="+tmplId+" Exception:" + e);
            }
         //   Debug.LogError("HandleInvPrefabData End");
        }

        public void HandleBagInventoryUpdate(Dictionary<string, object> props)
        {
            //  Debug.LogWarning("HandleBagInventoryUpdate");

            if (!itemdataloaded)
            {
                AtavismPrefabManager.Instance.LoadItemPrefabData();
                itemMsgQueue.Add(props);
                Debug.LogWarning("HandleBagInventoryUpdate item definition not loaded add queue");
                return;
            }
            List<int> iconLack = new List<int>();
         //   Debug.LogWarning("HandleBagInventoryUpdate");

            string keys = " [ ";
            foreach (string it in props.Keys)
            {
                if (!it.Contains("icon2"))
                    keys += " ; " + it + " => " + props[it] + "\n";
                if (keys.Length > 10000)
                {

                   // Debug.LogWarning("HandleBagInventoryUpdate: keys:" + keys);
                    keys = "";
                }
            }
               //  Debug.LogWarning("HandleBagInventoryUpdate: keys:" + keys);
                 

            foreach (Bag b in bags.Values)
            {
                if (b.itemTemplate != null)
                {
                    pool.Enqueue(b.itemTemplate);
                    //   DestroyImmediate(b.itemTemplate);
                }
                foreach (AtavismInventoryItem aii in b.items.Values)
                {
                        pool.Enqueue(aii);
                    //   DestroyImmediate(aii);
                }
            }
            bags.Clear();
            try
            {

                sellFactor = (float)props["SellFactor"];
                int numBags = (int)props["numBags"];
                for (int i = 0; i < numBags; i++)
                {
                    Bag bag = new Bag();
                    bag.id = (int)props["bag_" + i + "ID"];
                    bag.name = (string)props["bag_" + i + "Name"];
                    AtavismInventoryItem invInfo = LoadItemPrefabData(bag.name);
                    
                    bag.itemTemplate = invInfo;
                    if (invInfo != null)
                    {
                        if (invInfo.icon == null)
                            iconLack.Add(invInfo.templateId);
                        if (invInfo.icon != null)
                            bag.icon = invInfo.icon;
                        else
                            bag.icon = AtavismSettings.Instance.defaultItemIcon;

                       // bag.icon = invInfo.icon;
                    }
                    AtavismLogger.LogDebugMessage("Got bag with name: " + bag.name);
                    bag.numSlots = (int)props["bag_" + i + "NumSlots"];
                    if (bag.numSlots == 0)
                    {
                        bag.isActive = false;
                    }
                    else
                    {
                        bag.isActive = true;
                    }
                    bag.slotNum = i;
                    //CSVReader.loadBagData(bag);
                    bags.Add(i, bag);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("HandleBagInventoryUpdate Bag Exception:" + e);
            }
            int tmplId = -1;
            try
            {
                int numItems = (int)props["numItems"];
                AtavismLogger.LogDebugMessage("HandleBagInventoryUpdate numItems=" + numItems);
                
                for (int i = 0; i < numItems; i++)
                {
                    int bagNum = (int)props["item_" + i + "BagNum"];
                    int slotNum = (int)props["item_" + i + "SlotNum"];
                    string baseName = (string)props["item_" + i + "BaseName"];
                    int templateID = (int)props["item_" + i + "TemplateID"];
                    tmplId = templateID;
                    AtavismInventoryItem invInfo = LoadItemPrefabData(templateID);
             //       Debug.LogWarning("HandleBagInventoryUpdate: "+baseName+" bagNum="+bagNum+" slotNum="+slotNum+" inv="+invInfo);
                 //   GameObject go = new GameObject();
                 //   AtavismInventoryItem invInfo = go.AddComponent<AtavismInventoryItem>();
                    invInfo.Count = (int)props["item_" + i + "Count"];
                    //ClientAPI.Log("ITEM: item count for item %s is %s" % (invInfo.name, invInfo.count))
                    invInfo.ItemId = (OID)props["item_" + i + "Id"];
                    invInfo.name = (string)props["item_" + i + "Name"];
                    invInfo.IsBound = (bool)props["item_" + i + "Bound"];
                    invInfo.EnergyCost = (int)props["item_" + i + "EnergyCost"];
                    invInfo.MaxDurability = (int)props["item_" + i + "MaxDurability"];

                    if (invInfo.icon == null)
                    {
                        iconLack.Add(templateID);
                    }
                    AtavismLogger.LogDebugMessage("Got item: " + invInfo.BaseName);

                    if (invInfo.MaxDurability > 0)
                    {
                        invInfo.Durability = (int)props["item_" + i + "Durability"];
                    }
                    int numResists = (int)props["item_" + i + "NumResistances"];
                    for (int j = 0; j < numResists; j++)
                    {
                        string resistName = (string)props["item_" + i + "Resist_" + j + "Name"];
                        int resistValue = (int)props["item_" + i + "Resist_" + j + "Value"];
                        invInfo.Resistances[resistName] = resistValue;
                    }
                    int numStats = (int)props["item_" + i + "NumStats"];
                    for (int j = 0; j < numStats; j++)
                    {
                        string statName = (string)props["item_" + i + "Stat_" + j + "Name"];
                        int statValue = (int)props["item_" + i + "Stat_" + j + "Value"];
                        invInfo.Stats[statName] = statValue;
                    }

                    int NumEStats = (int)props["item_" + i + "NumEStats"];
                    for (int j = 0; j < NumEStats; j++)
                    {
                        string statName = (string)props["item_" + i + "EStat_" + j + "Name"];
                        int statValue = (int)props["item_" + i + "EStat_" + j + "Value"];
                        invInfo.EnchantStats[statName] = statValue;
                        //  Debug.LogError("Enchant Stat : " + statName + " " + statValue);
                    }
                    invInfo.SocketSlots.Clear();
                    invInfo.SocketSlotsOid.Clear();
                    int NumSocket = (int)props["item_" + i + "NumSocket"];
                    for (int j = 0; j < NumSocket; j++)
                    {
                        string socType = (string)props["item_" + i + "socket_" + j + "Type"];
                        int socItem = (int)props["item_" + i + "socket_" + j + "Item"];
                        long socItemOid = (long)props["item_" + i + "socket_" + j + "ItemOid"];
                        int socId = (int)props["item_" + i + "socket_" + j + "Id"];
                        if (invInfo.SocketSlots.ContainsKey(socType))
                        {
                            invInfo.SocketSlots[socType].Add(socId, socItem);
                            invInfo.SocketSlotsOid[socType].Add(socId, socItemOid);
                        }
                        else
                        {
                            Dictionary<int, long> dicLong = new Dictionary<int, long>();
                            dicLong.Add(socId, socItemOid);
                            invInfo.SocketSlotsOid.Add(socType, dicLong);
                            Dictionary<int, int> dic = new Dictionary<int, int>();
                            dic.Add(socId, socItem);
                            invInfo.SocketSlots.Add(socType, dic);
                        }
                    }


                    int NumOfSet = (int)props["item_" + i + "NumOfSet"];
                    invInfo.setCount = NumOfSet;

                    int ELevel = (int)props["item_" + i + "ELevel"];
                    invInfo.enchantLeval = ELevel;
                    if (invInfo.itemType == "Weapon")
                    {
                        invInfo.DamageValue = (int)props["item_" + i + "DamageValue"];
                        invInfo.DamageMaxValue = (int)props["item_" + i + "DamageValueMax"];
                        invInfo.DamageType = (string)props["item_" + i + "DamageType"];
                        invInfo.WeaponSpeed = (int)props["item_" + i + "Delay"];
                    }
                    bags[bagNum].items.Add(slotNum, invInfo);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("HandleBagInventoryUpdate items "+tmplId+" Exception:" + e +e.Message+e.StackTrace);
            }
           
           //    Debug.LogWarning("HandleBagInventoryUpdate End");
            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "ItemIcon");

                //    NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.GET_ICON", ps);
              //  Debug.LogError("ITEM_PREFAB_DATA  END" + Time.time);
            }
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("INVENTORY_UPDATE", args);
        }
        
        public void HandleEquippedInventoryUpdate(Dictionary<string, object> props)
        {

            if (!itemdataloaded)
            {
                AtavismPrefabManager.Instance.LoadItemPrefabData();
                itemEqMsgQueue.Add(props);
                Debug.LogWarning("HandleEquippedInventoryUpdate item definition not loaded add queue");
                return;
            }
            //     Debug.LogWarning("HandleEquippedInventoryUpdate");
            /*
                    string keys = " [ ";
                    foreach (var it in props.Keys)
                    {
                        keys += " ; " + it + " => " + props[it];
                    }
                    Debug.LogWarning("HandleEquippedInventoryUpdate: keys:" + keys);
            */
            List<int> iconLack = new List<int>();
               foreach (AtavismInventoryItem aii in equippedItems.Values)
               {
                pool.Enqueue(aii);
               }
            equippedItems.Clear();
            try
            {
                int numSlots = (int)props["numSlots"];
                for (int i = 0; i < numSlots; i++)
                {
                    string name = (string)props["item_" + i + "Name"];
                    if (name == null || name == "")
                        continue;
                    string baseName = (string)props["item_" + i + "BaseName"];
                    AtavismInventoryItem invInfo = LoadItemPrefabData(baseName);
                  //  Debug.LogError("HandleEquippedInventoryUpdate name="+name+" baseName=" + baseName + " invInfo=" + invInfo);
                    if (invInfo.icon == null)
                    {
                        iconLack.Add(invInfo.templateId);
                    }
                  //  Debug.LogError("HandleEquippedInventoryUpdate name="+name+" baseName=" + baseName + " invInfo=" + invInfo);
                    invInfo.name = name;
                    invInfo.Count = (int)props["item_" + i + "Count"];
                    invInfo.slot = (string)props["item_" + i + "Slot"];
                    invInfo.ItemId = (OID)props["item_" + i + "Id"];
                    invInfo.IsBound = (bool)props["item_" + i + "Bound"];
                    invInfo.EnergyCost = (int)props["item_" + i + "EnergyCost"];
                    invInfo.MaxDurability = (int)props["item_" + i + "MaxDurability"];
                    if (invInfo.MaxDurability > 0)
                    {
                        invInfo.Durability = (int)props["item_" + i + "Durability"];
                        //AtavismLogger.LogDebugMessage("Durability: " + invInfo.Durability + "/" + invInfo.MaxDurability);
                    }
                    int numResists = (int)props["item_" + i + "NumResistances"];
                    for (int j = 0; j < numResists; j++)
                    {
                        string resistName = (string)props["item_" + i + "Resist_" + j + "Name"];
                        int resistValue = (int)props["item_" + i + "Resist_" + j + "Value"];
                        invInfo.Resistances[resistName] = resistValue;
                    }
                    int numStats = (int)props["item_" + i + "NumStats"];
                    for (int j = 0; j < numStats; j++)
                    {
                        string statName = (string)props["item_" + i + "Stat_" + j + "Name"];
                        int statValue = (int)props["item_" + i + "Stat_" + j + "Value"];
                        invInfo.Stats[statName] = statValue;
                    }
                    int NumEStats = (int)props["item_" + i + "NumEStats"];
                    for (int j = 0; j < NumEStats; j++)
                    {
                        string statName = (string)props["item_" + i + "EStat_" + j + "Name"];
                        int statValue = (int)props["item_" + i + "EStat_" + j + "Value"];
                        invInfo.EnchantStats[statName] = statValue;
                        //  Debug.LogError("Enchant Stat : " + statName + " " + statValue);
                    }
                    invInfo.SocketSlots.Clear();
                    invInfo.SocketSlotsOid.Clear();
                    int NumSocket = (int)props["item_" + i + "NumSocket"];
                    for (int j = 0; j < NumSocket; j++)
                    {
                        string socType = (string)props["item_" + i + "socket_" + j + "Type"];
                        int socItem = (int)props["item_" + i + "socket_" + j + "Item"];
                        long socItemOid = (long)props["item_" + i + "socket_" + j + "ItemOid"];
                        int socId = (int)props["item_" + i + "socket_" + j + "Id"];
                        if (invInfo.SocketSlots.ContainsKey(socType))
                        {
                            invInfo.SocketSlots[socType].Add(socId, socItem);
                            invInfo.SocketSlotsOid[socType].Add(socId, socItemOid);
                        }
                        else
                        {
                            Dictionary<int, int> dic = new Dictionary<int, int>();
                            dic.Add(socId, socItem);
                            invInfo.SocketSlots.Add(socType, dic);
                            Dictionary<int, long> dicLong = new Dictionary<int, long>();
                            dicLong.Add(socId, socItemOid);
                            invInfo.SocketSlotsOid.Add(socType, dicLong);
                        }
                    }
                    int NumOfSet = (int)props["item_" + i + "NumOfSet"];
                    invInfo.setCount = NumOfSet;


                    int ELevel = (int)props["item_" + i + "ELevel"];
                    invInfo.enchantLeval = ELevel;
                    //ClientAPI.Log("InventoryUpdateEntry fields: %s, %d, %d, %s" % (invInfo.itemId, bagNum, slotNum, invInfo.name))
                    if (invInfo.itemType == "Weapon")
                    {
                        invInfo.DamageValue = (int)props["item_" + i + "DamageValue"];
                        invInfo.DamageMaxValue = (int)props["item_" + i + "DamageValueMax"];
                        invInfo.DamageType = (string)props["item_" + i + "DamageType"];
                        invInfo.WeaponSpeed = (int)props["item_" + i + "Delay"];
                    }
                    equippedItems.Add(i, invInfo);
                    AtavismLogger.LogDebugMessage("Added equipped item: " + invInfo.name + " to slot: " + i);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("HandleEquippedInventoryUpdate Bag Exception:" + e);
            }
            // Get Equipped Ammo
            int equippedAmmoID = (int)props["equippedAmmo"];
            if (equippedAmmoID > 0)
            {
                equippedAmmo = GetItemByTemplateID(equippedAmmoID);
                equippedAmmo.Count = GetCountOfItem(equippedAmmoID);

            }
            else
            {
                equippedAmmo = null;
            }
            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "ItemIcon");
                // NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.GET_ICON", ps);
                //Debug.LogError("ITEM_PREFAB_DATA " + Time.time);
            }
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("EQUIPPED_UPDATE", args);
            //   Debug.LogWarning("HandleEquippedInventoryUpdate END");

        }

        public void HandleCloseBankInventory(Dictionary<string, object> props)
        {
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("CLOSE_STORAGE_WINDOW", args);
        }


        public void HandleBankInventoryUpdate(Dictionary<string, object> props)
        {
            //  Debug.LogWarning("HandleBankInventoryUpdate");

            /*    string keys = " [ ";
                 foreach (var it in props.Keys)
                 {
                     keys += " ; " + it+" => "+props[it];
                 }
                 Debug.LogWarning("HandleBankInventoryUpdate: keys:" + keys);
                 */
            List<int> iconLack = new List<int>();
            storageItems.Clear();
            bool open = false;
            try
            {
                int numBags = (int)props["numBags"];
                 open = (bool)props["open"];
                for (int i = 0; i < numBags; i++)
                {
                    Bag bag = new Bag();
                    bag.id = (int)props["bag_" + i + "ID"];
                    bag.name = (string)props["bag_" + i + "Name"];
                    AtavismInventoryItem invInfo = LoadItemPrefabData(bag.name);
                    bag.itemTemplate = invInfo;
                    if (invInfo != null)
                    {
                        if (invInfo.icon != null)
                            bag.icon = invInfo.icon;
                        else
                            bag.icon = AtavismSettings.Instance.defaultItemIcon;
                       // bag.icon = invInfo.icon;
                    }
                    AtavismLogger.LogDebugMessage("Got bag with name: " + bag.name);
                    bag.numSlots = (int)props["bag_" + i + "NumSlots"];
                    if (bag.numSlots == 0)
                    {
                        bag.isActive = false;
                    }
                    else
                    {
                        bag.isActive = true;
                    }
                    bag.slotNum = i;
                    //CSVReader.loadBagData(bag);
                    storageItems[i] = bag;
                }
                int numItems = (int)props["numItems"];
                for (int i = 0; i < numItems; i++)
                {
                    int bagNum = (int)props["item_" + i + "BagNum"];
                    int slotNum = (int)props["item_" + i + "SlotNum"];
                    //string baseName = (string)props["item_" + i + "BaseName"];
                    int templateID = (int)props["item_" + i + "TemplateID"];
                    AtavismInventoryItem invInfo = LoadItemPrefabData(templateID);
                    if (invInfo.icon == null)
                    {
                        iconLack.Add(templateID);
                    }
                    AtavismLogger.LogDebugMessage("Got item: " + invInfo.BaseName);
                    //invInfo.copyData(GetGenericItemData(invInfo.baseName));
                    invInfo.Count = (int)props["item_" + i + "Count"];
                    //ClientAPI.Log("ITEM: item count for item %s is %s" % (invInfo.name, invInfo.count))
                    invInfo.ItemId = (OID)props["item_" + i + "Id"];
                    invInfo.name = (string)props["item_" + i + "Name"];
                    invInfo.IsBound = (bool)props["item_" + i + "Bound"];
                    invInfo.EnergyCost = (int)props["item_" + i + "EnergyCost"];
                    invInfo.MaxDurability = (int)props["item_" + i + "MaxDurability"];
                    if (invInfo.MaxDurability > 0)
                    {
                        invInfo.Durability = (int)props["item_" + i + "Durability"];
                    }
                    int numResists = (int)props["item_" + i + "NumResistances"];
                    for (int j = 0; j < numResists; j++)
                    {
                        string resistName = (string)props["item_" + i + "Resist_" + j + "Name"];
                        int resistValue = (int)props["item_" + i + "Resist_" + j + "Value"];
                        invInfo.Resistances[resistName] = resistValue;
                    }
                    int numStats = (int)props["item_" + i + "NumStats"];
                    for (int j = 0; j < numStats; j++)
                    {
                        string statName = (string)props["item_" + i + "Stat_" + j + "Name"];
                        int statValue = (int)props["item_" + i + "Stat_" + j + "Value"];
                        invInfo.Stats[statName] = statValue;
                    }
              
                    if (invInfo.itemType == "Weapon")
                    {
                        invInfo.DamageValue = (int)props["item_" + i + "DamageValue"];
                        invInfo.DamageType = (string)props["item_" + i + "DamageType"];
                        invInfo.WeaponSpeed = (int)props["item_" + i + "Delay"];
                    }
                    storageItems[bagNum].items[slotNum] = invInfo;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("HandleBankInventoryUpdate  Exception:" + e);
            }
            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "ItemIcon");
                // NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.GET_ICON", ps);
                AtavismLogger.LogDebugMessage("ITEM_PREFAB_DATA " + Time.time);
            }
            string[] args = new string[1];
            args[0] = open.ToString();
            AtavismEventSystem.DispatchEvent("BANK_UPDATE", args);

            storageOpenLoc = ClientAPI.GetPlayerObject().Position;
        }

        public void HandleCurrencies(Dictionary<string, object> props)
        {
         //   Debug.LogWarning("HandleCurrencies Start");
            try
            {
                if (!currencydataloaded)
                {
                    AtavismPrefabManager.Instance.LoadCurrencyPrefabData();
                    currMsgQueue.Add(props);
                    Debug.LogWarning("HandleCurrencies Currencies definition not loaded add queue");
                    return;
                }
              

                sellFactor = (float)props["SellFactor"];
                int numCurrencies = (int)props["numCurrencies"];
                for (int i = 0; i < numCurrencies; i++)
                {
                    int currencyID = (int)props["currency" + i + "ID"];
                   // Debug.LogError(">"+props["currency" + i + "Current"]+"<");
                    if (currencies.ContainsKey(currencyID))
                        currencies[currencyID].Current = (long)props["currency" + i + "Current"];
                }
            
            }
            catch (System.Exception e)
            {
                Debug.LogError("HandleCurrencies  Exception:" + e);
            }
            // dispatch a ui event to tell the rest of the system
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("CURRENCY_UPDATE", args);
         //   Debug.LogWarning("HandleCurrencies end load");
        }

        public void HandleLootList(Dictionary<string, object> props)
        {
            List<int> iconLack = new List<int>();
            loot.Clear();
            lootCurr.Clear();
            try
            {
                int numItems = (int)props["numItems"];

                AtavismLogger.LogDebugMessage("Got Loot list with num items: " + numItems);

                lootTarget = (OID)props["lootTarget"];
                for (int i = 0; i < numItems; i++)
                {
                    string name = (string)props["item_" + i + "Name"];
                    if (name == null || name == "")
                        continue;
                    string baseName = (string)props["item_" + i + "BaseName"];
                    AtavismInventoryItem invInfo = LoadItemPrefabData(baseName);
                    if (invInfo.icon == null)
                    {
                        iconLack.Add(invInfo.templateId);
                    }
                    invInfo.name = name;
                    invInfo.Count = (int)props["item_" + i + "Count"];
                    invInfo.ItemId = (OID)props["item_" + i + "Id"];
                    invInfo.EnergyCost = (int)props["item_" + i + "EnergyCost"];
                    int numResists = (int)props["item_" + i + "NumResistances"];
                    for (int j = 0; j < numResists; j++)
                    {
                        string resistName = (string)props["item_" + i + "Resist_" + j + "Name"];
                        int resistValue = (int)props["item_" + i + "Resist_" + j + "Value"];
                        invInfo.Resistances[resistName] = resistValue;
                    }
                    int numStats = (int)props["item_" + i + "NumStats"];
                    for (int j = 0; j < numStats; j++)
                    {
                        string statName = (string)props["item_" + i + "Stat_" + j + "Name"];
                        int statValue = (int)props["item_" + i + "Stat_" + j + "Value"];
                        invInfo.Stats[statName] = statValue;
                    }
                    loot.Add(i, invInfo);
                    AtavismLogger.LogDebugMessage("Added loot item: " + invInfo.name + " to slot: " + i);
                }
                if (props.ContainsKey("numCurr"))
                {
                    int numCurr = (int)props["numCurr"];
                    for (int i = 0; i < numCurr; i++)
                    {
                        int id = (int)props["curr_" + i + "Id"];
                        int count = (int)props["curr_" + i + "Count"];
                        lootCurr.Add(id, count);
                    }

                }

            }
            catch (System.Exception e)
            {
                Debug.LogError("HandleLootList  Exception:" + e);
            }
            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "ItemIcon");
                // NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.GET_ICON", ps);
               // Debug.LogError("ITEM_PREFAB_DATA " + Time.time);
            }
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("LOOT_UPDATE", args);
        }

        public void HandleInventoryEvent(Dictionary<string, object> props)
        {
            //  Debug.LogWarning("HandleInventoryEvent");
            try
            {
                string eventType = (string)props["event"];
                int itemID = (int)props["itemID"];
                int count = (int)props["count"];
                string data = (string)props["data"];

                // dispatch a ui event to tell the rest of the system
                string[] args = new string[4];
                args[0] = eventType;
                args[1] = itemID.ToString();
                args[2] = count.ToString();
                args[3] = data;
                AtavismEventSystem.DispatchEvent("INVENTORY_EVENT", args);
            }
            catch (System.Exception e)
            {
                Debug.LogError("HandleInventoryEvent  Exception:" + e);
            }
        }

#endregion Message Handlers

#region Properties
        public static Inventory Instance
        {
            get
            {
                return instance;
            }
        }

       /* public Dictionary<int, AtavismInventoryItem> Items
        {
            get
            {
                return items;
            }
        }
        */
        public Dictionary<int, Bag> Bags
        {
            get
            {
                return bags;
            }
        }

        public Dictionary<int, AtavismInventoryItem> EquippedItems
        {
            get
            {
                return equippedItems;
            }
        }

        public AtavismInventoryItem EquippedAmmo
        {
            get
            {
                return equippedAmmo;
            }
        }

        public Dictionary<int, Bag> StorageItems
        {
            get
            {
                return storageItems;
            }
        }

        public Dictionary<int, Currency> Currencies
        {
            get
            {
                return currencies;
            }
        }

        public Dictionary<int, AtavismInventoryItem> Loot
        {
            get
            {
                return loot;
            }
        }
        public Dictionary<int, int> LootCurr
        {
            get
            {
                return lootCurr;
            }
        }


        public OID LootTarget
        {
            get
            {
                return lootTarget;
            }
        }
        public bool InventoryItemLoaded
        {
            get
            {
                return itemdataloaded;
            }
        }
#endregion Properties
    }
}