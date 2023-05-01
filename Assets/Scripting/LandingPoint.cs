using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPoint : MonoBehaviour
{
    [SerializeField] int requiredCargoCount = 1;
    [SerializeField] float transitionTriggerDelay = 0.2f;
    [SerializeField] string sceneToLoadUponComplete = null;
    [SerializeField] Transform dockRoot = null;
    [SerializeField] Animator anim = null;

    public bool isPlayerDocked { get; private set; } = false;

    Coroutine dockCheckRoutine = null;

    const string LANDED_ANIM = "Landed";

    WaitForSeconds transitionDelay = new WaitForSeconds(4.4f);

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator DockCheckRoutine(Rigidbody rgBody)
    {
        float timer = 0.0f;

        while (true)
        {
            if (rgBody.velocity.x < 0.1f && rgBody.velocity.x > -0.1f &&
                rgBody.velocity.y < 0.1f && rgBody.velocity.y > -0.1f)
            {
                timer += Time.deltaTime;
            }
            else timer = 0.0f;

            if (timer >= transitionTriggerDelay) break;

            yield return null;
        }

        isPlayerDocked = true;
        PlayerInput.ChangeControlsAllowedState(false);
        anim.Play(LANDED_ANIM);
        rgBody.transform.parent = dockRoot;

        yield return transitionDelay;

        SceneTransitioner.StartTransitionToNextScene(sceneToLoadUponComplete);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerDocked) return;

        PlayerController playerController = other.attachedRigidbody.GetComponent<PlayerController>();
        if (playerController == null) return;

        if (playerController.CarryingCargoCount < requiredCargoCount) return;

        if (dockCheckRoutine != null) StopCoroutine(dockCheckRoutine);
        dockCheckRoutine = StartCoroutine(DockCheckRoutine(playerController.GetComponent<Rigidbody>()));
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayerDocked) return;

        PlayerController playerController = other.attachedRigidbody.GetComponent<PlayerController>();
        if (playerController == null) return;

        if (dockCheckRoutine != null) StopCoroutine(dockCheckRoutine);
    }
}
