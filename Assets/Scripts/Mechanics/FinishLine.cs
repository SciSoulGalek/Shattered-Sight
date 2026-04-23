using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class FinishLine : MonoBehaviour
{
    [Header("Level Info")]
    public int thisLevelIndex = 1;
    public string nextLevelSceneName = "";

    private bool _triggered;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;
        StartCoroutine(FinishRoutine(other));
    }

    private IEnumerator FinishRoutine(Collider2D player)
    {
        var controller = player.GetComponent<PlayerController2D>();
        if (controller != null)
            controller.enabled = false;

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        float wait = 0.3f;
        var effects = player.GetComponent<PlayerEffects>();
        if (effects != null)
        {
            effects.PlayFinish();
            if (effects.finishAnimDuration > 0f)
                wait = effects.finishAnimDuration;
        }

        GameProgress.CompleteLevel(thisLevelIndex);

        if (wait > 0f)
            yield return new WaitForSeconds(wait);

        if (LevelTransition.Instance != null)
        {
            if (!string.IsNullOrEmpty(nextLevelSceneName))
                LevelTransition.Instance.FadeToScene(nextLevelSceneName);
            else
                LevelTransition.Instance.FadeToScene("MainMenu");
        }
        else
        {
            if (!string.IsNullOrEmpty(nextLevelSceneName))
                SceneManager.LoadScene(nextLevelSceneName);
            else
                SceneManager.LoadScene("MainMenu");
        }
    }
}
