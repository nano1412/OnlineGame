using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Multiplayer.Widgets;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;


public class mainMenuSceneController : MonoBehaviour
{
    public static mainMenuSceneController current;

    public GameObject joinCanvas;
    public GameObject hostCanvas;

    public TMP_Text hostCode;
    public TMP_InputField codeInput;
    public Button joinButton;
    public Button hostButton;

    ISession activeSession;
    ISession ActiveSession
    {
        get => activeSession;
        set {
            activeSession = value;
            Debug.Log($"Active session: {activeSession}");
        }
    }

    const string playerNamePropertyKey = "playerName";
    int playerCount = NetworkManager.Singleton.ConnectedClients.Count;


    void Awake()
    {
        if (current == null)
        {
            current = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        joinButton.onClick.AddListener(JoinSessionByCode);
        hostButton.onClick.AddListener(StartSessionAsHost);
        joinCanvas.SetActive(false);
        hostCanvas.SetActive(false);

        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signin Succeeded PlayerID: {AuthenticationService.Instance.PlayerId}");
        } catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(ActiveSession != null && hostCode != null)
        {
            hostCode.text = ActiveSession.Code;

        }
        if (hostCanvas != null && joinCanvas != null && !hostCanvas.activeSelf && !joinCanvas.activeSelf)
        {
            LeaveSession();
        }
        playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        Debug.Log(playerCount);
        if(playerCount == 2)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
    }

    public void JoinGame()
    {
        //enable join canvas
        joinCanvas.SetActive(true);
        //once player enter correct code move player to next scene as a client
    }

    public void HostGame()
    {
        // start hostting
        // gen and show code
        // move player to next scene
        hostCanvas.SetActive(true);
    }
    async Task<Dictionary<string, PlayerProperty>> GetPlayerProperties()
    {
        var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        var playerNameProperty = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);
        return new Dictionary<string, PlayerProperty> { { playerNamePropertyKey, playerNameProperty } };
    }
    public async void StartSessionAsHost()
    {
        var playerProperties = await GetPlayerProperties();

        var options = new SessionOptions
        {
            MaxPlayers = 2,
            IsLocked = false,
            IsPrivate = false,
            PlayerProperties = playerProperties
        }.WithRelayNetwork();

        activeSession = await MultiplayerService.Instance.CreateSessionAsync(options);
        Debug.Log($"Session {ActiveSession.Id} Join code: {ActiveSession.Code}");
    }

    public async void JoinSessionByCode()
    {
        ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(codeInput.text);
        Debug.Log($"Session {ActiveSession.Id} join");
    }

    public async Task LeaveSession()
    {
        if(activeSession != null)
        {
            try
            {
                await ActiveSession.LeaveAsync();
            }
            catch
            {

            }
            finally
            {
                ActiveSession = null;
            }
        }
    }

}
