using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("Background Settings")]
    [Tooltip("Lista de sprites de fondo disponibles")]
    public Sprite[] backgroundSprites;

    [Header("Background Object")]
    [Tooltip("GameObject con SpriteRenderer que mostrará el fondo")]
    public SpriteRenderer backgroundRenderer;

    [Header("Optional Settings")]
    [Tooltip("Orden de capa para el fondo (normalmente -10 o menos)")]
    public int sortingOrder = -10;

    private void Start()
    {
        SetRandomBackground();
    }

    public void SetRandomBackground()
    {
        if (backgroundSprites == null || backgroundSprites.Length == 0)
        {
            Debug.LogWarning("BackgroundManager: No hay sprites de fondo asignados");
            return;
        }

        if (backgroundRenderer == null)
        {
            Debug.LogError("BackgroundManager: No hay SpriteRenderer asignado");
            return;
        }

        int randomIndex = Random.Range(0, backgroundSprites.Length);
        Sprite selectedBackground = backgroundSprites[randomIndex];

        backgroundRenderer.sprite = selectedBackground;
        backgroundRenderer.sortingOrder = sortingOrder;

        Debug.Log($"BackgroundManager: Fondo seleccionado: {selectedBackground.name}");
    }

    public void SetSpecificBackground(int index)
    {
        if (backgroundSprites == null || index < 0 || index >= backgroundSprites.Length)
        {
            Debug.LogWarning($"BackgroundManager: Índice {index} fuera de rango");
            return;
        }

        if (backgroundRenderer == null)
        {
            Debug.LogError("BackgroundManager: No hay SpriteRenderer asignado");
            return;
        }

        backgroundRenderer.sprite = backgroundSprites[index];
        backgroundRenderer.sortingOrder = sortingOrder;

        Debug.Log($"BackgroundManager: Fondo establecido: {backgroundSprites[index].name}");
    }
}
