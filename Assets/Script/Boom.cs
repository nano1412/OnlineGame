using UnityEngine;
using Unity.Netcode;

public class Boom : NetworkBehaviour
{
    private Transform target;
    public float speed = 70f;
    public float damage = 10;
    public float explosionRadius = 5f;
    public float countdown = 3f; // ระเบิดหลังจาก 3 วินาที
    private bool hasExploded = false;

    public AudioClip hitSound;
    private AudioSource audioSource;

    public GameObject boomPrefab;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Invoke(nameof(ExplodeServerRpc), countdown);
    }

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            ExplodeServerRpc();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    [ServerRpc]
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
            Playermovement player = collider.GetComponent<Playermovement>();
            if (player != null)
            {
                player.TakeDamageServerRpc(damage);
            }
        }

        ExplodeClientRpc();
        Destroy(gameObject);
    }

    [ClientRpc]
    void ExplodeClientRpc()
    {
        // เพิ่มเอฟเฟกต์ระเบิดที่ทุกไคลเอนต์เห็นพร้อมกัน
        Debug.Log("Boom exploded!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Playermovement player = collision.GetComponent<Playermovement>();
        if (player != null)
        {
            ExplodeServerRpc();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Playermovement player = collision.gameObject.GetComponent<Playermovement>();
        if (player != null)
        {
            ExplodeServerRpc();
        }
    }
}
