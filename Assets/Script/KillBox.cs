using UnityEngine;
using Unity.Netcode;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var networkObject = other.GetComponent<NetworkObject>();
            if (networkObject != null && NetworkManager.Singleton.IsServer)
            {
                // เรียก Game Over ทันที โดยส่ง clientId ไปให้ GameController
                GameController.Instance.ForceGameOver(networkObject.OwnerClientId);
            }
        }
    }
}