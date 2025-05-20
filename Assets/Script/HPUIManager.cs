using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections.Generic;

public class HPUIManager : MonoBehaviour
{
    public TextMeshProUGUI player1HPText;
    public TextMeshProUGUI player2HPText;

    private List<HealthSystem> playerHealths = new List<HealthSystem>();

    void Update()
    {
        if (playerHealths.Count < 2)
        {
            // หา Player ทุกตัวในฉากที่มี HealthSystem
            playerHealths.Clear();
            HealthSystem[] all = FindObjectsOfType<HealthSystem>();
            foreach (var h in all)
            {
                playerHealths.Add(h);
            }
        }

        if (playerHealths.Count >= 2)
        {
            var p1 = playerHealths[0];
            var p2 = playerHealths[1];

            player1HPText.text = $"Player 1 HP: {p1.currentHP.Value}";
            player2HPText.text = $"Player 2 HP: {p2.currentHP.Value}";
        }
    }
}
