using Unity.Netcode;
using UnityEngine;
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
    private bool touchStart = false;
    private Vector2 pointA;
    private Vector2 pointB;
    private float middleOfScreen = Screen.width / 2;
    public GameObject rotationCirclePrefab;
    public GameObject rotationOuterCirclePrefab;
    public GameObject rotationCircle;
    public GameObject rotationOuterCircle;
    public float maxFollowRadius;

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

        rotationCircle = Instantiate(rotationCirclePrefab, canvas.transform);
        rotationOuterCircle = Instantiate(rotationOuterCirclePrefab, canvas.transform);

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

        if (gameController.isPC)
        {
            UpdateHandPC();
            UpdateHand();
            UpdateThrowPositionPC();
            ChargeUpPC();
        } else
        {
            UpdateHandMobile();
            UpdateHand();
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

        
    }

    void UpdateHandMobile()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > middleOfScreen)
        {

            pointA = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            rotationCircle.transform.position = pointA;
            rotationOuterCircle.transform.position = pointA;
            rotationCircle.SetActive(true);
            rotationOuterCircle.SetActive(true);

        }
        if (Input.GetMouseButton(0))
        {
            touchStart = true;
            pointB = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else
        {
            touchStart = false;
        }
    }

    private void FixedUpdate()
    {
        if (touchStart && !gameController.isPC && IsOwner)
        {
            if (Input.mousePosition.x > middleOfScreen)
            {
                Vector2 offset = pointB - pointA;
                Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                rotationCircle.transform.position = new Vector2(pointA.x + offset.x, pointA.y + offset.y);
            }
        }
        else
        {
            rotationCircle.SetActive(false);
            rotationOuterCircle.SetActive(false);
        }

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

    void UpdateThrowPositionPC()
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
