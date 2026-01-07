using UnityEngine;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Configuration")]
    public EnemyDataSO enemyData;

    [Header("Death Settings")]
    public float deathDelay = 1.5f;
    public bool fadeOutOnDeath = true;
    public float fadeOutDuration = 1f;

    [Header("Runtime Stats")]
    private int currentHealth;
    private bool isDead = false;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public event Action<Enemy> OnEnemyDeath;
    public event Action<int> OnHealthChanged;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => enemyData.baseHealth;
    public bool IsDead => isDead;
    public string EnemyName => enemyData.enemyName;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void Initialize(EnemyDataSO data)
    {
        enemyData = data;
        currentHealth = enemyData.baseHealth;
        isDead = false;

        if (spriteRenderer != null)
        {
            if (enemyData.sprite != null)
            {
                spriteRenderer.sprite = enemyData.sprite;
            }

            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        PlayAnimation("Idle");

        EnemyHealthUI healthUI = GetComponentInChildren<EnemyHealthUI>();
        if (healthUI != null)
        {
            healthUI.Initialize(enemyData.enemyName, currentHealth, MaxHealth);
        }

        Debug.Log($"Enemy: {enemyData.enemyName} inicializado - HP: {currentHealth}");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        int finalDamage = Mathf.Max(1, damage - enemyData.baseDefense);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth);

        Debug.Log($"Enemy: {enemyData.enemyName} recibió {finalDamage} de daño - HP: {currentHealth}/{MaxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayAnimation("Hurt");
        }
    }

    public int CalculateAttackDamage()
    {
        return enemyData.baseAttack;
    }

    public void PlayAttackAnimation()
    {
        PlayAnimation("Attack");
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        PlayAnimation("Death");

        Debug.Log($"Enemy: {enemyData.enemyName} ha muerto");

        OnEnemyDeath?.Invoke(this);

        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        if (fadeOutOnDeath && spriteRenderer != null)
        {
            yield return StartCoroutine(FadeOut());
        }
        else
        {
            yield return new WaitForSeconds(deathDelay);
        }

        Debug.Log($"Enemy: {enemyData.enemyName} destruido");
        Destroy(gameObject);
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);

            Color newColor = startColor;
            newColor.a = alpha;
            spriteRenderer.color = newColor;

            yield return null;
        }

        Color finalColor = startColor;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;
    }

    private void PlayAnimation(string animationName)
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetTrigger(animationName);
        }
    }

    public ItemDrop[] GetDroppedItems()
    {
        var droppedItems = new System.Collections.Generic.List<ItemDrop>();

        foreach (var itemDrop in enemyData.itemDrops)
        {
            float roll = UnityEngine.Random.Range(0f, 100f);
            if (roll <= itemDrop.dropChance)
            {
                droppedItems.Add(itemDrop);
            }
        }

        return droppedItems.ToArray();
    }

    public void SetHealthUI(EnemyHealthUI healthUI)
    {
        if (healthUI != null)
        {
            healthUI.Initialize(enemyData.enemyName, currentHealth, MaxHealth);
        }
    }
}
