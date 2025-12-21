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
    public List<SerializableInventoryItem> quickInventory = new List<SerializableInventoryItem>(); // ← AGREGAR ESTA LÍNEA
    public List<SerializableInventoryItem> baseStash = new List<SerializableInventoryItem>();
    public int playerMoney = 0;
    public string lastScene = "PruebaInventario";
    public DateTime lastSaveTime;

    public GameSaveData()
    {
        playerInventory = new List<SerializableInventoryItem>();
        quickInventory = new List<SerializableInventoryItem>(); // ← AGREGAR ESTA LÍNEA
        baseStash = new List<SerializableInventoryItem>();
        lastSaveTime = DateTime.Now;
    }
}

