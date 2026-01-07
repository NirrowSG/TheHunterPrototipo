using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneSpecificUIManager : MonoBehaviour
{
    [System.Serializable]
    public class UIElement
    {
        public GameObject gameObject;
        public List<string> escenasVisibles = new List<string>();
        public bool invertirLogica = false;
    }

    [Header("Elementos UI por Escena")]
    public List<UIElement> elementosUI = new List<UIElement>();

    private void Awake()
    {
        ActualizarVisibilidadTodos();
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
        ActualizarVisibilidadTodos();
    }

    private void ActualizarVisibilidadTodos()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        foreach (UIElement elemento in elementosUI)
        {
            if (elemento.gameObject != null)
            {
                bool estaEnLista = elemento.escenasVisibles.Contains(escenaActual);
                bool debeEstarActivo = elemento.invertirLogica ? !estaEnLista : estaEnLista;

                elemento.gameObject.SetActive(debeEstarActivo);

                string estado = debeEstarActivo ? "ACTIVO" : "INACTIVO";
                Debug.Log($"SceneSpecificUIManager: {elemento.gameObject.name} {estado} en escena '{escenaActual}'");
            }
        }
    }
}
