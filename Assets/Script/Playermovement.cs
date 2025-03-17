using UnityEngine;
using Unity.Netcode;

//public class Playermovement : MonoBehaviour
public class Playermovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public NetworkVariable<bool> isFlipped = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        // Apply the flip based on the network value
        spriteRenderer.flipX = current;
    }

    void Update()
    {
        if (!IsOwner) return;

        Move();
        Jump();
        
            
    }

    void Move()
    {
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
            isFlipped.Value = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
            isFlipped.Value = false;
        }

        //rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if ((Input.GetKeyDown(KeyCode.W) && isGrounded) || (Input.GetKeyDown(KeyCode.Space) && isGrounded) )
        {
            //rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    [ServerRpc]
    private void ToggleFlipServerRpc(bool newFlipState)
    {
        // Update the NetworkVariable on the server
        isFlipped.Value = newFlipState;
    }
}
