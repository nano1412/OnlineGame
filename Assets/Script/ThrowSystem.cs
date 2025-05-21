using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class ThrowSystem : NetworkBehaviour
{
    [SerializeField] public GameController gameController;
    private GameObject canvas;

    [SerializeField] GameObject hand;
    [SerializeField] GameObject BombReleasePoint;
    [SerializeField] float throwPointDistance;

    SpriteRenderer handSpriteRenderer;
    Playermovement playermovement;
    private Vector3 defaultHandPostion;
    private Vector3 mouse_pos;
    private Vector3 object_pos;
    private float angle;



    //for mobile control
    public InputActionReference aim;
    private Vector2 aimInput;
    Vector2 lastDirection = Vector2.up;

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
        canvas = GameObject.Find("Canvas");

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

        aimInput = aim.action.ReadValue<Vector2>();

        if (gameController.isPC)
        {
            UpdateHandPC();
            UpdateThrowPosition(mouse_pos - transform.position);
            ChargeUpPC();
        } else
        {
            UpdateHandMobile();
            UpdateThrowPosition(aimInput);
            ChargeUpMobile();
        }
    }

    void UpdateHandPC()
    {
        if (!IsOwner) return;


        mouse_pos = Input.mousePosition;
        mouse_pos.z = -20;
        object_pos = Camera.main.WorldToScreenPoint(hand.transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;

        UpdateHand();
    }

    void UpdateHandMobile()
    {
        if (!IsOwner) return;
        angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
        UpdateHand();
    }

    private void UpdateHand()
    {
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

    void ChargeUpPC()
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

                Vector3 direction = BombReleasePoint.transform.position - transform.position;
                ThrowBombServerRpc(direction, currentCharge * powerMultiplier, BombReleasePoint.transform.position);
                isCharging = false;
        }
    }

    void ChargeUpMobile()
    {
        cooldowncount -= Time.deltaTime;

        

        if(aimInput != Vector2.zero)
        {
            lastDirection = aimInput.normalized;
        }

        if (aimInput.magnitude > 0.5f && cooldowncount < 0)
        {
            isCharging = true;
            currentCharge = minThrowForce;
        }

        if (aimInput.magnitude > 0.5f && isCharging)
        {
            currentCharge += chargeSpeed * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, minThrowForce, maxThrowForce);
        }

        if (aimInput == Vector2.zero && cooldowncount < 0 && isCharging)
        {
            cooldowncount = cooldown;

            Vector3 direction = lastDirection;
            ThrowBombServerRpc(direction, currentCharge * powerMultiplier, BombReleasePoint.transform.position);
            isCharging = false;
        }
    }

    void UpdateThrowPosition(Vector3 direction)
    {
        if (direction.magnitude > throwPointDistance)
        {
            direction = direction.normalized * throwPointDistance;
        }
        BombReleasePoint.transform.position = transform.position + direction;
    }

    [ServerRpc]
    void ThrowBombServerRpc(Vector3 direction, float force, Vector3 throwPosition)
    {
        GameObject newbomb = Instantiate(bomb, throwPosition, Quaternion.identity);
        var netObj = newbomb.GetComponent<NetworkObject>();
        netObj.Spawn();

        newbomb.GetComponent<Boom>().Initialize(new Vector2(direction.x, direction.y), force);
    }

}
