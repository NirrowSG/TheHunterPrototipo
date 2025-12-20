using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Configuración")]
    public Button travelButton;
    public string sceneToLoad = "SampleScene";

    private void Start()
    {
        if (travelButton != null)
        {
            travelButton.onClick.AddListener(CargarEscena);
            Debug.Log("SceneLoader: Botón de viaje conectado");
        }
        else
        {
            Debug.LogWarning("SceneLoader: No se asignó el botón de viaje");
        }
    }

    public void CargarEscena()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"SceneLoader: Cargando escena {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("SceneLoader: No se especificó ninguna escena para cargar");
        }
    }

    public void CargarEscenaPorNombre(string nombreEscena)
    {
        if (!string.IsNullOrEmpty(nombreEscena))
        {
            Debug.Log($"SceneLoader: Cargando escena {nombreEscena}");
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError("SceneLoader: Nombre de escena vacío");
        }
    }
}
