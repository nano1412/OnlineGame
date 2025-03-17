using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class LoginManager : MonoBehaviour
{
    //public TMP_InputField userNameInputField;
    //public TMP_InputField characterIdInputField;
    public List<uint> AlternatePlayerPrefabs;
    public GameObject loginPanel;
    public GameObject leaveButton;
    public GameObject hostButton;
    public GameObject clientButton;
    //public GameObject scorePanel;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        SetUIVisible(false);
    }

    public void SetUIVisible(bool isUserLogin)
    {
        loginPanel.SetActive(!isUserLogin);
        leaveButton.SetActive(isUserLogin);
        //scorePanel.SetActive(isUserLogin);
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Leave();
        }
    }

    public void Leave()
    {
        NetworkManager.Singleton.Shutdown();
        SetUIVisible(false);
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            SetUIVisible(true);
        }
    }

    private void HandleServerStarted()
    {
        Debug.Log("Server started");
    }

    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        //string userName = userNameInputField.text;
        //string characterId = characterIdInputField.text;
        //string[] inputFields = { userName, characterId };
        //string clientData = string.Join("|", inputFields);
        //NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(clientData);
        NetworkManager.Singleton.StartClient();
    }
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        ulong clientId = request.ClientNetworkId;
        bool isApproved = true;
        int characterPrefabIndex = 0;

        // กำหนดตำแหน่งตัวละครตามว่าเป็น Host หรือ Client
        if (NetworkManager.Singleton.IsHost)
        {
            response.Position = new Vector3(-5f, 1f, 0f); // Host อยู่ทางซ้าย
        }
        else
        {
            response.Position = new Vector3(5f, 1f, 0f);  // Client อยู่ทางขวา
        }

        // เลือก PlayerPrefab
        response.PlayerPrefabHash = AlternatePlayerPrefabs[characterPrefabIndex];
        response.Approved = isApproved;
        response.CreatePlayerObject = true;
        response.Rotation = Quaternion.identity;
    }
    /*private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        ulong clientId = request.ClientNetworkId;
        bool isApproved = true;
        int characterPrefabIndex = 0;

        if (request.Payload.Length > 0)
        {
            string receivedData = System.Text.Encoding.ASCII.GetString(request.Payload);
            string[] extractedData = receivedData.Split('|');
            if (extractedData.Length > 1)
            {
                int.TryParse(extractedData[1], out characterPrefabIndex);
            }
        }

        response.Approved = isApproved;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = AlternatePlayerPrefabs[characterPrefabIndex];

        // ตั้งตำแหน่งเริ่มต้นจากเซิร์ฟเวอร์ที่นี่
        response.Position = new Vector3(-5f, 1f, 0f);  // ตำแหน่งซ้ายสุด
        response.Rotation = Quaternion.identity;
    }*/

}
