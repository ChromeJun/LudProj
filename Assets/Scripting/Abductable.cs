using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abductable : MonoBehaviour
{
    [Header("Abductable Base")]
    [SerializeField] protected AbductType abductType = AbductType.Debris;
    public AbductType AbductType { get { return abductType; } }
    [SerializeField] protected int sizeCapacity = 1;
    public int SizeCapacity { get { return sizeCapacity; } }

    protected Rigidbody rgBody = null;
    public Rigidbody RgBody { get { return rgBody; } }

    public bool IsFalling { get; protected set; } = false;

    public float StartZPos { get; protected set; } = 0.0f;
    public Vector3 StartSuckPos { get; protected set; } = Vector3.zero;

    public Transform OriginalParent { get; protected set; } = null;
    public Vector3 OriginalScale { get; protected set; } = Vector3.one;

    protected Coroutine alignmentCheckRoutine = null;

    protected WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        rgBody = GetComponent<Rigidbody>();
        OriginalScale = transform.localScale;
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
        transform.localScale = OriginalScale;
        transform.parent = OriginalParent;
        rgBody.useGravity = true;
        IsFalling = true;
    }

    public virtual void StopFalling()
    {
        rgBody.isKinematic = false;
        StartSuckPos = transform.position;
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

    }
}
