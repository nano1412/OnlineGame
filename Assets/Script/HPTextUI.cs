using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPTextUI : MonoBehaviour
{
    [Header("อ้างอิง HealthSystem ของ Player")]
    public HealthSystem healthSystem;

    [Header("Text สำหรับแสดง HP")]
    public TextMeshProUGUI hpText; // ใช้ TMP
    // public Text hpText; // ถ้าใช้ Text ธรรมดาให้ใช้บรรทัดนี้แทน

    void Start()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged += UpdateHPText;
            UpdateHPText(healthSystem.CurrentHP, healthSystem.MaxHP);
        }
    }

    void OnDestroy()
    {
        if (healthSystem != null)
            healthSystem.OnHealthChanged -= UpdateHPText;
    }

    void UpdateHPText(int current, int max)
    {
        hpText.text = $"HP: {current} / {max}";
    }
}
