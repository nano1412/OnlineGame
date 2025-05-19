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

    // Event สำหรับอัปเดต UI HP
    public event Action<int, int> OnHealthChanged; // currentHP, maxHP

    // Property ให้ UI หรือระบบอื่นเข้าถึงค่าปัจจุบันได้
    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    // Event แจ้งเมื่อ HP หมด
    public static event Action<ulong> OnPlayerDeath;

    void Start()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();

        // เรียก event ตอนเริ่มเพื่อให้ UI แสดง HP ทันที
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    // ฟังก์ชันเรียกเมื่อโดนระเบิด
    public void TakeDamage(int amount)
    {
        if (!IsOwner) return; // ให้เจ้าของเท่านั้นจัดการ HP ตัวเอง

        currentHP -= amount;
        Debug.Log("Player " + OwnerClientId + " โดนระเบิด! เหลือ HP = " + currentHP);

        // แจ้ง UI เปลี่ยน HP
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (currentHP <= 0)
        {
            Debug.Log("Player " + OwnerClientId + " แพ้แล้ว!");

            // แจ้งว่า Player ตาย (เฉพาะ Server)
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
