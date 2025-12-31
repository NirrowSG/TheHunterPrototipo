using UnityEngine;

public class FindSavePath : MonoBehaviour
{
    void Start()
    {
        Debug.Log($">>> RUTA DE GUARDADO: {Application.persistentDataPath}");
    }
}
