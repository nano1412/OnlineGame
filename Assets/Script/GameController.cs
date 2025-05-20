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
        // ���¡ ServerRpc ���ͨѴ��áóռ�������
        if (NetworkManager.Singleton.IsServer)
        {
            PlayerLoseServerRpc(clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerLoseServerRpc(ulong losingClientId)
    {
        Debug.Log("Player " + losingClientId + " ������!");

        // TODO: ����鴨Ѵ���������� �� �ʴ� UI ��, ��ʵ���, ����¹�ҡ ���
    }
}
