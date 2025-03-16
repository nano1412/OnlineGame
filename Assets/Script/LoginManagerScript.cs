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
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }
}
