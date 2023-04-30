using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Rigidbody rgBody = null;
    [SerializeField] Transform modelRoot = null;
    [SerializeField] AbductionBeam abductionBeam = null;
    [SerializeField] BlowingBeam blowingBeam = null;

    [Header("Movement Settings")]
    [SerializeField] float horizontalForce = 1.0f;
    [SerializeField] float upwardForce = 2.0f;
    [SerializeField] float downwardForce = 8.0f;
    [SerializeField] float gravityForce = 2.0f;
    [SerializeField] float maxHorizontalVelocity = 50.0f;
    [SerializeField] Vector2 maxVerticalVelocity = new Vector2(-1000.0f, 50.0f);

    [Header("Post-Movement Settings")]
    [SerializeField] float slowDownForce = 6.0f;
    [SerializeField] float slowDownOppositeThreshold = 3.0f;
    [SerializeField] float reverseForceMultiplier = 3.0f;
    [SerializeField] bool postMoveExplosiveMode = false;

    [Header("Tilt Settings")]
    [SerializeField] float maxTiltAngle = 50.0f;
    [SerializeField] float velocityToTiltMultiplier = 5.0f;

    [Header("Misc Settings")]
    [SerializeField] float brakeForce = 4.0f;
    [SerializeField] float hoverForce = 1.0f;
    [SerializeField] float maxHoverVelocity = -10.0f;

    public Vector3 Velocity { get { return rgBody.velocity; } }

    Coroutine inputRoutine = null;
    Coroutine velocityCheckRoutine = null;
    Coroutine gravityRoutine = null;
    Coroutine brakeRoutine = null;
    Coroutine tiltRoutine = null;
    Coroutine slowDownRoutine = null;

    public bool IsMovingLeft { get; private set; } = false;
    public bool IsMovingRight { get; private set; } = false;
    public bool IsMovingUp { get; private set; } = false;
    public bool IsMovingDown { get; private set; } = false;
    public bool IsBraking { get; private set; } = false;
    public bool IsSucking { get; private set; } = false;
    public bool IsBlowing { get; private set; } = false;
    public bool IsSlowingDown { get; private set; } = false;
    public Direction SlowDownDirection { get; private set; } = Direction.Still;

    WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

    private void OnEnable()
    {
        SlowDownDirection = Direction.Still;

        if (inputRoutine != null) StopCoroutine(inputRoutine);
        inputRoutine = StartCoroutine(InputRoutine(true));

        if (tiltRoutine != null) StopCoroutine(tiltRoutine);
        tiltRoutine = StartCoroutine(TiltRoutine());

        if (velocityCheckRoutine != null) StopCoroutine(velocityCheckRoutine);
        velocityCheckRoutine = StartCoroutine(VelocityCheckRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator VelocityCheckRoutine()
    {
        while (true)
        {
            rgBody.velocity = new Vector3(Mathf.Clamp(rgBody.velocity.x, -maxHorizontalVelocity, maxHorizontalVelocity),
                                            Mathf.Clamp(rgBody.velocity.y, maxVerticalVelocity.x, maxVerticalVelocity.y), 0.0f);
            yield return null;
        }
    }

    IEnumerator GravityRoutine()
    {
        while (true)
        {
            rgBody.AddForce(Vector3.down * gravityForce);
            yield return null;
        }
    }

    IEnumerator SlowDownRoutine(Direction slowDownDirection)
    {
        bool breakFirstLoop = false;

        IsSlowingDown = true;

        while (true)
        {
            switch (slowDownDirection)
            {
                case Direction.Left:

                    rgBody.AddForce(Vector3.left * slowDownForce);
                    if (rgBody.velocity.x < -slowDownOppositeThreshold)
                        breakFirstLoop = true;

                    break;

                case Direction.Right:

                    rgBody.AddForce(Vector3.right * slowDownForce);
                    if (rgBody.velocity.x > slowDownOppositeThreshold)
                        breakFirstLoop = true;

                    break;
            }

            if (breakFirstLoop) break;

            yield return null;
        }

        IsSlowingDown = false;
    }

    IEnumerator TiltRoutine()
    {
        while (true)
        {
            float targetAngle = Mathf.Abs(rgBody.velocity.x) * velocityToTiltMultiplier;
            targetAngle = (rgBody.velocity.x > 0.0f) ? -targetAngle : targetAngle;

            modelRoot.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Clamp(targetAngle, -maxTiltAngle, maxTiltAngle));
            yield return null;
        }
    }

    IEnumerator BrakeRoutine()
    {
        bool isBalancingVertically = rgBody.velocity.y < maxHoverVelocity;
        bool isBalancingLeft = rgBody.velocity.x < 0.0f;
        bool isBalancingRight = rgBody.velocity.x > 0.0f;

        while (true)
        {
            if (isBalancingVertically) rgBody.AddForce(Vector3.up * brakeForce);
            if (isBalancingLeft) rgBody.AddForce(Vector3.right * brakeForce);
            if (isBalancingRight) rgBody.AddForce(Vector3.left * brakeForce);

            if (isBalancingLeft && rgBody.velocity.x >= 0.0f) isBalancingLeft = false;
            if (isBalancingRight && rgBody.velocity.x <= 0.0f) isBalancingRight = false;

            if (!isBalancingVertically) rgBody.AddForce(Vector3.down * hoverForce);
            else if (isBalancingVertically && rgBody.velocity.y >= maxHoverVelocity) isBalancingVertically = false;

            rgBody.velocity = new Vector3(!isBalancingLeft && !isBalancingRight ? Mathf.Clamp(rgBody.velocity.x, 0.0f, 0.0f) : rgBody.velocity.x,
                                            !isBalancingVertically ? Mathf.Clamp(rgBody.velocity.y, maxHoverVelocity, 0.0f) : rgBody.velocity.y, 0.0f);

            yield return null;
        }
    }

    IEnumerator InputRoutine(bool waitForInput)
    {
        IsMovingLeft = false;
        IsMovingRight = false;
        IsMovingUp = false;
        IsMovingDown = false;

        if (waitForInput)
        {
            while (true)
            {
                if (Input.anyKeyDown) break; 
                yield return null;
            }
        }

        if (gravityRoutine != null) StopCoroutine(gravityRoutine);
        gravityRoutine = StartCoroutine(GravityRoutine());

        while (true)
        {
            if (PlayerInput.IsKeyArrayDown(PlayerInput.BrakeKeys, () => {

                IsMovingLeft = false;
                IsMovingRight = false;
                IsMovingUp = false;
                IsMovingDown = false;

                abductionBeam.DeactivateRay();

                if (gravityRoutine != null) StopCoroutine(gravityRoutine);
                if (slowDownRoutine != null) StopCoroutine(slowDownRoutine);
                IsSlowingDown = false;

                if (!IsBraking)
                {
                    if (brakeRoutine != null) StopCoroutine(brakeRoutine);
                    brakeRoutine = StartCoroutine(BrakeRoutine());
                }

                IsBraking = true;
            }))
            {
                yield return fixedUpdate;
                continue;
            }
            else
            {
                if (IsBraking)
                {
                    if (brakeRoutine != null) StopCoroutine(brakeRoutine);

                    if (gravityRoutine != null) StopCoroutine(gravityRoutine);
                    gravityRoutine = StartCoroutine(GravityRoutine());
                }

                IsBraking = false;
            }

            if (!PlayerInput.IsKeyArrayDown(PlayerInput.LeftMoveKeys, () => { 
                rgBody.AddForce(Vector3.left * horizontalForce);
                IsMovingLeft = true;
                if (slowDownRoutine != null) StopCoroutine(slowDownRoutine);
                IsSlowingDown = false;

            }))
            {
                if (IsMovingLeft)
                {
                    if (postMoveExplosiveMode)
                        rgBody.AddForce(Vector3.right * Mathf.Abs(rgBody.velocity.x) * reverseForceMultiplier, ForceMode.Impulse);
                    else
                    {
                        if (slowDownRoutine != null) StopCoroutine(slowDownRoutine);
                        slowDownRoutine = StartCoroutine(SlowDownRoutine(Direction.Right));
                    }
                }
                IsMovingLeft = false;
            }

            if (!IsMovingLeft)
            {
                if (!PlayerInput.IsKeyArrayDown(PlayerInput.RightMoveKeys, () => { 
                    rgBody.AddForce(Vector3.right * horizontalForce); 
                    IsMovingRight = true;
                    if (slowDownRoutine != null) StopCoroutine(slowDownRoutine);
                    IsSlowingDown = false;

                }))
                {
                    if (IsMovingRight)
                    {
                        if (postMoveExplosiveMode) 
                            rgBody.AddForce(Vector3.left * Mathf.Abs(rgBody.velocity.x) * reverseForceMultiplier, ForceMode.Impulse);
                        else
                        {
                            if (slowDownRoutine != null) StopCoroutine(slowDownRoutine);
                            slowDownRoutine = StartCoroutine(SlowDownRoutine(Direction.Left));
                        }
                    }
                    IsMovingRight = false;
                }
            }
           
            if (!PlayerInput.IsKeyArrayDown(PlayerInput.UpMoveKeys, () => {
                rgBody.AddForce(Vector3.up * upwardForce);
                if (gravityRoutine != null) StopCoroutine(gravityRoutine);
                IsMovingUp = true;
            }))
            {
                if (IsMovingUp)
                {
                    if (gravityRoutine != null) StopCoroutine(gravityRoutine);
                    gravityRoutine = StartCoroutine(GravityRoutine());
                }

                IsMovingUp = false;
            }

            if (!IsMovingUp)
            {
                if (!PlayerInput.IsKeyArrayDown(PlayerInput.DownMoveKeys, () => {
                    rgBody.AddForce(Vector3.down * downwardForce);
                    if (gravityRoutine != null) StopCoroutine(gravityRoutine);
                    IsMovingDown = true;
                }))
                {
                    if (IsMovingDown)
                    {
                        if (gravityRoutine != null) StopCoroutine(gravityRoutine);
                        gravityRoutine = StartCoroutine(GravityRoutine());
                    }

                    IsMovingDown = false;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (!IsSucking) abductionBeam.ActivateRay();
                IsSucking = true;
            }
            else
            {
                if (IsSucking) abductionBeam.DeactivateRay();
                IsSucking = false;
            }

            if (Input.GetMouseButton(1))
            {
                if (!IsBlowing) blowingBeam.ActivateRay();
                IsBlowing = true;
            }
            else
            {
                if (IsBlowing) blowingBeam.DeactivateRay();
                IsBlowing = false;
            }

            yield return fixedUpdate;
        }

    }
}