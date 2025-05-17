using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public bool isPC;

    [ServerRpc(RequireOwnership = false)]
    public void PlayerLoseServerRpc(ulong loserClientId)
    {
        Debug.Log("Player " + loserClientId + " ������!");
        // �����к� Game Over �����
    }
}
