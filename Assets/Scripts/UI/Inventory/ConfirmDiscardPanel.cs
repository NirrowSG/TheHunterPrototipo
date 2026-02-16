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
    private bool isFromBaseStash = false;
    private RectTransform panelRectTransform;

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

        if (panelObject != null)
        {
            panelRectTransform = panelObject.GetComponent<RectTransform>();
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
        isFromBaseStash = false;

        if (panelObject != null)
        {
            panelObject.SetActive(true);

            if (panelRectTransform != null)
            {
                panelRectTransform.SetAsLastSibling();
                ////Debug.Log("ConfirmDiscardPanel: Panel movido al frente");
            }
        }

        //Debug.Log($"ConfirmDiscardPanel: Mostrando confirmación para slot {slotIndex}");
    }

    public void MostrarConfirmacionBaseStash(int stashIndex)
    {
        slotToDiscard = stashIndex;
        isFromBaseStash = true;

        if (panelObject != null)
        {
            panelObject.SetActive(true);

            if (panelRectTransform != null)
            {
                panelRectTransform.SetAsLastSibling();
                //Debug.Log("ConfirmDiscardPanel: Panel movido al frente (BaseStash)");
            }
        }

        //Debug.Log($"ConfirmDiscardPanel: Mostrando confirmación para BaseStash índice {stashIndex}");
    }

    private void OnYesClicked()
    {
        //Debug.Log("ConfirmDiscardPanel: Usuario confirmó descarte");

        if (isFromBaseStash)
        {
            if (BaseStashManager.Instance != null && slotToDiscard >= 0)
            {
                BaseStashManager.Instance.RemoverItemDelStash(slotToDiscard);
                //Debug.Log($"ConfirmDiscardPanel: Item removido del BaseStash en índice {slotToDiscard}");

                if (BaseStashUIManager.Instance != null)
                {
                    BaseStashUIManager.Instance.UpdateAllCategories();
                }
            }
        }
        else
        {
            if (InventoryManager.Instance != null && slotToDiscard >= 0)
            {
                InventoryManager.Instance.DescartarItem(slotToDiscard);
            }
        }

        OcultarPanel();

        if (ItemInfoDisplay.Instance != null)
        {
            ItemInfoDisplay.Instance.OcultarPanel();
        }
    }

    private void OnNoClicked()
    {
        //Debug.Log("ConfirmDiscardPanel: Usuario canceló descarte");
        OcultarPanel();
    }

    private void OcultarPanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(false);
        }

        slotToDiscard = -1;
        isFromBaseStash = false;
    }
}
