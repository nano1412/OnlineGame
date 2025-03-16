using Unity.Netcode;
using UnityEngine;

public class ThrowSystem : NetworkBehaviour
{
    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject bomb;
    [SerializeField] GameObject arrow;
    public float angle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            Vector2 mousePosition = Input.mousePosition;
            //mousePosition.z = 10.0f;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector2 playerPosition = transform.position;

            angle = Vector2.SignedAngle(playerPosition, mousePosition);
        }
    }
}
