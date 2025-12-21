using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasPersistence : MonoBehaviour
{
    public static CanvasPersistence Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("CanvasPersistence: Canvas marcado como persistente");
        }
        else
        {
            Debug.Log("CanvasPersistence: Canvas duplicado encontrado, destruyendo...");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"CanvasPersistence: Escena cargada - {scene.name}");

        GameObject[] eventSystems = GameObject.FindGameObjectsWithTag("GameController");
        if (eventSystems.Length > 1)
        {
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i]);
            }
        }
    }
}
