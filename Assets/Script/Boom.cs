using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Boom : NetworkBehaviour
{
    private Transform target;
    public float explosionRadius = 5f;
    public float countdown = 3f;
    private bool hasExploded = false;
    [SerializeField] float lifeTime = 2;
    [SerializeField] float armTime = 2;

    public AudioClip hitSound;
    private AudioSource audioSource;

    private Vector2 initialDirection;
    private float initialForce;
    private bool applyForce;

    public GameObject boomPrefab;
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public void Initialize(Vector2 direction, float force)
    {
        initialDirection = direction;
        initialForce = force;
        applyForce = true;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Invoke(nameof(ExplodeServerRpc), countdown);
        StartCoroutine(EnableColliderAfterDelay(armTime));
    }

    void FixedUpdate()
    {
        lifeTime -= Time.deltaTime;
        armTime -= Time.deltaTime;
        if (applyForce)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = initialDirection * initialForce;
            applyForce = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void ExplodeServerRpc()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            HealthSystem health = collider.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(1); // ลด HP ทีละ 1
            }
        }

        ExplodeClientRpc();
        Destroy(gameObject);
    }

    [ClientRpc(RequireOwnership = false)]
    void ExplodeClientRpc()
    {
        Debug.Log("Boom exploded!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Player") || lifeTime < 0) && armTime < 0)
        {
            ExplodeServerRpc();
        }
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        col.enabled = false;
        yield return new WaitForSeconds(delay);
        col.enabled = true;
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem health = collision.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(1); // ลด HP ทีละ 1
            }

            Destroy(gameObject); // หรือใช้ Despawn ถ้าใช้ NetworkObject
        }
    }*/
}
