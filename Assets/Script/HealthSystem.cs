using UnityEngine;
using Unity.Netcode;
using System.Collections;
using TMPro;

public class HealthSystem : NetworkBehaviour
{
    [SerializeField] private int maxHP = 5;

    // ใช้ NetworkVariable เพื่อให้ทุกคนเห็นค่า HP ของผู้เล่นนี้
    public NetworkVariable<int> currentHP = new NetworkVariable<int>();

    private SpriteRenderer sr;
    private bool isFlashing = false;

    public static event System.Action<ulong> OnPlayerDeath;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            currentHP.Value = maxHP;
        }

        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        if (!IsOwner) return;

        currentHP.Value -= amount;
        Debug.Log($"Player {OwnerClientId} โดนระเบิด! เหลือ HP = {currentHP.Value}");

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (currentHP.Value <= 0)
        {
            Debug.Log($"Player {OwnerClientId} แพ้แล้ว!");
            if (IsServer)
            {
                OnPlayerDeath?.Invoke(OwnerClientId);
            }
        }
    }

    IEnumerator FlashRed()
    {
        isFlashing = true;
        sr.color = Color.red;
        yield return new WaitForSeconds(2f);
        sr.color = Color.white;
        isFlashing = false;
    }
}
