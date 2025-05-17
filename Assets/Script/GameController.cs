using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public bool isPC;

    [ServerRpc(RequireOwnership = false)]
    public void PlayerLoseServerRpc(ulong loserClientId)
    {
        Debug.Log("Player " + loserClientId + " แพ้แล้ว!");
        // เพิ่มระบบ Game Over ที่นี่
    }
}
