using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.Netcode;
//using QFSW.QC;
using TMPro;
using Debug = UnityEngine.Debug;

public class LoginManagerScript : MonoBehaviour
{
    public TMP_InputField userNameInputField;
    public TMP_InputField characterIdInputField;
    public List<uint> AlternatePlayerPrefabs;
    public GameObject loginPanel;
    public GameObject leaveButton;
    //public GameObject scorePanel;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConntected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        SetUIVisible(false);
    }

    public void SetUIVisible(bool isUserLogin)
    {
        if (isUserLogin)
        {
            loginPanel.SetActive(false); leaveButton.SetActive(true); //scorePanel.SetActive(true);
        }
        else
        {
            loginPanel.SetActive(true); leaveButton.SetActive(false); //scorePanel.SetActive(false);
        }
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        //Debug.Log("client disconnect Id = " + clientId);
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("client disconnect");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Leave();
        }
    }

    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Host is disconnected");
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        else if(NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Client is disconnected");
            NetworkManager.Singleton.Shutdown();
        }
        // loginPanel.SetActive(true);
        // leaveButton.SetActive(false);
        SetUIVisible(false);
    }

    private void HandleClientConntected(ulong clientId)
    {
        Debug.Log("clientId = " + clientId);
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            // loginPanel.SetActive(false);
            // leaveButton.SetActive(true);
            SetUIVisible(true);
        }
    }

    private void HandleServerStarted()
    {
        Debug.Log("Server started");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) {return;}

        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConntected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    private bool isApproveConnection = false;

    //[Command("set-approve")]
    public bool SetIsApproveConnection()
    {
        isApproveConnection = !isApproveConnection;
        return isApproveConnection;
    }
    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        string userName = userNameInputField.GetComponent<TMP_InputField>().text;
        string characterId = characterIdInputField.GetComponent<TMP_InputField>().text;
        string[] inputFields = { userName, characterId };
        string clientData = HelperScript.CombineStrings(inputFields);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = 
            System.Text.Encoding.ASCII.GetBytes(clientData);
        NetworkManager.Singleton.StartClient();
    }

    public bool appoveConnection(string clientData, string hostData)
    {
        Debug.Log("Client Data = " + clientData);
        Debug.Log("Host Data = " + hostData);
        bool isApprove = System.String.Equals(clientData.Trim(),hostData.Trim()) ? false : true;
        Debug.Log("IsApprove = " + isApprove);
        return isApprove;
    }

    private void SetSpawnLocation(ulong clientId, NetworkManager.ConnectionApprovalResponse response)
    {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        
        // server
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            spawnPos = new Vector3(5f, 1f, 0f);
            spawnRot = Quaternion.Euler(0, 135f, 0);
        }
        else
        {
            switch (NetworkManager.Singleton.ConnectedClients.Count)
            {
                case 1:
                    //spawnPos = new Vector3(-2, 1, 0);
                    spawnPos = new Vector3(5f, 1f, 0f);
                    spawnRot = Quaternion.Euler(0, 100f, 0);
                    break;
                case 2:
                    //spawnPos = new Vector3(-4, 1, 0);
                    spawnPos = new Vector3(5f, 1f, 0f);
                    spawnRot = Quaternion.Euler(0, 255f, 0);
                    break;
            }
        }
        response.Position = spawnPos;
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
        int characterPrefabIndex = 0;
        if (byteLength > 0)
        {
            string combinedString = System.Text.Encoding.ASCII.GetString(connectionData, 0, byteLength);
            string[] extractedStrings = HelperScript.ExtractStrings(combinedString);
            for (int i = 0; i < extractedStrings.Length; i++)
            {
                if (i == 0)
                {
                    string clientData = extractedStrings[i];
                    string hostData = userNameInputField.GetComponent<TMP_InputField>().text;
                    isApproved = appoveConnection(clientData, hostData);     
                }
                else if (i == 1)
                {
                    characterPrefabIndex = int.Parse(extractedStrings[i]);
                } }}
        else
        {
            if (NetworkManager.Singleton.IsHost)
            {
                string characterId = characterIdInputField.GetComponent<TMP_InputField>().text;
                characterPrefabIndex = int.Parse(characterId);
            }
        }
        // if (byteLength > 0)
        // {
        //     string clientData = System.Text.Encoding.ASCII.GetString(connectionData, 0, byteLength);
        //     string hostData = userNameInputField.GetComponent<TMP_InputField>().text;
        //     isApproved = appoveConnection(clientData, hostData);
        // }

        // Your approval logic determines the following values
        response.Approved = isApproved;
        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        //response.PlayerPrefabHash = 2774027741; // blue
        //response.PlayerPrefabHash = 4218494367; // yellow
        response.PlayerPrefabHash = AlternatePlayerPrefabs[characterPrefabIndex];

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = Vector3.zero + new Vector3(0,2,0);

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;
        
        SetSpawnLocation(clientId, response);
        NetworkLog.LogInfoServer("spawn pos of " + clientId + ": " + response.Position.ToString());

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        response.Reason = "Some reason for not approving the client";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }
}
