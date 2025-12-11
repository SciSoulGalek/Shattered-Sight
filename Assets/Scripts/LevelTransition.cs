using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public static LevelTransition Instance { get; private set; }

    [Header("Fade UI")]
    public CanvasGroup fadeGroup;      

    [Header("Timings")]
    public float preFadeDelay   = 0.0f; 
    public float fadeOutTime    = 0.7f;
    public float fadeInTime     = 0.7f; 
    public float delayAfterLoad = 0.2f; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 

        if (fadeGroup == null)
            fadeGroup = GetComponentInChildren<CanvasGroup>();

        if (fadeGroup != null)
            fadeGroup.alpha = 0f;
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    public void FadeToNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        StartCoroutine(FadeAndLoadScene(nextIndex));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (fadeGroup == null)
        {
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        if (preFadeDelay > 0f)
            yield return new WaitForSeconds(preFadeDelay);

        yield return Fade(0f, 1f, fadeOutTime);

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return null;

        if (delayAfterLoad > 0f)
            yield return new WaitForSeconds(delayAfterLoad);

        yield return Fade(1f, 0f, fadeInTime);
    }

    private IEnumerator FadeAndLoadScene(int buildIndex)
    {
        if (fadeGroup == null)
        {
            SceneManager.LoadScene(buildIndex);
            yield break;
        }

        if (preFadeDelay > 0f)
            yield return new WaitForSeconds(preFadeDelay);

        yield return Fade(0f, 1f, fadeOutTime);

        yield return SceneManager.LoadSceneAsync(buildIndex);
        yield return null;

        if (delayAfterLoad > 0f)
            yield return new WaitForSeconds(delayAfterLoad);

        yield return Fade(1f, 0f, fadeInTime);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeGroup == null || duration <= 0f)
        {
            fadeGroup.alpha = to;
            yield break;
        }

        float t = 0f;
        fadeGroup.alpha = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            fadeGroup.alpha = Mathf.Lerp(from, to, k);
            yield return null;
        }

        fadeGroup.alpha = to;
    }
}
