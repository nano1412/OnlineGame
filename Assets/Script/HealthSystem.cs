using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;

public class HealthSystem : NetworkBehaviour
{
    [SerializeField] private int maxHP = 5;
    private int currentHP;

    private SpriteRenderer sr;
    private bool isFlashing = false;

    // Event ����Ѻ�ѻവ UI HP
    public event Action<int, int> OnHealthChanged; // currentHP, maxHP

    // Property ��� UI �����к������Ҷ֧��һѨ�غѹ��
    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    // Event ������� HP ���
    public static event Action<ulong> OnPlayerDeath;

    void Start()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();

        // ���¡ event �͹������������ UI �ʴ� HP �ѹ��
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    // �ѧ��ѹ���¡�����ⴹ���Դ
    public void TakeDamage(int amount)
    {
        if (!IsOwner) return; // �����Ңͧ��ҹ�鹨Ѵ��� HP ����ͧ

        currentHP -= amount;
        Debug.Log("Player " + OwnerClientId + " ⴹ���Դ! ����� HP = " + currentHP);

        // �� UI ����¹ HP
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (currentHP <= 0)
        {
            Debug.Log("Player " + OwnerClientId + " ������!");

            // ����� Player ��� (੾�� Server)
            if (IsServer)
            {
                OnPlayerDeath?.Invoke(OwnerClientId);
            }
        }
    }

    // �Ϳ࿡����ᴧ�����ⴹ����
    IEnumerator FlashRed()
    {
        isFlashing = true;
        sr.color = Color.red;
        yield return new WaitForSeconds(2f);
        sr.color = Color.white;
        isFlashing = false;
    }
}
