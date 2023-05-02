using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    [SerializeField] float delay = 0.15f;

    Coroutine startRoutine = null;
    WaitForSeconds startDelay = null;
    WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

    private void Awake()
    {
        startDelay = new WaitForSeconds(delay);

        if (startRoutine != null) StopCoroutine(startRoutine);
        startRoutine = StartCoroutine(StartRoutine());
    }

    IEnumerator StartRoutine()
    {
        yield return startDelay;

        while (true)
        {
            if (Input.anyKeyDown)
            {
                SceneTransitioner.StartTransitionToNextScene("Level1");
                break;
            }

            yield return null;
        }
    }
}
