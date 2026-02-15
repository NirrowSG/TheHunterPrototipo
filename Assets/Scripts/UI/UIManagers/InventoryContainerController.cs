using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryContainerController : MonoBehaviour
{
    public static InventoryContainerController Instance;

    [Header("Panel de Navegación Permanente")]
    [Tooltip("Panel que normalmente está visible (ej: InventoryButtonsLeftPanel)")]
    public GameObject panelNavegacionPermanente;

    [Tooltip("Activar el panel de navegación al iniciar el juego")]
    public bool panelNavegacionActivoAlIniciar = false;

    [Header("Paneles Alternables")]
    [Tooltip("Paneles que se alternan (InventoryPanel, CraftingPanel, SalidasPanel, etc.)")]
    public GameObject[] panelesAlternables;

    [Header("Panel por Defecto")]
    [Tooltip("Panel que se muestra por defecto al abrir el inventario")]
    public GameObject panelPorDefecto;

    [Header("Configuración")]
    [Tooltip("Ocultar paneles alternables al iniciar")]
    public bool ocultarPanelesAlIniciar = true;

    [Tooltip("Mantener panel de navegación activo siempre (excepto cuando se cierre manualmente)")]
    public bool mantenePanelNavegacionActivo = false;

    [Header("Controles de Juego")]
    [Tooltip("Joystick que se desactivará cuando el inventario esté abierto")]
    public GameObject joystick;

    private GameObject panelActual;
    private Dictionary<string, GameObject> panelesDiccionario;
    private bool panelNavegacionCerradoManualmente = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InicializarPaneles();
    }

    private void InicializarPaneles()
    {
        panelesDiccionario = new Dictionary<string, GameObject>();

        foreach (GameObject panel in panelesAlternables)
        {
            if (panel != null)
            {
                panelesDiccionario[panel.name] = panel;

                if (ocultarPanelesAlIniciar)
                {
                    panel.SetActive(false);
                }
            }
        }

        EstablecerEstadoInicialPanelNavegacion();

        if (panelPorDefecto != null && !ocultarPanelesAlIniciar)
        {
            MostrarPanel(panelPorDefecto);
        }

        Debug.Log($"InventoryContainerController: {panelesDiccionario.Count} paneles alternables inicializados");
    }

    private void EstablecerEstadoInicialPanelNavegacion()
    {
        if (panelNavegacionPermanente != null)
        {
            panelNavegacionPermanente.SetActive(panelNavegacionActivoAlIniciar);
            panelNavegacionCerradoManualmente = !panelNavegacionActivoAlIniciar;

            ActualizarEstadoJoystick();

            Debug.Log($"InventoryContainerController: Panel de navegación {(panelNavegacionActivoAlIniciar ? "activado" : "desactivado")} al inicio");
        }
    }

    private void AsegurarPanelNavegacionActivo()
    {
        if (panelNavegacionPermanente != null && !panelNavegacionPermanente.activeSelf && !panelNavegacionCerradoManualmente)
        {
            panelNavegacionPermanente.SetActive(true);
            ActualizarEstadoJoystick();
            Debug.Log("InventoryContainerController: Panel de navegación permanente activado");
        }
    }

    private void ActualizarEstadoJoystick()
    {
        if (joystick != null)
        {
            bool panelNavegacionActivo = panelNavegacionPermanente != null && panelNavegacionPermanente.activeSelf;
            joystick.SetActive(!panelNavegacionActivo);
            Debug.Log($"InventoryContainerController: Joystick {(panelNavegacionActivo ? "desactivado" : "activado")}");
        }
    }

    public void AlternarPanelNavegacion()
    {
        if (panelNavegacionPermanente == null)
        {
            Debug.LogWarning("InventoryContainerController: No hay panel de navegación configurado");
            return;
        }

        bool estaActivo = panelNavegacionPermanente.activeSelf;

        panelNavegacionPermanente.SetActive(!estaActivo);
        panelNavegacionCerradoManualmente = estaActivo;

        ActualizarEstadoJoystick();

        Debug.Log($"InventoryContainerController: Panel de navegación {(!estaActivo ? "abierto" : "cerrado")}");
    }

    public void MostrarPanelNavegacion()
    {
        if (panelNavegacionPermanente != null)
        {
            panelNavegacionPermanente.SetActive(true);
            panelNavegacionCerradoManualmente = false;
            ActualizarEstadoJoystick();
            Debug.Log("InventoryContainerController: Panel de navegación mostrado");
        }
    }

    public void OcultarPanelNavegacion()
    {
        if (panelNavegacionPermanente != null)
        {
            panelNavegacionPermanente.SetActive(false);
            panelNavegacionCerradoManualmente = true;
            ActualizarEstadoJoystick();
            Debug.Log("InventoryContainerController: Panel de navegación ocultado");
        }
    }

    public void AlternarPanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogWarning("InventoryContainerController: Intento de alternar un panel nulo");
            return;
        }

        if (!panelNavegacionCerradoManualmente && mantenePanelNavegacionActivo)
        {
            AsegurarPanelNavegacionActivo();
        }

        if (panel == panelActual && panel.activeSelf)
        {
            CerrarPanelActual();
        }
        else
        {
            MostrarPanel(panel);
        }
    }

    public void AlternarPanelPorNombre(string nombrePanel)
    {
        if (panelesDiccionario.ContainsKey(nombrePanel))
        {
            AlternarPanel(panelesDiccionario[nombrePanel]);
        }
        else
        {
            Debug.LogWarning($"InventoryContainerController: Panel '{nombrePanel}' no encontrado");
        }
    }

    public void MostrarPanel(GameObject panel)
    {
        if (panel == null)
        {
            return;
        }

        CerrarTodosPaneles();

        panel.SetActive(true);
        panelActual = panel;

        if (!panelNavegacionCerradoManualmente && mantenePanelNavegacionActivo)
        {
            AsegurarPanelNavegacionActivo();
        }

        Debug.Log($"InventoryContainerController: Panel '{panel.name}' mostrado");
    }

    public void MostrarPanelPorNombre(string nombrePanel)
    {
        if (panelesDiccionario.ContainsKey(nombrePanel))
        {
            MostrarPanel(panelesDiccionario[nombrePanel]);
        }
        else
        {
            Debug.LogWarning($"InventoryContainerController: Panel '{nombrePanel}' no encontrado");
        }
    }

    public void MostrarPanelPorDefecto()
    {
        if (panelPorDefecto != null)
        {
            MostrarPanel(panelPorDefecto);
        }
        else
        {
            Debug.LogWarning("InventoryContainerController: No hay panel por defecto configurado");
        }
    }

    public void CerrarTodosPaneles()
    {
        foreach (GameObject panel in panelesAlternables)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        panelActual = null;
    }

    public void CerrarPanelActual()
    {
        if (panelActual != null)
        {
            panelActual.SetActive(false);
            Debug.Log($"InventoryContainerController: Panel '{panelActual.name}' cerrado");
            panelActual = null;
        }

        if (panelPorDefecto != null)
        {
            MostrarPanel(panelPorDefecto);
        }
    }

    public void CerrarInventarioCompleto()
    {
        if (InventoryPanelController.Instance != null)
        {
            InventoryPanelController.Instance.CerrarInventario();
        }

        CerrarTodosPaneles();
        panelNavegacionCerradoManualmente = false;
        Debug.Log("InventoryContainerController: Inventario completo cerrado");
    }

    public bool HayPanelAbierto()
    {
        return panelActual != null && panelActual.activeSelf;
    }

    public GameObject ObtenerPanelActual()
    {
        return panelActual;
    }

    public bool EstaPanelAbierto(GameObject panel)
    {
        return panel != null && panel == panelActual && panel.activeSelf;
    }

    public bool EstaPanelNavegacionVisible()
    {
        return panelNavegacionPermanente != null && panelNavegacionPermanente.activeSelf;
    }

    private void Update()
    {
        if (mantenePanelNavegacionActivo && !panelNavegacionCerradoManualmente)
        {
            AsegurarPanelNavegacionActivo();
        }
    }
}
