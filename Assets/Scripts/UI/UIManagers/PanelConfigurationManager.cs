using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PanelConfigurationManager : MonoBehaviour
{
    [Header("Paneles a Controlar")]
    [Tooltip("Panel de botones de navegación izquierdo")]
    public GameObject inventoryButtonsLeftPanel;

    [Tooltip("Otros paneles que se pueden ocultar en ciertas escenas")]
    public List<GameObject> panelesAdicionales = new List<GameObject>();

    [Header("Configuración por Escena")]
    [Tooltip("Escenas donde solo se muestra el InventoryPanel (sin botones de navegación)")]
    public List<string> escenasConInventarioMinimo;

    private bool estadoInicialButtonsPanel;
    private Dictionary<GameObject, bool> estadosInicialesPaneles = new Dictionary<GameObject, bool>();

    private void Awake()
    {
        GuardarEstadosIniciales();
    }

    private void Start()
    {
        ConfigurarPanelesSegunEscena();
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
        ConfigurarPanelesSegunEscena();
    }

    private void GuardarEstadosIniciales()
    {
        if (inventoryButtonsLeftPanel != null)
        {
            estadoInicialButtonsPanel = inventoryButtonsLeftPanel.activeSelf;
        }

        estadosInicialesPaneles.Clear();
        foreach (GameObject panel in panelesAdicionales)
        {
            if (panel != null)
            {
                estadosInicialesPaneles[panel] = panel.activeSelf;
            }
        }

        Debug.Log("PanelConfigurationManager: Estados iniciales guardados");
    }

    private void ConfigurarPanelesSegunEscena()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        bool esModoMinimo = escenasConInventarioMinimo.Contains(escenaActual);

        if (esModoMinimo)
        {
            if (inventoryButtonsLeftPanel != null)
            {
                inventoryButtonsLeftPanel.SetActive(false);
            }

            foreach (GameObject panel in panelesAdicionales)
            {
                if (panel != null)
                {
                    panel.SetActive(false);
                }
            }

            Debug.Log($"PanelConfigurationManager: Modo mínimo activado en escena '{escenaActual}' - Solo InventoryPanel accesible");
        }
        else
        {
            if (inventoryButtonsLeftPanel != null)
            {
                inventoryButtonsLeftPanel.SetActive(estadoInicialButtonsPanel);
            }

            foreach (GameObject panel in panelesAdicionales)
            {
                if (panel != null && estadosInicialesPaneles.ContainsKey(panel))
                {
                    panel.SetActive(estadosInicialesPaneles[panel]);
                }
            }

            Debug.Log($"PanelConfigurationManager: Estados originales restaurados en escena '{escenaActual}'");
        }
    }
}
