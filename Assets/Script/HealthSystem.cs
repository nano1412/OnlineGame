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

    // ฟังก์ชันเรียกเมื่อโดนระเบิด
    public void TakeDamage(int amount)
    {
        if (!IsOwner) return; // ให้เจ้าของเท่านั้นที่จัดการ HP ตัวเอง

        currentHP -= amount;
        Debug.Log("Player " + OwnerClientId + " โดนระเบิด! เหลือ HP = " + currentHP);

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (currentHP <= 0)
        {
            Debug.Log("Player " + OwnerClientId + " แพ้แล้ว!");

            // เรียก Server ให้บอกว่าแพ้
            if (gameController != null && IsServer)
            {
                gameController.PlayerLoseServerRpc(OwnerClientId);
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
