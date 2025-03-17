using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ThrowSystem : NetworkBehaviour
{
    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject bomb;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject hand;

    private Vector3 mouse_pos;
    private Vector3 object_pos;
    private float angle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hand = transform.Find("hand").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            mouse_pos = Input.mousePosition;
            mouse_pos.z = -20;
            object_pos = Camera.main.WorldToScreenPoint(hand.transform.position);
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
            angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
            hand.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
