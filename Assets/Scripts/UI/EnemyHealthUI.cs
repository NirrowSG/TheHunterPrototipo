using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI enemyNameText;
    public Slider healthBarSlider;
    public Image healthBarFillImage;

    [Header("UI Settings")]
    public Vector3 uiOffset = new Vector3(0, 1.5f, 0);
    public bool hideWhenDead = true;
    public bool faceCamera = true;

    [Header("Health Bar Colors")]
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color criticalColor = Color.red;
    public float criticalThreshold = 0.3f;
    public float damagedThreshold = 0.6f;

    private Enemy enemy;
    private Transform mainCamera;
    private Canvas canvas;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        canvas = GetComponent<Canvas>();

        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }

        if (healthBarSlider != null && healthBarFillImage == null)
        {
            healthBarFillImage = healthBarSlider.fillRect?.GetComponent<Image>();
        }
    }

    private void Start()
    {
        if (enemy != null)
        {
            enemy.OnHealthChanged += UpdateHealthBar;
            enemy.OnEnemyDeath += OnEnemyDied;
        }

        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (faceCamera && mainCamera != null)
        {
            transform.rotation = mainCamera.rotation;
        }
    }

    private void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnHealthChanged -= UpdateHealthBar;
            enemy.OnEnemyDeath -= OnEnemyDied;
        }
    }

    public void Initialize(string enemyName, int currentHealth, int maxHealth)
    {
        if (enemyNameText != null)
        {
            enemyNameText.text = enemyName;
        }

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }

        UpdateHealthBar(currentHealth, maxHealth);

        Debug.Log($"EnemyHealthUI: {enemyName} inicializado - HP: {currentHealth}/{maxHealth}");
    }

    private void UpdateHealthBar(int currentHealth)
    {
        if (enemy != null)
        {
            UpdateHealthBar(currentHealth, enemy.MaxHealth);
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBarSlider == null)
        {
            Debug.LogError("EnemyHealthUI: healthBarSlider es null!");
            return;
        }

        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;

        float healthPercentage = (float)currentHealth / maxHealth;

        if (healthBarFillImage != null)
        {
            if (healthPercentage <= criticalThreshold)
            {
                healthBarFillImage.color = criticalColor;
            }
            else if (healthPercentage <= damagedThreshold)
            {
                healthBarFillImage.color = damagedColor;
            }
            else
            {
                healthBarFillImage.color = healthyColor;
            }
        }

        Debug.Log($"EnemyHealthUI: HP actualizado - {currentHealth}/{maxHealth} ({healthPercentage:P0})");
    }

    private void OnEnemyDied(Enemy deadEnemy)
    {
        if (hideWhenDead)
        {
            gameObject.SetActive(false);
        }
    }
}
