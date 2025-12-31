using UnityEngine;

public class TestInventory : MonoBehaviour
{
    public ItemDataSO maderaData;
    public ItemDataSO cuchilloData;
    public ItemDataSO metalData;
    public ItemDataSO CascoData;
    public ItemDataSO CamisetaData;
    public ItemDataSO PistolaData;

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

    public void AgregarCasco()
    {
        if (CascoData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AgregarItem(CascoData, 1);
        }
    }

    public void AgregarCamiseta()
    {
        if (CamisetaData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AgregarItem(CamisetaData, 1);
        }
    }

    public void AgregarMetal()
    {
        if(metalData != null && InventoryManager.Instance != null)
        { InventoryManager.Instance.AgregarItem(metalData, 5); }
    }

    public void AgregarPistola()
    {
        if (PistolaData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AgregarItem(PistolaData, 1);
        }
    }
}
