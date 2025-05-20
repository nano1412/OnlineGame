using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }
    public bool isPC;

   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        HealthSystem.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        HealthSystem.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath(ulong clientId)
    {
        // เรียก ServerRpc เพื่อจัดการกรณีผู้เล่นแพ้
        if (NetworkManager.Singleton.IsServer)
        {
            PlayerLoseServerRpc(clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerLoseServerRpc(ulong losingClientId)
    {
        Debug.Log("Player " + losingClientId + " แพ้แล้ว!");

        // TODO: ใส่โค้ดจัดการเมื่อแพ้ เช่น แสดง UI แพ้, รีสตาร์ท, เปลี่ยนฉาก ฯลฯ
    }
}
