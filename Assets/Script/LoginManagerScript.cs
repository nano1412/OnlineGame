using System;
using UnityEngine;
using Unity.Netcode;
using TMPro;

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
        NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        ulong clientId = request.ClientNetworkId;

        Vector3 spawnPos;
        Quaternion spawnRot = Quaternion.identity;

        if (NetworkManager.Singleton.ConnectedClients.Count == 1)
        {
            // ฝ่ายผู้กล้า (ซ้าย)
            spawnPos = new Vector3(-4, 1, 0);
        }
        else
        {
            // ฝ่ายปีศาจ (ขวา)
            spawnPos = new Vector3(4, 1, 0);
        }

        response.Position = spawnPos;
        response.Rotation = spawnRot;
        response.Approved = true;
        response.CreatePlayerObject = true;
    }
}
