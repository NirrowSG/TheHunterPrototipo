using UnityEngine;
using UnityEngine.UI;

public class BotonInventarioHelper : MonoBehaviour
{
    private Button boton;
    private bool eventoConectado = false;

    private void Awake()
    {
        boton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        ConectarEvento();
    }

    private void Start()
    {
        ConectarEvento();
    }

    private void ConectarEvento()
    {
        if (boton == null)
        {
            boton = GetComponent<Button>();
        }

        if (boton != null && !eventoConectado)
        {
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(AlternarInventario);
            eventoConectado = true;
            Debug.Log("BotonInventarioHelper: Botón de inventario conectado");
        }
    }

    private void AlternarInventario()
    {
        Debug.Log("BotonInventarioHelper: Click detectado - Intentando alternar inventario");

        if (InventoryPanelController.Instance != null)
        {
            InventoryPanelController.Instance.AlternarInventario();
            Debug.Log("BotonInventarioHelper: Inventario alternado exitosamente");
        }
        else
        {
            Debug.LogWarning("BotonInventarioHelper: InventoryPanelController.Instance es NULL");
        }
    }

    private void OnDisable()
    {
        eventoConectado = false;
    }
}
