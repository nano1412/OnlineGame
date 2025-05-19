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

    // Event ������ͼ����蹵��
    public static event Action<ulong> OnPlayerDeath;

    void Start()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();
    }

    // �ѧ��ѹ���¡�����ⴹ���Դ
    public void TakeDamage(int amount)
    {
        if (!IsOwner) return; // �����Ңͧ����ФèѴ��� HP �ͧ����ͧ��ҹ��

        currentHP -= amount;
        Debug.Log("Player " + OwnerClientId + " ⴹ���Դ! ����� HP = " + currentHP);

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (currentHP <= 0)
        {
            Debug.Log("Player " + OwnerClientId + " ������!");

            // ���˵ء�ó�����ѧ�к���� (Server ��ҹ�鹷����)
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
