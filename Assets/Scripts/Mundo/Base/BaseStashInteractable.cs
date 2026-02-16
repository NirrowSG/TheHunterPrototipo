using UnityEngine;
using UnityEngine.InputSystem;

public class BaseStashInteractable : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    [Tooltip("Distancia máxima para interactuar")]
    public float interactionDistance = 2f;

    [Header("Feedback Visual")]
    [Tooltip("Texto o sprite que aparece cuando puedes interactuar")]
    public GameObject interactionPrompt;

    [Header("Opcional: Sprite de la Caja")]
    public SpriteRenderer boxRenderer;
    public Sprite closedBoxSprite;
    public Sprite openBoxSprite;

    private Transform playerTransform;
    private bool isNearBox = false;
    private bool isStashOpen = false;

    private void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        if (boxRenderer != null && closedBoxSprite != null)
        {
            boxRenderer.sprite = closedBoxSprite;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            //Debug.Log("BaseStashInteractable: Player encontrado");
        }
        else
        {
            Debug.LogError("BaseStashInteractable: No se encontró el Player con tag 'Player'");
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        isNearBox = distance <= interactionDistance;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(isNearBox && !isStashOpen);
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (isNearBox)
            {
                ToggleStash();
            }
        }
    }

    private void ToggleStash()
    {
        if (BaseStashUIManager.Instance == null)
        {
            Debug.LogError("BaseStashInteractable: BaseStashUIManager no encontrado");
            return;
        }

        if (isStashOpen)
        {
            CloseStash();
        }
        else
        {
            OpenStash();
        }
    }

    private void OpenStash()
    {
        isStashOpen = true;

        if (boxRenderer != null && openBoxSprite != null)
        {
            boxRenderer.sprite = openBoxSprite;
        }

        BaseStashUIManager.Instance.OpenStashUI();
        //Debug.Log("BaseStashInteractable: Almacenamiento abierto");
    }

    private void CloseStash()
    {
        isStashOpen = false;

        if (boxRenderer != null && closedBoxSprite != null)
        {
            boxRenderer.sprite = closedBoxSprite;
        }

        BaseStashUIManager.Instance.CloseStashUI();
        //Debug.Log("BaseStashInteractable: Almacenamiento cerrado");
    }

    public void OnStashClosed()
    {
        isStashOpen = false;

        if (boxRenderer != null && closedBoxSprite != null)
        {
            boxRenderer.sprite = closedBoxSprite;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
