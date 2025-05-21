using Unity.Netcode;
using Unity.Netcode.Components;
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

        // ✅ ป้องกัน null
        if (LoginManager.Instance == null)
        {
            Debug.LogError("LoginManager.Instance is null!");
            return;
        }

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

        // ถ้าใช้ NetworkTransform
        var netTransform = GetComponent<NetworkTransform>();
        if (netTransform != null)
        {
            netTransform.Teleport(transform.position, transform.rotation, transform.localScale);
        }
    }
}
