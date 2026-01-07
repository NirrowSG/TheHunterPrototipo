using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class RandomSceneSelector : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [Tooltip("Lista de todas las escenas disponibles para selección aleatoria")]
    public List<string> escenasPosibles = new List<string>();

    [Header("Referencias UI")]
    [Tooltip("Botones que mostrarán las opciones (deben ser exactamente 3)")]
    public List<Button> botonesOpcion = new List<Button>();

    [Tooltip("Textos de los botones para mostrar el nombre de la escena")]
    public List<TMP_Text> textosBoton = new List<TMP_Text>();

    [Header("Configuración")]
    [Tooltip("Cantidad de opciones a mostrar")]
    public int cantidadOpciones = 3;

    private List<string> escenasSeleccionadas = new List<string>();

    private const int TOTAL_ESCENAS_NECESARIAS = 5;

    private void Start()
    {
        ValidarConfiguracion();
        SeleccionarEscenasAleatorias();
        ConfigurarBotones();
    }

    private void ValidarConfiguracion()
    {
        if (escenasPosibles.Count < TOTAL_ESCENAS_NECESARIAS)
        {
            Debug.LogError($"RandomSceneSelector: Se necesitan al menos {TOTAL_ESCENAS_NECESARIAS} escenas en la lista. Actualmente hay {escenasPosibles.Count}");
            return;
        }

        if (botonesOpcion.Count != cantidadOpciones)
        {
            Debug.LogError($"RandomSceneSelector: Se necesitan exactamente {cantidadOpciones} botones. Actualmente hay {botonesOpcion.Count}");
            return;
        }

        if (textosBoton.Count != cantidadOpciones)
        {
            Debug.LogError($"RandomSceneSelector: Se necesitan exactamente {cantidadOpciones} textos. Actualmente hay {textosBoton.Count}");
        }
    }

    private void SeleccionarEscenasAleatorias()
    {
        escenasSeleccionadas.Clear();

        List<string> escenasDisponibles = new List<string>(escenasPosibles);

        for (int i = 0; i < cantidadOpciones && escenasDisponibles.Count > 0; i++)
        {
            int indiceAleatorio = Random.Range(0, escenasDisponibles.Count);
            escenasSeleccionadas.Add(escenasDisponibles[indiceAleatorio]);
            escenasDisponibles.RemoveAt(indiceAleatorio);
        }

        Debug.Log($"RandomSceneSelector: Se seleccionaron {escenasSeleccionadas.Count} escenas aleatorias");
    }

    private void ConfigurarBotones()
    {
        for (int i = 0; i < botonesOpcion.Count && i < escenasSeleccionadas.Count; i++)
        {
            int indiceLocal = i;
            string escenaActual = escenasSeleccionadas[indiceLocal];

            if (textosBoton[i] != null)
            {
                textosBoton[i].text = ObtenerNombreAmigable(escenaActual);
            }

            botonesOpcion[i].onClick.RemoveAllListeners();
            botonesOpcion[i].onClick.AddListener(() => CargarEscena(escenaActual));

            Debug.Log($"RandomSceneSelector: Botón {i + 1} configurado para escena '{escenaActual}'");
        }
    }

    private string ObtenerNombreAmigable(string nombreEscena)
    {
        switch (nombreEscena)
        {
            case "BosqueCombate":
                return "Combate";
            case "BosqueCombateElite":
                return "Combate Elite";
            case "BosqueEvento":
                return "Evento";
            case "BosqueFarmear":
                return "Farmear";
            case "BosqueTienda":
                return "Tienda";
            default:
                return nombreEscena.Replace("Bosque", "");
        }
    }

    private void CargarEscena(string nombreEscena)
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogWarning("RandomSceneSelector: Nombre de escena vacío");
            return;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.GuardarInventarioDeJugador();
        }

        Debug.Log($"RandomSceneSelector: Cargando escena '{nombreEscena}'");
        SceneManager.LoadScene(nombreEscena);
    }
}
