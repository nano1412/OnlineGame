using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPTextUI : MonoBehaviour
{
    [Header("��ҧ�ԧ HealthSystem �ͧ Player")]
    public HealthSystem healthSystem;

    [Header("Text ����Ѻ�ʴ� HP")]
    public TextMeshProUGUI hpText; // �� TMP
    // public Text hpText; // ����� Text ������������÷Ѵ���᷹

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
