using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableInventoryItem
{
    public string itemID;
    public int cantidad;

    public SerializableInventoryItem(string id, int cant)
    {
        itemID = id;
        cantidad = cant;
    }
}

[System.Serializable]
public class GameSaveData
{
    public List<SerializableInventoryItem> playerInventory = new List<SerializableInventoryItem>();
    public List<SerializableInventoryItem> quickInventory = new List<SerializableInventoryItem>();
    public List<SerializableInventoryItem> baseStash = new List<SerializableInventoryItem>();
    public List<SerializableEquipmentSlot> equipment = new List<SerializableEquipmentSlot>();
    public SerializablePlayerStats playerStats;
    public int playerMoney = 0;
    public string lastScene = "PruebaInventario";
    public DateTime lastSaveTime;

    public GameSaveData()
    {
        playerInventory = new List<SerializableInventoryItem>();
        quickInventory = new List<SerializableInventoryItem>();
        baseStash = new List<SerializableInventoryItem>();
        equipment = new List<SerializableEquipmentSlot>();
        playerStats = new SerializablePlayerStats(new PlayerStats());
        lastSaveTime = DateTime.Now;
    }
}
