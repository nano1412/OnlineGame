using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ThrowSystem : NetworkBehaviour
{
    [SerializeField] GameObject hand;

    SpriteRenderer handSpriteRenderer;
    Playermovement playermovement;
    private Vector3 defaultHandPostion;
    private Vector3 mouse_pos;
    private Vector3 object_pos;
    private float angle;

    [SerializeField] GameObject bomb;
    [SerializeField] GameObject arrow;
    [SerializeField] float chargeSpeed = 3;
    [SerializeField] float power;
    [SerializeField] float powerMultiplier;
    [SerializeField] private float maxPower = 10;
    [SerializeField] float cooldown = 2;
    [SerializeField] float cooldowncount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultHandPostion = hand.transform.localPosition;
        hand = transform.Find("hand").gameObject;
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
        UpdateHand();
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

        if (Input.GetMouseButton(0) && cooldowncount < 0 && power < maxPower)
        {
            power += Time.deltaTime * chargeSpeed;
        }

        if (Input.GetMouseButtonUp(0) && cooldowncount < 0)
        {
            cooldowncount = cooldown;
            ThrowBomb();
        }
    }

    void ThrowBomb()
    {
        print(mouse_pos.x);
        print(mouse_pos.y);
        GameObject newBomb = Instantiate(bomb, hand.transform.Find("bombRelease").position, Quaternion.identity,transform);
        // add force depend on angle of hand and power

        newBomb.GetComponent<Rigidbody2D>().AddForce(new Vector2(mouse_pos.x, mouse_pos.y) * power * powerMultiplier,ForceMode2D.Impulse);

        power = 1;
    }
}
