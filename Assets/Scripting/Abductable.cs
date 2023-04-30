using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abductable : MonoBehaviour
{
    [Header("Abductable Base")]
    [SerializeField] protected int sizeCapacity = 1;
    public int SizeCapacity { get { return sizeCapacity; } }

    [SerializeField] protected float heightDistToDie = 2.0f;
    protected Rigidbody rgBody = null;
    public Rigidbody RgBody { get { return rgBody; } }

    protected const string GROUND_TAG = "Ground";

    public bool IsFalling { get; protected set; } = false;

    public float StartFallHeight { get; protected set; } = 0.0f;
    public float StartZPos { get; protected set; } = 0.0f;

    public Transform OriginalParent { get; protected set; } = null;

    protected Coroutine alignmentCheckRoutine = null;

    protected WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        rgBody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        OriginalParent = transform.parent;
        StartZPos = transform.position.z;

        if (alignmentCheckRoutine != null) StopCoroutine(alignmentCheckRoutine);
        alignmentCheckRoutine = StartCoroutine(AlignmentCheckRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void BeginFalling()
    {
        transform.parent = OriginalParent;
        rgBody.useGravity = true;
        IsFalling = true;
        StartFallHeight = transform.position.y;
    }

    public virtual void StopFalling()
    {
        rgBody.useGravity = false;
        IsFalling = false;
    }

    public virtual void AddForce(Vector3 direction, float force)
    {
        rgBody.AddForce(direction * force);
    }

    public virtual void ClampVerticalVelocity(float min, float max)
    {
        rgBody.velocity = new Vector3(rgBody.velocity.x, Mathf.Clamp(rgBody.velocity.y, min, max), 0.0f);
    }

    public virtual void Die()
    {
        StopAllCoroutines();
        CancelInvoke();

        rgBody.useGravity = false;

        gameObject.SetActive(false);
    }

    IEnumerator AlignmentCheckRoutine()
    {
        while (true)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, StartZPos);
            yield return fixedUpdate;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag(GROUND_TAG) && IsFalling && 
            StartFallHeight - transform.position.y >= heightDistToDie)
        {
            Die();
        }
    }
}
