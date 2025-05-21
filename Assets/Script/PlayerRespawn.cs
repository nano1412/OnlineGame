using Unity.Netcode;
using UnityEngine;

public class PlayerRespawn : NetworkBehaviour
{
    public void RequestRespawn()
    {
        if (IsOwner)
        {
            RespawnServerRpc();
        }
    }

    [ServerRpc]
    private void RespawnServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = OwnerClientId;
        Vector3 spawnPosition = LoginManager.Instance.GetSpawnPositionForClient(clientId);
        RespawnClientRpc(spawnPosition);
    }

    [ClientRpc]
    private void RespawnClientRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}
