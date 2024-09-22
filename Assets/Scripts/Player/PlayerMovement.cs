using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float gravity = 7;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime; //How much time the player can hang in the air before jumping
    private float coyoteCounter; //How much time passed since the player ran off the edge

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX; //Horizontal wall jump force
    [SerializeField] private float wallJumpY; //Vertical wall jump force
    [SerializeField] private float wallSlideSpeed;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    //Internal variables
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    private bool grindingOnWall;
    private bool drop;

    private void Awake()
    {
        //Grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (!drop && !isGrounded())
        { 
            drop = Input.GetKeyDown(KeyCode.DownArrow);
        }

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        bool jumpInputDown = Input.GetKeyDown(KeyCode.Space)   ||
                             Input.GetKeyDown(KeyCode.UpArrow) ||
                             Input.GetKeyDown(KeyCode.W);
        bool jumpInputUp = Input.GetKeyUp(KeyCode.Space)   || 
                           Input.GetKeyUp(KeyCode.UpArrow) || 
                           Input.GetKeyUp(KeyCode.W);

        if (onWall() && !isGrounded() && (!drop || jumpInputDown))
        {
            drop = false;
            body.gravityScale = wallSlideSpeed;
            if (!grindingOnWall)
            {
                body.velocity = Vector2.zero;
                grindingOnWall = true;
            }
            else
            {
                body.velocity = new Vector2(0f, Mathf.Clamp(body.velocity.y, -10, 0));
            }
        }
        else if (!onWall() || (onWall() && isGrounded()) || (onWall() && drop))
        {
            body.gravityScale = gravity;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            grindingOnWall = false;

            //Flip player when moving left-right
            if (horizontalInput > 0.01f)
            {
                transform.localScale = Vector3.one;
            }
            else if (horizontalInput < -0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        //Jump
        if(jumpInputDown)
        { 
            Jump();
        }
        //Adjustable jump height
        if (body.velocity.y > 0 && jumpInputUp)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);
        }


        if (isGrounded() || onWall())
        {
            coyoteCounter = coyoteTime; //Reset coyote counter when on the ground
            jumpCounter = extraJumps; //Reset jump counter to extra jump value
        }
        else
        {
            coyoteCounter -= Time.deltaTime; //Start decreasing coyote counter when not on the ground 
        }
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0) return; 
        //If coyote counter is 0 or less and not on the wall and don't have any extra jumps don't do anything

        SoundManager.instance.PlaySound(jumpSound);

        if (onWall())
        { 
            WallJump();
        }
        else
        {
            if (isGrounded())
            { 
                body.velocity = new Vector2(body.velocity.x, jumpPower);
            }
            else
            {
                //If not on the ground and coyote counter bigger than 0 do a normal jump
                if (coyoteCounter > 0)
                { 
                    body.velocity = new Vector2(body.velocity.x, jumpPower);
                }
                else
                {
                    if (jumpCounter > 0) //If we have extra jumps then jump and decrease the jump counter
                    {
                        body.velocity = new Vector2(body.velocity.x, jumpPower);
                        jumpCounter--;
                    }
                }
            }
            //Reset coyote counter to 0 to avoid double jumps
            coyoteCounter = 0;
        }
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        wallJumpCooldown = 0;
    }


    private bool isGrounded()
    {
        RaycastHit2D raycastHitGround = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 
                                                    0, Vector2.down, 0.1f, groundLayer);

        return raycastHitGround.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHitHorizontal = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 
                                                    0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHitHorizontal.collider != null;
    }
    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}