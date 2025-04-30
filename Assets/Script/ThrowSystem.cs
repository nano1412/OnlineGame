using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ThrowSystem : NetworkBehaviour
{
    [SerializeField] GameObject hand;
    [SerializeField] GameObject BombReleasePoint;
    [SerializeField] float throwPointDistance;

    SpriteRenderer handSpriteRenderer;
    Playermovement playermovement;
    private Vector3 defaultHandPostion;
    private Vector3 mouse_pos;
    private Vector3 object_pos;
    private float angle;

    [SerializeField] GameObject bomb;
    [SerializeField] GameObject arrow;
    //[SerializeField] float power;
    //[SerializeField] private float maxPower = 10;
    [SerializeField] float powerMultiplier = 10f;
    [SerializeField] float cooldown = 2;
    [SerializeField] float cooldowncount;

    public float minThrowForce = 5f;
    public float maxThrowForce = 20f;
    public float chargeSpeed = 10f;

    private float currentCharge;
    private bool isCharging;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultHandPostion = hand.transform.localPosition;
        hand = transform.Find("hand").gameObject;
        BombReleasePoint = hand.transform.Find("bombRelease").gameObject;
        playermovement = transform.GetComponent<Playermovement>();
        handSpriteRenderer = hand.transform.GetComponent<SpriteRenderer>();

        playermovement.isFlipped.OnValueChanged += HandleFlipChange;

    }

    private void HandleFlipChange(bool previous, bool current)
    {
        // Update the sprite's flipX based on the NetworkVariable's value
        handSpriteRenderer.flipX = current;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        UpdateHand();
        UpdateThrowPosition();
        ChargeUp();

    }

    void UpdateHand()
    {
        if (!IsOwner) return;


        mouse_pos = Input.mousePosition;
        mouse_pos.z = -20;
        object_pos = Camera.main.WorldToScreenPoint(hand.transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;

        if (playermovement.isFlipped.Value)
        {
            hand.transform.rotation = Quaternion.Euler(0, 0, angle + 180);
            hand.transform.localPosition = new Vector3(-defaultHandPostion.x, defaultHandPostion.y, defaultHandPostion.z);
        }
        else
        {
            hand.transform.rotation = Quaternion.Euler(0, 0, angle);
            hand.transform.localPosition = defaultHandPostion;
        }
    }

    void ChargeUp()
    {
        cooldowncount -= Time.deltaTime;

        if (Input.GetMouseButton(0) && cooldowncount < 0)
        {
            isCharging = true;
            currentCharge = minThrowForce;
        }

        if (Input.GetMouseButton(0) && isCharging)
        {
            currentCharge += chargeSpeed * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, minThrowForce, maxThrowForce);
        }

        if (Input.GetMouseButtonUp(0) && cooldowncount < 0 && isCharging)
        {
            cooldowncount = cooldown;

                Vector2 direction = GetMouseDirection();
                ThrowBombServerRpc(direction, currentCharge * powerMultiplier, BombReleasePoint.transform.position);
                isCharging = false;
        }
    }

    Vector2 GetMouseDirection()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mouseWorld - BombReleasePoint.transform.position).normalized;
    }

    void UpdateThrowPosition()
    {
        
        Vector3 direction = mouse_pos - transform.position;
        if (direction.magnitude > throwPointDistance)
        {
            direction = direction.normalized * throwPointDistance;
        }
        BombReleasePoint.transform.position = transform.position + direction;
    }

    [ServerRpc]
    void ThrowBombServerRpc(Vector2 direction, float force, Vector3 throwPosition)
    {
        GameObject newbomb = Instantiate(bomb, throwPosition, Quaternion.identity);
        var netObj = newbomb.GetComponent<NetworkObject>();
        netObj.Spawn();

        newbomb.GetComponent<Boom>().Initialize(direction, force);
    }
}
