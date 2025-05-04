using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.Windows;
using UnityEngine.EventSystems;

//public class Playermovement : MonoBehaviour
public class Playermovement : NetworkBehaviour
{
    private float health = 5f;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public NetworkVariable<bool> isFlipped = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] GameObject playerControllerCanvasPrefab;
    private GameObject playerControllerCanvas;
    private ButtonHold leftbutton;
    private ButtonHold rightbutton;
    private ButtonHold downbutton;
    private ButtonHold jumpbutton;
    //public LayerMask groundLayer;

    public Rigidbody2D rb;
    private SpriteRenderer bodySpriteRenderer;
    private bool isGrounded;
    [SerializeField] private Vector3 throwPointStartPosition;
    private GameObject throwPoint;

    void Start()
    {
        playerControllerCanvas = Instantiate(playerControllerCanvasPrefab);
        leftbutton = playerControllerCanvas.transform.Find("movementButton").Find("leftbutton").GetComponent<ButtonHold>();
        rightbutton = playerControllerCanvas.transform.Find("movementButton").Find("rightButton").GetComponent<ButtonHold>();
        downbutton = playerControllerCanvas.transform.Find("movementButton").Find("dropButton").GetComponent<ButtonHold>();
        jumpbutton = playerControllerCanvas.transform.Find("movementButton").Find("jumpButton").GetComponent<ButtonHold>();


    throwPoint = transform.Find("hand").gameObject.transform.Find("bombRelease").gameObject;
        throwPointStartPosition = throwPoint.transform.position;
        rb = GetComponent<Rigidbody2D>();
        bodySpriteRenderer = transform.Find("body").GetComponent<SpriteRenderer>();
        isFlipped.OnValueChanged += HandleFlipChange;

        if (!IsOwner) return;
            // ��˹����˹��������
            if (NetworkManager.Singleton.IsHost)
            {
                transform.position = new Vector3(-5f, 1f, 0f);  // Host �����������
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                transform.position = new Vector3(5f, 1f, 0f);  // Client ����������
            }
    }

    private void HandleFlipChange(bool previous, bool current)
    {

        bodySpriteRenderer.flipX = current;

    }

    void Update()
    {
        if (!IsOwner) return;

        if (UnityEngine.Input.GetKey(KeyCode.A) || leftbutton.isHolding)
        {
            MoveLeft();
        }

        if (UnityEngine.Input.GetKey(KeyCode.S) || downbutton.isHolding)
        {
            Drop();
        }

        if (UnityEngine.Input.GetKey(KeyCode.D) || rightbutton.isHolding)
        {
            MoveRight();
        }

        if ((UnityEngine.Input.GetKeyDown(KeyCode.W) || UnityEngine.Input.GetKeyDown(KeyCode.Space) || jumpbutton.isHolding )&& isGrounded)
        {
            Jump();
        }
            
    }

    public void MoveLeft()
    {
        isFlipped.Value = true;   
        rb.linearVelocity = new Vector2(-1f * moveSpeed, rb.linearVelocity.y);
    }

    public void MoveRight()
    {
        isFlipped.Value = false;
        rb.linearVelocity = new Vector2(1f * moveSpeed, rb.linearVelocity.y);
    }

    public void Drop()
    {

    }

    public void Jump()
    {
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        
            //rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleFlipServerRpc(bool newFlipState)
    {
        // Update the NetworkVariable on the server
        isFlipped.Value = newFlipState;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        health -= damage;
        Debug.Log($"Player {OwnerClientId} took {damage} damage! Current HP: {health}");
        TakeDamageClientRpc(damage);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(float damage)
    {
        Debug.Log($"Player {OwnerClientId} HP updated: {health}");
    }
}

