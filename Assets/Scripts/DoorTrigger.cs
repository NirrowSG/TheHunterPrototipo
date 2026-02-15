using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DoorTrigger : MonoBehaviour
{
    [Header("Configuración de la Puerta")]
    [Tooltip("Nombre de la escena de destino")]
    public string targetScene;

    [Tooltip("Posición donde aparecerá el jugador en la escena destino")]
    public Vector2 spawnPosition;

    [Header("Opciones de Interacción")]
    [Tooltip("¿Requiere presionar una tecla para viajar?")]
    public bool requireKeyPress = true;

    [Header("Feedback Visual")]
    [Tooltip("Texto UI para mostrar mensaje de interacción")]
    public GameObject interactionPrompt;

    private bool playerInRange = false;

    private void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && requireKeyPress && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TravelToScene();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }

            if (!requireKeyPress)
            {
                TravelToScene();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }

    private void TravelToScene()
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning("DoorTrigger: No se especificó escena de destino");
            return;
        }

        GuardarPosicionDeSpawn();
        GuardarTodosDatos();

        Debug.Log($"DoorTrigger: Viajando a {targetScene} en posición {spawnPosition}");
        SceneManager.LoadScene(targetScene);
    }

    private void GuardarPosicionDeSpawn()
    {
        PlayerPrefs.SetFloat("SpawnX", spawnPosition.x);
        PlayerPrefs.SetFloat("SpawnY", spawnPosition.y);
        PlayerPrefs.Save();
    }

    private void GuardarTodosDatos()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.GuardarInventarioDeJugador();
        }

        if (EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.GuardarEquipamiento();
        }

        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.GuardarStats();
        }

        if (GameDataManager.Instance != null)
        {
            GameSaveData saveData = GameDataManager.Instance.GetSaveData();
            saveData.lastScene = targetScene;
            GameDataManager.Instance.GuardarDatos();
        }
    }
}
