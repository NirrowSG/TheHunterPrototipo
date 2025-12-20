using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmDiscardPanel : MonoBehaviour
{
    public static ConfirmDiscardPanel Instance;

    [Header("Referencias UI")]
    public GameObject panelObject;
    public TextMeshProUGUI messageText;
    public Button yesButton;
    public Button noButton;

    private int slotToDiscard = -1;

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
        if (yesButton != null)
        {
            yesButton.onClick.AddListener(OnYesClicked);
        }

        if (noButton != null)
        {
            noButton.onClick.AddListener(OnNoClicked);
        }

        OcultarPanel();
    }

    public void MostrarConfirmacion(int slotIndex)
    {
        slotToDiscard = slotIndex;

        if (panelObject != null)
        {
            panelObject.SetActive(true);
        }

        Debug.Log($"ConfirmDiscardPanel: Mostrando confirmación para slot {slotIndex}");
    }

    private void OnYesClicked()
    {
        Debug.Log("ConfirmDiscardPanel: Usuario confirmó descarte");

        if (InventoryManager.Instance != null && slotToDiscard >= 0)
        {
            InventoryManager.Instance.DescartarItem(slotToDiscard);
        }

        OcultarPanel();

        if (ItemInfoDisplay.Instance != null)
        {
            ItemInfoDisplay.Instance.OcultarPanel();
        }
    }

    private void OnNoClicked()
    {
        Debug.Log("ConfirmDiscardPanel: Usuario canceló descarte");
        OcultarPanel();
    }

    private void OcultarPanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(false);
        }

        slotToDiscard = -1;
    }
}
