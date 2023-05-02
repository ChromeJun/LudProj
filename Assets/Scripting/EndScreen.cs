using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] float delay = 35.0f;

    Coroutine startRoutine = null;
    WaitForSeconds startDelay = null;

    private void Awake()
    {
        startDelay = new WaitForSeconds(delay);

        if (startRoutine != null) StopCoroutine(startRoutine);
        startRoutine = StartCoroutine(StartRoutine());
    }

    IEnumerator StartRoutine()
    {
        yield return startDelay;

        SceneTransitioner.StartTransitionToNextScene("TitleCard");
    }
}
