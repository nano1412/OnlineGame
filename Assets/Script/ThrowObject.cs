using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public GameObject projectilePrefab; // พรีแฟ็บของวัตถุที่ต้องการปา
    public Transform throwPoint; // จุดที่ใช้ปล่อยของ (ติดไว้ที่ตัวละคร)
    public float forceMultiplier = 10f; // ค่าความแรงของการปา

    private Vector2 startPoint; // จุดเริ่มต้นตอนลาก
    private Vector2 endPoint; // จุดปล่อย
    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // กดเมาส์เพื่อเริ่มลาก
        {
            startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0) && isDragging) // ปล่อยเมาส์เพื่อยิง
        {
            endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Throw();
            isDragging = false;
        }
    }

    void Throw()
    {
        Vector2 throwDirection = (startPoint - endPoint).normalized; // ทิศทางการยิง (ย้อนจากลาก)
        float throwPower = (startPoint - endPoint).magnitude * forceMultiplier; // คำนวณแรงยิง

        GameObject projectile = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity); // สร้างวัตถุ
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>(); // ดึง Rigidbody2D ของวัตถุ

        if (rb != null)
        {
            rb.AddForce(throwDirection * throwPower, ForceMode2D.Impulse); // ยิงออกไป
        }
    }
}
