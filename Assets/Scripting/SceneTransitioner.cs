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

    WaitForSeconds delay = new WaitForSeconds(0.36f);

    Coroutine transitionRoutine = null;

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void BeginTransitionToNextScene(string sceneName)
    {
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(TransitionRoutine(sceneName));
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        transitionAnimation.Play(TRANSITION_IN_ANIM);

        yield return delay;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        yield return operation;

        transitionAnimation.Play(TRANSITION_OUT_ANIM);

        yield return delay;
    }

    public static void StartTransitionToNextScene(string sceneName)
    {
        Instance.BeginTransitionToNextScene(sceneName);
    }
}
