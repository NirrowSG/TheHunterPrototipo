using UnityEngine;
using UnityEngine.InputSystem;

public class HouseInteraction : MonoBehaviour
{
    [Header("Sprites de la Casa")]
    public Sprite exteriorSprite;
    public Sprite interiorSprite;

    [Header("Referencias")]
    public SpriteRenderer houseRenderer;

    [Header("Configuración de Interacción")]
    [Tooltip("Distancia máxima para interactuar")]
    public float interactionDistance = 3f;

    [Header("Posiciones del Jugador")]
    [Tooltip("Posición donde aparece el jugador al entrar")]
    public Vector2 interiorPlayerPosition = new Vector2(0, -1);

    [Tooltip("Posición donde aparece el jugador al salir")]
    public Vector2 exteriorPlayerPosition = new Vector2(0, -3);

    [Header("Colisión (Opcional)")]
    [Tooltip("Collider que bloquea el paso cuando estás fuera")]
    public Collider2D wallCollider;

    [Header("Feedback Visual (Opcional)")]
    public GameObject interactionPrompt;

    private bool isInside = false;
    private Transform playerTransform;
    private bool isNearHouse = false;

    private void Start()
    {
        if (houseRenderer == null)
        {
            houseRenderer = GetComponent<SpriteRenderer>();
        }

        if (houseRenderer != null && exteriorSprite != null)
        {
            houseRenderer.sprite = exteriorSprite;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log("HouseInteraction: Player encontrado");
        }
        else
        {
            Debug.LogError("HouseInteraction: No se encontró el Player con tag 'Player'");
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        isNearHouse = distance <= interactionDistance;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(isNearHouse);
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (isNearHouse)
            {
                if (isInside)
                {
                    ExitHouseWithTransition();
                }
                else
                {
                    EnterHouseWithTransition();
                }
            }
        }
    }

    private void EnterHouseWithTransition()
    {
        if (ScreenTransition.Instance != null)
        {
            ScreenTransition.Instance.FadeTransition(() => EnterHouse());
        }
        else
        {
            EnterHouse();
        }
    }

    private void ExitHouseWithTransition()
    {
        if (ScreenTransition.Instance != null)
        {
            ScreenTransition.Instance.FadeTransition(() => ExitHouse());
        }
        else
        {
            ExitHouse();
        }
    }

    private void EnterHouse()
    {
        if (houseRenderer != null && interiorSprite != null)
        {
            houseRenderer.sprite = interiorSprite;
            isInside = true;

            if (wallCollider != null)
            {
                wallCollider.enabled = false;
            }

            if (playerTransform != null && interiorPlayerPosition != Vector2.zero)
            {
                playerTransform.position = new Vector3(
                    transform.position.x + interiorPlayerPosition.x,
                    transform.position.y + interiorPlayerPosition.y,
                    playerTransform.position.z
                );
            }

            Debug.Log("HouseInteraction: Jugador ENTRÓ a la casa");
        }
    }

    private void ExitHouse()
    {
        if (houseRenderer != null && exteriorSprite != null)
        {
            houseRenderer.sprite = exteriorSprite;
            isInside = false;

            if (wallCollider != null)
            {
                wallCollider.enabled = true;
            }

            if (playerTransform != null && exteriorPlayerPosition != Vector2.zero)
            {
                playerTransform.position = new Vector3(
                    transform.position.x + exteriorPlayerPosition.x,
                    transform.position.y + exteriorPlayerPosition.y,
                    playerTransform.position.z
                );
            }

            Debug.Log("HouseInteraction: Jugador SALIÓ de la casa");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + (Vector3)interiorPlayerPosition, 0.3f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + (Vector3)exteriorPlayerPosition, 0.3f);
    }
}
