using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup = null;
    [SerializeField] Animation transitionAnimation = null;

    const string TRANSITION_IN_ANIM = "CanvasTransitionFadeIn";
    const string TRANSITION_OUT_ANIM = "CanvasTransitionFadeOut";

    public static SceneTransitioner Instance;

    WaitForSeconds interDelay = new WaitForSeconds(0.2f);
    WaitForSeconds delay = new WaitForSeconds(0.6f);

    Coroutine transitionRoutine = null;

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            return;
        }

        DontDestroyOnLoad(gameObject.transform.parent.gameObject);
        Instance = this;
    }

    void BeginTransitionToNextScene(string sceneName, bool reallowPlayerInputs)
    {
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(TransitionRoutine(sceneName, reallowPlayerInputs));
    }

    IEnumerator TransitionRoutine(string sceneName, bool reallowPlayerInputs)
    {
        transitionAnimation.Play(TRANSITION_IN_ANIM);

        yield return delay;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        yield return operation;
        yield return interDelay;

        transitionAnimation.Play(TRANSITION_OUT_ANIM);

        yield return delay;

        if (reallowPlayerInputs) PlayerInput.ChangeControlsAllowedState(true);
    }

    public static void StartTransitionToNextScene(string sceneName, bool reallowPlayerInputs = true)
    {
        Instance.BeginTransitionToNextScene(sceneName, reallowPlayerInputs);
    }
}
