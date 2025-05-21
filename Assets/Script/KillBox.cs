using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(1);
            }

            var respawn = other.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                respawn.RequestRespawn();
            }
        }
    }
}