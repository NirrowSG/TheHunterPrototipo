using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ItemInfoDisplay : MonoBehaviour
{
    public static ItemInfoDisplay Instance;

    [Header("Referencias UI")]
    public GameObject panelObject;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Button splitButton;
    public Button discardButton;

    [Header("Configuración")]
    public Vector2 offset = new Vector2(10f, 10f);

    private RectTransform panelRectTransform;
    private Canvas canvas;
    private bool isVisible = false;
    private bool justOpened = false;
    private ItemDataSO currentItemData;
    private int currentSlotIndex = -1;
    private bool isFromBaseStash = false;

    private void Awake()
    {
        //Debug.Log("ItemInfoDisplay: Awake llamado");

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InicializarReferencias();
    }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            InicializarReferencias();
        }
    }

    private void InicializarReferencias()
    {
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            //Debug.Log($"ItemInfoDisplay: Canvas encontrado: {canvas != null}");
        }

        if (panelRectTransform == null && panelObject != null)
        {
            panelRectTransform = panelObject.GetComponent<RectTransform>();
            //Debug.Log($"ItemInfoDisplay: PanelRectTransform asignado: {panelRectTransform != null}");
        }

        if (panelRectTransform == null)
        {
            panelRectTransform = GetComponent<RectTransform>();
            //Debug.Log("ItemInfoDisplay: Usando RectTransform propio");
        }

        if (splitButton != null)
        {
            splitButton.onClick.AddListener(OnSplitButtonClicked);
            //Debug.Log("ItemInfoDisplay: Botón dividir conectado");
        }
        if (discardButton != null)
        {
            discardButton.onClick.AddListener(OnDiscardButtonClicked);
            //Debug.Log("ItemInfoDisplay: Botón descartar conectado");
        }

        //Debug.Log($"ItemInfoDisplay: Referencias - Panel: {panelObject != null}, Name: {itemNameText != null}, Desc: {itemDescriptionText != null}");
    }

    private void Start()
    {
        //Debug.Log("ItemInfoDisplay: Start llamado");
        OcultarPanel();
    }

    public void MostrarInfoItem(ItemDataSO itemData, Vector3 posicionMundo, int slotIndex)
    {
        //Debug.Log($"ItemInfoDisplay: MostrarInfoItem llamado para {itemData?.Name} en slot {slotIndex}");

        if (itemData == null)
        {
            Debug.LogWarning("ItemInfoDisplay: itemData es null");
            return;
        }

        if (panelObject == null)
        {
            Debug.LogError("ItemInfoDisplay: panelObject es null");
            return;
        }

        if (itemNameText == null || itemDescriptionText == null)
        {
            Debug.LogError("ItemInfoDisplay: Referencias de texto son null");
            return;
        }

        currentItemData = itemData;
        currentSlotIndex = slotIndex;
        isFromBaseStash = false;

        itemNameText.text = itemData.Name;
        itemDescriptionText.text = itemData.Description;

        if (splitButton != null)
        {
            InventoryItem item = InventoryManager.Instance?.inventarioItems[slotIndex];
            bool canSplit = item != null && itemData.IsStackable && item.cantidad > 1;
            splitButton.gameObject.SetActive(canSplit);
            //Debug.Log($"ItemInfoDisplay: Botón dividir {(canSplit ? "visible" : "oculto")} (cantidad: {item?.cantidad})");
        }

        panelObject.SetActive(true);
        isVisible = true;
        justOpened = true;

        if (panelRectTransform != null)
        {
            panelRectTransform.SetAsLastSibling();
            //Debug.Log("ItemInfoDisplay: Panel movido al frente");
        }

        //Debug.Log($"ItemInfoDisplay: Panel activado, mostrando '{itemData.Name}'");

        if (discardButton != null)
        {
            discardButton.gameObject.SetActive(true);
            //Debug.Log("ItemInfoDisplay: Botón descartar visible");
        }

        PosicionarPanel(posicionMundo);
    }

    public void MostrarInfoItemDeBaseStash(ItemDataSO itemData, Vector3 posicionMundo, int stashIndex)
    {
        //Debug.Log($"ItemInfoDisplay: MostrarInfoItem (BaseStash) llamado para {itemData?.Name} en índice {stashIndex}");

        if (itemData == null)
        {
            Debug.LogWarning("ItemInfoDisplay: itemData es null");
            return;
        }

        if (panelObject == null)
        {
            Debug.LogError("ItemInfoDisplay: panelObject es null");
            return;
        }

        if (itemNameText == null || itemDescriptionText == null)
        {
            Debug.LogError("ItemInfoDisplay: Referencias de texto son null");
            return;
        }

        currentItemData = itemData;
        currentSlotIndex = stashIndex;
        isFromBaseStash = true;

        itemNameText.text = itemData.Name;
        itemDescriptionText.text = itemData.Description;

        if (splitButton != null)
        {
            splitButton.gameObject.SetActive(false);
        }

        panelObject.SetActive(true);
        isVisible = true;
        justOpened = true;

        if (panelRectTransform != null)
        {
            panelRectTransform.SetAsLastSibling();
            //Debug.Log("ItemInfoDisplay: Panel movido al frente");
        }

        //Debug.Log($"ItemInfoDisplay: Panel activado (BaseStash), mostrando '{itemData.Name}'");

        if (discardButton != null)
        {
            discardButton.gameObject.SetActive(true);
            //Debug.Log("ItemInfoDisplay: Botón descartar visible");
        }

        PosicionarPanel(posicionMundo);
    }

    private void OnSplitButtonClicked()
    {
        //Debug.Log($"ItemInfoDisplay: Dividir item en slot {currentSlotIndex}");

        if (InventoryManager.Instance != null && currentSlotIndex >= 0)
        {
            InventoryManager.Instance.DividirItem(currentSlotIndex);
            OcultarPanel();
        }
    }

    private void PosicionarPanel(Vector3 posicionMundo)
    {
        if (canvas == null || panelRectTransform == null)
        {
            Debug.LogWarning("ItemInfoDisplay: Canvas o PanelRect es null en PosicionarPanel");
            return;
        }

        Vector2 posicionLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            posicionMundo,
            canvas.worldCamera,
            out posicionLocal
        );

        panelRectTransform.localPosition = posicionLocal + offset;

        AjustarPosicionEnPantalla();
    }

    private void AjustarPosicionEnPantalla()
    {
        if (panelRectTransform == null || canvas == null) return;

        Vector3[] corners = new Vector3[4];
        panelRectTransform.GetWorldCorners(corners);

        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        Vector3 ajuste = Vector3.zero;

        if (corners[2].x > canvasCorners[2].x)
        {
            ajuste.x = canvasCorners[2].x - corners[2].x - 10f;
        }
        if (corners[0].x < canvasCorners[0].x)
        {
            ajuste.x = canvasCorners[0].x - corners[0].x + 10f;
        }
        if (corners[2].y > canvasCorners[2].y)
        {
            ajuste.y = canvasCorners[2].y - corners[2].y - 10f;
        }
        if (corners[0].y < canvasCorners[0].y)
        {
            ajuste.y = canvasCorners[0].y - corners[0].y + 10f;
        }

        panelRectTransform.position += ajuste;
    }

    public void OcultarPanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(false);
            isVisible = false;
            currentItemData = null;
            currentSlotIndex = -1;
            isFromBaseStash = false;
            //Debug.Log("ItemInfoDisplay: Panel ocultado");
        }
    }

    public bool EstaVisible()
    {
        return isVisible;
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        if (isVisible && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (justOpened)
            {
                justOpened = false;
                return;
            }

            //Debug.Log("ItemInfoDisplay: Click detectado, cerrando panel");
            OcultarPanel();
        }
    }

    private void OnDiscardButtonClicked()
    {
        if (isFromBaseStash)
        {
            //Debug.Log($"ItemInfoDisplay: Solicitando confirmación para descartar del BaseStash índice {currentSlotIndex}");

            if (ConfirmDiscardPanel.Instance != null && currentSlotIndex >= 0)
            {
                ConfirmDiscardPanel.Instance.MostrarConfirmacionBaseStash(currentSlotIndex);
            }
        }
        else
        {
            //Debug.Log($"ItemInfoDisplay: Solicitando confirmación para descartar slot {currentSlotIndex}");

            if (ConfirmDiscardPanel.Instance != null && currentSlotIndex >= 0)
            {
                ConfirmDiscardPanel.Instance.MostrarConfirmacion(currentSlotIndex);
            }
        }
    }
}
