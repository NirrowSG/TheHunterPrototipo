using UnityEngine;
using UnityEngine.UI;

public class InventoryPanelController : MonoBehaviour
{
    public static InventoryPanelController Instance;

    [Header("Referencias UI")]
    public GameObject inventoryPanelObject;
    public Button closeButton;
    public Button openButton;

    private bool primeraApertura = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CerrarInventario);
            Debug.Log("InventoryPanelController: Botón cerrar conectado");
        }

        if (openButton != null)
        {
            openButton.onClick.AddListener(AbrirInventario);
            Debug.Log("InventoryPanelController: Botón abrir conectado");
        }

        ActualizarVisibilidadBotones();
    }

    public void AbrirInventario()
    {
        if (inventoryPanelObject != null)
        {
            inventoryPanelObject.SetActive(true);
            Debug.Log("InventoryPanelController: Inventario abierto");
        }

        if (primeraApertura)
        {
            primeraApertura = false;

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ActualizarUI();
                Debug.Log("InventoryPanelController: Primera apertura - UI actualizada");
            }
        }

        ActualizarVisibilidadBotones();
    }

    public void CerrarInventario()
    {
        if (inventoryPanelObject != null)
        {
            inventoryPanelObject.SetActive(false);
            Debug.Log("InventoryPanelController: Inventario cerrado");
        }

        if (ItemInfoDisplay.Instance != null && ItemInfoDisplay.Instance.EstaVisible())
        {
            ItemInfoDisplay.Instance.OcultarPanel();
        }

        ActualizarVisibilidadBotones();
    }

    public void AlternarInventario()
    {
        if (inventoryPanelObject != null)
        {
            bool estaActivo = inventoryPanelObject.activeSelf;

            if (estaActivo)
            {
                CerrarInventario();
            }
            else
            {
                AbrirInventario();
            }
        }
    }

    public bool EstaAbierto()
    {
        return inventoryPanelObject != null && inventoryPanelObject.activeSelf;
    }

    private void ActualizarVisibilidadBotones()
    {
        bool inventarioAbierto = inventoryPanelObject != null && inventoryPanelObject.activeSelf;

        if (openButton != null)
        {
            openButton.gameObject.SetActive(!inventarioAbierto);
        }
    }
}
