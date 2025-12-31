using UnityEngine;
using UnityEngine.SceneManagement;

public class EquipmentUIManager : MonoBehaviour
{
    public static EquipmentUIManager Instance;

    public EquipmentSlot[] equipmentSlots;
    public Transform equipmentSlotsParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);  // ← Cambio aquí
            Debug.Log("EquipmentUIManager: Canvas raíz marcado como persistente");
        }
        else if (Instance != this)
        {
            Debug.Log("EquipmentUIManager: Instancia duplicada encontrada, destruyendo Canvas duplicado...");
            Destroy(transform.root.gameObject);  // ← Cambio aquí
            return;
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
        Debug.Log($"EquipmentUIManager: Escena {scene.name} cargada, reinicializando slots...");
        ReinicializarSlots();
    }

    private void Start()
    {
        InicializarSlots();
    }

    private void InicializarSlots()
    {
        if (equipmentSlotsParent == null)
        {
            Debug.LogError("EquipmentUIManager: equipmentSlotsParent no está asignado");
            return;
        }

        equipmentSlots = equipmentSlotsParent.GetComponentsInChildren<EquipmentSlot>(true);
        Debug.Log($"EquipmentUIManager: {equipmentSlots.Length} slots de equipamiento encontrados");

        ActualizarTodosLosSlots();
    }

    private void ReinicializarSlots()
    {
        if (equipmentSlotsParent == null)
        {
            Debug.LogWarning("EquipmentUIManager: equipmentSlotsParent es null, buscando en escena...");
            GameObject equipPanel = GameObject.Find("EquipmentSlotsContainer");
            if (equipPanel != null)
            {
                equipmentSlotsParent = equipPanel.transform;
            }
        }

        if (equipmentSlotsParent != null)
        {
            equipmentSlots = equipmentSlotsParent.GetComponentsInChildren<EquipmentSlot>(true);
            Debug.Log($"EquipmentUIManager: Slots reinicializados - {equipmentSlots.Length} slots encontrados");
            ActualizarTodosLosSlots();
        }
    }

    public void ActualizarTodosLosSlots()
    {
        if (equipmentSlots == null || equipmentSlots.Length == 0)
        {
            Debug.LogWarning("EquipmentUIManager: No hay slots para actualizar");
            return;
        }

        foreach (var slot in equipmentSlots)
        {
            if (slot != null)
            {
                slot.ActualizarSlot();
            }
        }

        Debug.Log($"EquipmentUIManager: {equipmentSlots.Length} slots actualizados");
    }
}
