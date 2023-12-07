using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Config
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float boostTimer = 0f;
    [SerializeField] float cooldownBoostTime = 20f;
    [SerializeField] Vector2 deathKick = new Vector2(0f, 5f);
    [SerializeField] AudioClip deathSFX;
    private float inputX;
    private float inputY;
    bool canDoubleJump;
    [Header("Check Ground")]
    public LayerMask whatIsGround;
    bool isGrounded;
    public Transform groundPoint;

    // State
    bool isAlive = true;
    bool isBoosted = false;

    // Cached component refrences
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    float gravityScaleAtStart;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            myRigidBody.velocity = new Vector3(0f, myRigidBody.velocity.y);
            return;
        }
        myRigidBody.velocity = new Vector2(inputX * moveSpeed, myRigidBody.velocity.y);
        isGrounded = Physics2D.OverlapCircle(groundPoint.position, .2f, whatIsGround);
        myAnimator.SetBool("Running", Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon);
        ClimbLadder();
        FlipSprite();
        Die();
    }
    public void Boost(InputAction.CallbackContext context)
    {
        if (!isAlive) { return; }
        if (context.performed)
            isBoosted = true;
    }
    public void BoostProcess()
    {
        if (isBoosted)
        {
            boostTimer += Time.deltaTime;
            myAnimator.SetBool("Boosting", true);
            if (boostTimer >= 5)
            {
                isBoosted = false;
                myAnimator.SetBool("Boosting", false);
                StartCoroutine(ZeroBoostTimer());
            }
        }
    }
    IEnumerator ZeroBoostTimer()
    {
        yield return new WaitForSeconds(cooldownBoostTime);
        boostTimer = 0;
    }

    public void Move(InputAction.CallbackContext context)
    {
        inputY = context.ReadValue<Vector2>().y;
        inputX = context.ReadValue<Vector2>().x;
    }

    private void ClimbLadder()
    {
        if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myAnimator.SetBool("Climbing", false);
            myRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = inputY;
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed)
        {
            myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpForce);
            canDoubleJump = true;
            myAnimator.SetTrigger("Jump");
        }
        else
        {
            if (canDoubleJump && context.performed)
            {
                myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpForce);
                canDoubleJump = false;
                myAnimator.SetTrigger("Jump");
            }
        }
    }

    private void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Death");
            AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position);
            GetComponent<Rigidbody2D>().velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
    }

}
