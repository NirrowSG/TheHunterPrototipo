using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Posición de spawn por defecto si no hay datos guardados")]
    public Vector2 defaultSpawnPosition = Vector2.zero;

    private void Awake()
    {
        PositionarJugador();
    }

    private void PositionarJugador()
    {
        if (PlayerPrefs.HasKey("SpawnX") && PlayerPrefs.HasKey("SpawnY"))
        {
            float spawnX = PlayerPrefs.GetFloat("SpawnX");
            float spawnY = PlayerPrefs.GetFloat("SpawnY");

            transform.position = new Vector3(spawnX, spawnY, transform.position.z);

            PlayerPrefs.DeleteKey("SpawnX");
            PlayerPrefs.DeleteKey("SpawnY");

            Debug.Log($"PlayerSpawnManager: Jugador posicionado en ({spawnX}, {spawnY})");
        }
        else
        {
            transform.position = new Vector3(defaultSpawnPosition.x, defaultSpawnPosition.y, transform.position.z);
            Debug.Log($"PlayerSpawnManager: Jugador posicionado en posición por defecto {defaultSpawnPosition}");
        }
    }
}
