using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class HealthSystem : NetworkBehaviour
{
    [SerializeField] private int maxHP = 5;
    private int currentHP;

    private SpriteRenderer sr;
    private bool isFlashing = false;

    [Header("Reference")]
    public GameController gameController;

    void Start()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();
    }

    // �ѧ��ѹ���¡�����ⴹ���Դ
    public void TakeDamage(int amount)
    {
        if (!IsOwner) return; // �����Ңͧ��ҹ�鹷��Ѵ��� HP ����ͧ

        currentHP -= amount;
        Debug.Log("Player " + OwnerClientId + " ⴹ���Դ! ����� HP = " + currentHP);

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (currentHP <= 0)
        {
            Debug.Log("Player " + OwnerClientId + " ������!");

            // ���¡ Server ���͡�����
            if (gameController != null && IsServer)
            {
                gameController.PlayerLoseServerRpc(OwnerClientId);
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
