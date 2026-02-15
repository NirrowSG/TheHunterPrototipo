using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenTransition : MonoBehaviour
{
    public static ScreenTransition Instance;

    [Header("Referencias")]
    [Tooltip("Panel negro que cubre toda la pantalla")]
    public Image fadeImage;

    [Header("Configuración")]
    [Tooltip("Duración del fade out (oscurecer)")]
    public float fadeOutDuration = 0.3f;

    [Tooltip("Duración del fade in (aclarar)")]
    public float fadeInDuration = 0.3f;

    [Tooltip("Tiempo que permanece en negro")]
    public float pauseDuration = 0.1f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (fadeImage != null)
            {
                canvasGroup = fadeImage.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = fadeImage.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0f;
                fadeImage.gameObject.SetActive(true);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeTransition(System.Action onMiddle)
    {
        StartCoroutine(FadeSequence(onMiddle));
    }

    private IEnumerator FadeSequence(System.Action onMiddle)
    {
        yield return StartCoroutine(FadeOut());

        onMiddle?.Invoke();

        yield return new WaitForSeconds(pauseDuration);

        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
