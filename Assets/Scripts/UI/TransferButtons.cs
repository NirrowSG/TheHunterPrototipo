using UnityEngine;
using UnityEngine.UI;

public class TransferButtons : MonoBehaviour
{
    [Header("Botones")]
    public Button depositarTodoButton;
    public Button cargarDesdeStashButton;

    private void Start()
    {
        if (depositarTodoButton != null)
        {
            depositarTodoButton.onClick.AddListener(DepositarTodo);
        }

        if (cargarDesdeStashButton != null)
        {
            cargarDesdeStashButton.onClick.AddListener(CargarDesdeStash);
        }
    }

    public void DepositarTodo()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.TransferirTodoAlStash();
            InventoryManager.Instance.GuardarInventarioDeJugador();
            Debug.Log("TransferButtons: Todos los items depositados y guardados");
        }
    }


    public void CargarDesdeStash()
    {
        Debug.Log("TransferButtons: Función de cargar desde stash - implementar UI de stash");
    }
}
