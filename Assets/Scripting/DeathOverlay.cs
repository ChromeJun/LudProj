using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathOverlay : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup = null;
    [SerializeField] Animator anim = null;
    [SerializeField] Typewriter typewriter = null;

    [SerializeField] float transitionDelay = 5.0f;

    const string PLAY_ANIM = "Play";

    Coroutine skipCheckRoutine = null;
    WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

    public void ShowCanvas()
    {
        PlayerInput.ChangeControlsAllowedState(false);

        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        anim.Play(PLAY_ANIM);

        if (skipCheckRoutine != null) StopCoroutine(skipCheckRoutine);
        skipCheckRoutine = StartCoroutine(SkipCheckRoutine());
    }

    IEnumerator SkipCheckRoutine()
    {
        while (true)
        {
            if (typewriter.IsDoneTyping) break;
            yield return endOfFrame;
        }

        float timer = 0.0f;

        while (true)
        {
            timer += Time.deltaTime;

            if (Input.anyKeyDown)
            {
                timer = transitionDelay;
            }

            if (timer >= transitionDelay) break;

            yield return null;
        }

        SceneTransitioner.StartTransitionToNextScene(SceneManager.GetActiveScene().name);
    }
}
