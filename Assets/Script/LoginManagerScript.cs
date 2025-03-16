using System;
using UnityEngine;
using Unity.Netcode;

public class LoginManagerScript : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject leaveButton;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck; // ใช้กำหนดจุดเกิด
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        Debug.Log("Client disconnected: " + clientId);
        if (NetworkManager.Singleton.IsClient)
        {
            Leave();
        }
    }

    public void Leave()
    {
        NetworkManager.Singleton.Shutdown();
        loginPanel.SetActive(true);
        leaveButton.SetActive(false);
    }

    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log("Client connected: " + clientId);
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginPanel.SetActive(false);
            leaveButton.SetActive(true);
        }
    }

    private void HandleServerStarted()
    {
        Debug.Log("Server started");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void SetSpawnLocation(ulong clientId, NetworkManager.ConnectionApprovalResponse response)
    {
        Vector2 spawnPos = Vector2.zero;
        Quaternion spawnRot = Quaternion.identity;

        // server
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            spawnPos = new Vector2(-4, 1);
            spawnRot = Quaternion.Euler(0, 0f, 0);
        }
        else
        {
            switch (NetworkManager.Singleton.ConnectedClients.Count)
            {
                case 1:
                    spawnPos = new Vector2(4, 1);
                    spawnRot = Quaternion.Euler(0, 0f, 0);
                    break;
            }
        }
        response.Position = new Vector3(spawnPos.x, spawnPos.y, 0);
        response.Rotation = spawnRot;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;
        bool isApproved = false;
        int byteLength = connectionData.Length;


        if (byteLength > 0)
        {
            isApproved = true; // Assume true for simplicity, you can add more logic if needed
        }
      

        response.Approved = isApproved;
        response.CreatePlayerObject = true;
        response.Position = Vector3.zero + new Vector3(0, 2, 0);
        response.Rotation = Quaternion.identity;

        SetSpawnLocation(clientId, response);
        NetworkLog.LogInfoServer("spawn pos of " + clientId + ": " + response.Position.ToString());
        response.Reason = "Some reason for not approving the client";
        response.Pending = false;
    }
}
