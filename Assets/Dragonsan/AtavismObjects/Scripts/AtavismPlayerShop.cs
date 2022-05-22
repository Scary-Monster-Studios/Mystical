using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Atavism
{

    public class AtavismPlayerShop : MonoBehaviour
    {
        static AtavismPlayerShop instance;
        Dictionary<OID, string> shoplist = new Dictionary<OID, string>();
        List<ShopItem> plyShopSellItems = new List<ShopItem>();
        List<ShopItem> plyShopBuyItems = new List<ShopItem>();
        OID shop = null;
        [SerializeField] string closeShopConfirmMessage = "Are you sure you want to close the store";

        // Start is called before the first frame update
        void Start()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;

            NetworkAPI.RegisterExtensionMessageHandler("start_player_shop", HandleStartPlayerShop);
            NetworkAPI.RegisterExtensionMessageHandler("list_player_shop", _HandleShops);
            NetworkAPI.RegisterExtensionMessageHandler("PlyShopItems", _HandlePlyShopItems);

        }

        private void HandleStartPlayerShop(Dictionary<string, object> props)
        {
           // Debug.LogError("HandleStartPlayerShop");
        }
        private void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("start_player_shop", HandleStartPlayerShop);
            NetworkAPI.RemoveExtensionMessageHandler("list_player_shop", _HandleShops);
            NetworkAPI.RemoveExtensionMessageHandler("PlyShopItems", _HandlePlyShopItems);

        }

        private void _HandlePlyShopItems(Dictionary<string, object> props)
        {
            plyShopSellItems.Clear();
            plyShopBuyItems.Clear();
            shop = OID.fromLong((long)props["shop"]);
            List<int> iconLack = new List<int>();
            try
            {
                int numItems = (int)props["numItems"];
                if (numItems == 0)
                {
                    string[] arg = new string[1];
                    AtavismEventSystem.DispatchEvent("CLOSE_SHOP", arg);
                    return;
                }

                for (int i = 0; i < numItems; i++)
                {
                    ShopItem shopItem = new ShopItem();
                    bool sell = (bool)props["item_" + i + "sell"];
                    if (!sell)
                    {
                        shopItem.itemID = (int)props["item_" + i + "TemplateID"];
                        AtavismInventoryItem invInfo = AtavismPrefabManager.Instance.LoadItem(shopItem.itemID);
                        shopItem.count = (int)props["item_" + i + "Count"];
                        shopItem.cost = (long)props["item_" + i + "price"];
                        shopItem.purchaseCurrency = (int)props["item_" + i + "Currency"];
                        invInfo.Count = shopItem.count;
                        shopItem.sellCount = shopItem.count;
                        shopItem.item = invInfo;
                        //   merchantItems.Add(shopItem);

                    }
                    else
                    {
                        int templateID = (int)props["item_" + i + "TemplateID"];
                        AtavismInventoryItem invInfo = AtavismPrefabManager.Instance.LoadItem(templateID);
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


                        int ELevel = (int)props["item_" + i + "ELevel"];
                        invInfo.enchantLeval = ELevel;
                        if (invInfo.itemType == "Weapon")
                        {
                            invInfo.DamageValue = (int)props["item_" + i + "DamageValue"];
                            invInfo.DamageMaxValue = (int)props["item_" + i + "DamageValueMax"];
                            invInfo.DamageType = (string)props["item_" + i + "DamageType"];
                            invInfo.WeaponSpeed = (int)props["item_" + i + "Delay"];
                        }

                        shopItem.item = invInfo;
                        //  shopItem.itemID = (int)props["item_" + i + "ID"];
                        shopItem.itemOID = (OID)props["item_" + i + "Oid"];
                        shopItem.count = (int)props["item_" + i + "Count"];
                        shopItem.cost = (long)props["item_" + i + "price"];
                        shopItem.purchaseCurrency = (int)props["item_" + i + "Currency"];
                    }

                    if (sell)
                        plyShopSellItems.Add(shopItem);
                    else
                        plyShopBuyItems.Add(shopItem);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("HandleBagInventoryUpdate items  Exception:" + e + e.Message + e.StackTrace);
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

                //  Debug.LogError("ITEM_PREFAB_DATA  END" + Time.time);
            }
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("SHOP_UPDATE", args);
           // Debug.LogError("_HandlePlyShopItems END" + Time.time);
        }

        private void _HandleShops(Dictionary<string, object> props)
        {
           // Debug.LogError("_HandleShops");
            shoplist.Clear();
            int num = (int)props["num"];
           // Debug.LogError("_HandleShops num="+ num);
            for (int i = 0; i < num; i++)
            {
                OID id = (OID)props["sOid" + i];
                string message = (string)props["sMsg" + i];
                Debug.Log("Shop List add msg=" + message + " id=" + id);
                shoplist.Add(id, message);
            }
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("SHOP_LIST_UPDATE", args);
           // Debug.LogError("_HandleShops END");
        }

        public void CloseShopConfirmed(object item, bool response)
        {
           // Debug.LogError("CloseShopConfirmed ");
            if (!response)
                return;

            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("shop", (OID)item);

            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.STOP_SHOP", props);
           // Debug.LogError("CloseShopConfirmed  END");
        }



        public ShopItem GetSellItem(int itemPos)
        {
            if (plyShopSellItems.Count > itemPos)
            {
                return plyShopSellItems[itemPos];
            }
            return null;
        }
        public ShopItem GetBuyItem(int itemPos)
        {
            if (plyShopBuyItems.Count > itemPos)
            {
                return plyShopBuyItems[itemPos];
            }
            return null;
        }
        public static AtavismPlayerShop Instance
        {
            get
            {
                return instance;
            }
        }
        public List<ShopItem> PlayerShopSellItems
        {
            get
            {
                return plyShopSellItems;
            }
        }
        public List<ShopItem> PlayerShopBuyItems
        {
            get
            {
                return plyShopBuyItems;
            }
        }
        public Dictionary<OID,string> ShopList
        {
            get
            {
                return shoplist;
            }
        }
        public OID Shop
        {
            get
            {
                return shop;
            }
        }
        public string CloseShopConfirmMessage
        {
            get
            {
                return closeShopConfirmMessage;
            }
        }
    }
}