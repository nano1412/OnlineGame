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

    // Event แจ้งเมื่อผู้เล่นตาย
    public static event Action<ulong> OnPlayerDeath;

    void Start()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();
    }

    // ฟังก์ชันเรียกเมื่อโดนระเบิด
    public void TakeDamage(int amount)
    {
        if (!IsOwner) return; // ให้เจ้าของตัวละครจัดการ HP ของตัวเองเท่านั้น

        currentHP -= amount;
        Debug.Log("Player " + OwnerClientId + " โดนระเบิด! เหลือ HP = " + currentHP);

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (currentHP <= 0)
        {
            Debug.Log("Player " + OwnerClientId + " แพ้แล้ว!");

            // แจ้งเหตุการณ์แพ้ไปยังระบบอื่น (Server เท่านั้นที่แจ้ง)
            if (IsServer)
            {
                OnPlayerDeath?.Invoke(OwnerClientId);
            }
        }
    }

    // เอฟเฟกต์ตัวแดงเมื่อโดนโจมตี
    IEnumerator FlashRed()
    {
        isFlashing = true;
        sr.color = Color.red;
        yield return new WaitForSeconds(2f);
        sr.color = Color.white;
        isFlashing = false;
    }
}
