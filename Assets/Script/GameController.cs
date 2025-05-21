using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }
    public bool isPC;
    public GameObject gameOverPanel;
    public GameObject gameOverPanellose;
    public GameObject gameOverPanelWin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gameOverPanellose.SetActive(false);
        gameOverPanelWin.SetActive(false);
    }

    private void OnEnable()
    {
        HealthSystem.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        HealthSystem.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath(ulong clientId)
    {
        if (IsServer)
        {
            PlayerLoseClientRpc(clientId);
        }
    }

    [ClientRpc]
    private void PlayerLoseClientRpc(ulong losingClientId)
    {
        gameOverPanel.SetActive(true);

        if (NetworkManager.Singleton.LocalClientId == losingClientId)
        {
            gameOverPanellose.SetActive(true);
        }
        else
        {
            gameOverPanelWin.SetActive(true);
        }
        Invoke("ReturnToMenu", 2f);

        // หยุดเกมหรือปิด UI อื่น ๆ เพิ่มเติมได้ที่นี่
    }

    void ReturnToMenu()
    {
        Destroy(mainMenuSceneController.current);
        SceneManager.LoadScene("Main_Menu");
        mainMenuSceneController.current.LeaveSession();
    }
}
