using UnityEngine;

[System.Serializable] // Importante para verlo en el Inspector
public class InventoryItem
{
    public ItemDataSO itemData;
    public int cantidad;

    public InventoryItem(ItemDataSO item, int cantidad)
    {
        this.itemData = item;
        this.cantidad = cantidad;
    }

    public void AumentarCantidad(int valor)
    {
        cantidad += valor;
    }
}