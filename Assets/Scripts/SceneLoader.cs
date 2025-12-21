using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Configuración")]
    public Button travelButton;
    public string sceneToLoad;


    private void Start()
    {
        if (travelButton != null)
        {
            travelButton.onClick.AddListener(LoadScene);
            Debug.Log("SceneLoader: Botón de viaje conectado");
        }
        else
        {
            Debug.LogWarning("SceneLoader: No se asignó el botón de viaje");
        }
    }

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("SceneLoader: No se especificó escena para cargar");
            return;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.GuardarInventarioDeJugador();
        }

        Debug.Log($"SceneLoader: Cargando escena - {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.GuardarInventarioDeJugador();
        }

        Debug.Log($"SceneLoader: Cargando escena por índice - {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
    }
}


