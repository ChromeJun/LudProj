using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [ContextMenuItem("Display Borders", "DisplayBorders")]

    [Header("BoundarySettings")]
    [SerializeField] float objectDepthVertical = 0.45f;
    [SerializeField] float objectDepthHorizontal = 1.0f;
    [SerializeField] Vector2 startTrackOffsets = new Vector2(6.0f, 10.0f);
    [SerializeField] Vector2 stopTrackOffsets = new Vector2(9.0f, 10.0f);
    [SerializeField] Vector2 stickTrackOffsets = new Vector2(1.0f, 1.0f);
    [SerializeField] Vector2 horizontalLimits = new Vector2(-20.0f, 20.0f);
    [SerializeField] Vector2 verticalLimits = new Vector2(5.0f, 100.0f);

    [Header("Movement Settings")]
    [SerializeField] float horizontalForce = 15.0f;
    [SerializeField] float verticalForce = 15.0f;
    [SerializeField] float stickHorizontalForce = 17.0f;
    [SerializeField] float stickVerticalForce = 17.0f;
    [SerializeField] float reverseHorizontalForce = 30.0f;
    [SerializeField] float reverseVerticalForce = 30.0f;

    [Header("Border Markers - FOR DEBUGGING")]
    [SerializeField] Transform[] rightMarkers = null;
    [SerializeField] Transform[] leftMarkers = null;
    [SerializeField] Transform[] topMarkers = null;
    [SerializeField] Transform[] bottomMarkers = null;

    Camera cam = null;
    Rigidbody rgBody = null;
    PlayerController playerController = null;

    Coroutine trackPlayerRoutine = null;
    Coroutine followPlayerRoutine = null;
    Coroutine clampViewRoutine = null;

    WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

    public Vector3 TopEdge { get; private set; } = Vector3.zero;
    public Vector3 RightEdge { get; private set; } = Vector3.zero;
    public Vector3 BottomEdge { get; private set; } = Vector3.zero;
    public Vector3 LeftEdge { get; private set; } = Vector3.zero;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        rgBody = GetComponent<Rigidbody>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        if (trackPlayerRoutine != null) StopCoroutine(trackPlayerRoutine);
        trackPlayerRoutine = StartCoroutine(TrackPlayerRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void PlaceObjectsAtEdges()
    {
        
    }

    private void CalculateScreenEdges()
    {
        float aspectRatio = cam.aspect;
        float fov = cam.fieldOfView;
        float halfFovRad = Mathf.Deg2Rad * fov * 0.5f;
        float distanceFromCameraVertical = objectDepthVertical / Mathf.Tan(halfFovRad);
        float distanceFromCameraHorizontal = objectDepthHorizontal / Mathf.Tan(halfFovRad);
        Vector3 cameraPosition = cam.transform.position;

        // Top edge
        TopEdge = cameraPosition + cam.transform.forward * distanceFromCameraVertical + cam.transform.up * distanceFromCameraVertical * aspectRatio;

        // Bottom edge
        BottomEdge = cameraPosition + cam.transform.forward * distanceFromCameraVertical - cam.transform.up * distanceFromCameraVertical * aspectRatio;

        // Left edge
        LeftEdge = cameraPosition + cam.transform.forward * distanceFromCameraHorizontal - cam.transform.right * distanceFromCameraHorizontal;

        // Right edge
        RightEdge = cameraPosition + cam.transform.forward * distanceFromCameraHorizontal + cam.transform.right * distanceFromCameraHorizontal;
    }

    void DisplayBorders()
    {
        cam = GetComponent<Camera>();
        playerController = FindObjectOfType<PlayerController>();

        CalculateScreenEdges();

        rightMarkers[0].position = new Vector3(RightEdge.x, cam.transform.position.y, playerController.transform.position.z);
        rightMarkers[1].position = new Vector3(RightEdge.x - startTrackOffsets.x, cam.transform.position.y, playerController.transform.position.z);
        rightMarkers[2].position = new Vector3(RightEdge.x - stopTrackOffsets.x, cam.transform.position.y, playerController.transform.position.z);
        rightMarkers[3].position = new Vector3(RightEdge.x - stickTrackOffsets.x, cam.transform.position.y, playerController.transform.position.z);

        leftMarkers[0].position = new Vector3(LeftEdge.x, cam.transform.position.y, playerController.transform.position.z);
        leftMarkers[1].position = new Vector3(LeftEdge.x + startTrackOffsets.x, cam.transform.position.y, playerController.transform.position.z);
        leftMarkers[2].position = new Vector3(LeftEdge.x + stopTrackOffsets.x, cam.transform.position.y, playerController.transform.position.z);
        leftMarkers[3].position = new Vector3(LeftEdge.x + stickTrackOffsets.x, cam.transform.position.y, playerController.transform.position.z);

        topMarkers[0].position = new Vector3(cam.transform.position.x, TopEdge.y, playerController.transform.position.z);
        topMarkers[1].position = new Vector3(cam.transform.position.x, TopEdge.y - startTrackOffsets.y, playerController.transform.position.z);
        topMarkers[2].position = new Vector3(cam.transform.position.x, TopEdge.y - stopTrackOffsets.y, playerController.transform.position.z);
        topMarkers[3].position = new Vector3(cam.transform.position.x, TopEdge.y - stickTrackOffsets.y, playerController.transform.position.z);

        bottomMarkers[0].position = new Vector3(cam.transform.position.x, BottomEdge.y, playerController.transform.position.z);
        bottomMarkers[1].position = new Vector3(cam.transform.position.x, BottomEdge.y + startTrackOffsets.y, playerController.transform.position.z);
        bottomMarkers[2].position = new Vector3(cam.transform.position.x, BottomEdge.y + stopTrackOffsets.y, playerController.transform.position.z);
        bottomMarkers[3].position = new Vector3(cam.transform.position.x, BottomEdge.y + stickTrackOffsets.y, playerController.transform.position.z);
    }

    void StartFollowingPlayer()
    {
        if (trackPlayerRoutine != null) StopCoroutine(trackPlayerRoutine);

        if (followPlayerRoutine != null) StopCoroutine(followPlayerRoutine);
        followPlayerRoutine = StartCoroutine(FollowPlayerRoutine());

        if (clampViewRoutine != null) StopCoroutine(clampViewRoutine);
        clampViewRoutine = StartCoroutine(ClampViewRoutine());
    }

    void StopFollowingPlayer(bool startTrackingPlayer = true)
    {
        if (followPlayerRoutine != null) StopCoroutine(followPlayerRoutine);

        if (!startTrackingPlayer) return;

        if (trackPlayerRoutine != null) StopCoroutine(trackPlayerRoutine);
        trackPlayerRoutine = StartCoroutine(TrackPlayerRoutine());
    }

    IEnumerator ClampViewRoutine()
    {
        while (true)
        {
            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, horizontalLimits.x, horizontalLimits.y),
                                                    Mathf.Clamp(cam.transform.position.y, verticalLimits.x, verticalLimits.y), cam.transform.position.z);

            yield return fixedUpdate;
        }
    }

    IEnumerator TrackPlayerRoutine()
    {
        while (true)
        {
            yield return null;

            CalculateScreenEdges();

            if (playerController.transform.position.x > RightEdge.x - startTrackOffsets.x || playerController.transform.position.x < LeftEdge.x + startTrackOffsets.x ||
                playerController.transform.position.y > TopEdge.y - startTrackOffsets.y || playerController.transform.position.y < BottomEdge.y + startTrackOffsets.y)
            {
                /*
                if (cam.transform.position.x - 0.5f <= horizontalLimits.x && playerController.transform.position.x < topLeft.x + startTrackOffsets.x)
                {
                    continue;
                }
                if (cam.transform.position.x + 0.5f >= horizontalLimits.y && playerController.transform.position.x > topRight.x - startTrackOffsets.x)
                {
                    continue;
                }
                if (cam.transform.position.y - 0.5f <= verticalLimits.x && playerController.transform.position.y < bottomLeft.y + startTrackOffsets.y)
                {
                    continue;
                }
                if (cam.transform.position.y + 0.5f >= verticalLimits.y && playerController.transform.position.y > topLeft.y - startTrackOffsets.y)
                {
                    continue;
                }
                */
                break;
            }
        }

        StartFollowingPlayer();
    }

    IEnumerator FollowPlayerRoutine()
    {
        bool isMovingLeft = false;
        bool IsMovingRight = false;
        bool IsMovingUp = false;
        bool IsMovingDown = false;

        while (true)
        {
            DisplayBorders();

            if (playerController.transform.position.x < LeftEdge.x + startTrackOffsets.x && !isMovingLeft) isMovingLeft = true;
            if (playerController.transform.position.x > RightEdge.x - startTrackOffsets.x && !IsMovingRight) IsMovingRight = true;
            if (playerController.transform.position.y < BottomEdge.y + startTrackOffsets.y && !IsMovingDown) IsMovingDown = true;
            if (playerController.transform.position.y > TopEdge.y - startTrackOffsets.y && !IsMovingUp) IsMovingUp = true;

            if (isMovingLeft)
            {
                if (playerController.transform.position.x <= LeftEdge.x + stickTrackOffsets.x)
                    rgBody.AddForce(Vector3.left * stickHorizontalForce);
                else
                    rgBody.AddForce(Vector3.left * horizontalForce);

                if (playerController.transform.position.x >= LeftEdge.x + stopTrackOffsets.x)
                {
                    rgBody.AddForce(Vector3.right * reverseHorizontalForce);
                    isMovingLeft = false;
                }
            }
            
            if (IsMovingRight)
            {
                if (playerController.transform.position.x >= RightEdge.x - stickTrackOffsets.x)
                    rgBody.AddForce(Vector3.right * stickHorizontalForce);
                else
                    rgBody.AddForce(Vector3.right * horizontalForce);

                if (playerController.transform.position.x <= RightEdge.x - stopTrackOffsets.x)
                {
                    rgBody.AddForce(Vector3.left * reverseHorizontalForce);
                    IsMovingRight = false;
                }
            }

            if (IsMovingDown)
            {
                if (playerController.transform.position.y <= BottomEdge.y + stickTrackOffsets.y)
                    rgBody.AddForce(Vector3.down * stickVerticalForce);
                else
                    rgBody.AddForce(Vector3.down * verticalForce);

                if (playerController.transform.position.y >= BottomEdge.y + stopTrackOffsets.y)
                {
                    rgBody.AddForce(Vector3.up * reverseVerticalForce);
                    IsMovingDown = false;
                }
            }

            if (IsMovingUp)
            {
                if (playerController.transform.position.y >= TopEdge.y - stickTrackOffsets.y)
                    rgBody.AddForce(Vector3.up * stickVerticalForce);
                else
                    rgBody.AddForce(Vector3.up * verticalForce);

                if (playerController.transform.position.y <= TopEdge.y - stopTrackOffsets.y)
                {
                    rgBody.AddForce(Vector3.down * reverseVerticalForce);
                    IsMovingUp = false;
                }
            }


            if (!isMovingLeft && !IsMovingRight && !IsMovingUp && !IsMovingDown) break;

            yield return fixedUpdate;
        }

        StopFollowingPlayer();
    }

}
