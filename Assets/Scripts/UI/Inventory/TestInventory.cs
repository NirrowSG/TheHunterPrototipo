using UnityEngine;

public class TestInventory : MonoBehaviour
{
    public ItemDataSO maderaData;
    public ItemDataSO cuchilloData;
    public ItemDataSO metalData;

    public void AgregarMadera()
    {
        if (maderaData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AgregarItem(maderaData, 5);
        }
    }

    public void AgregarCuchillo()
    {
        if (cuchilloData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AgregarItem(cuchilloData, 1);
        }
    }

    public void AgregarMetal()
    {
        if(metalData != null && InventoryManager.Instance != null)
        { InventoryManager.Instance.AgregarItem(metalData, 5); }
    }
}
