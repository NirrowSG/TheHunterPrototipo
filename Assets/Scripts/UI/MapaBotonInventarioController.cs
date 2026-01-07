using UnityEngine;
using UnityEngine.SceneManagement;

public class MapaBotonInventarioController : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        VerificarEscena();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        VerificarEscena();
    }

    private void VerificarEscena()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        bool esMapaBosque = escenaActual == "MapaFarmeoBosque";

        gameObject.SetActive(esMapaBosque);

        Debug.Log($"MapaBotonInventario: {(esMapaBosque ? "ACTIVADO" : "DESACTIVADO")} en escena '{escenaActual}'");
    }
}
