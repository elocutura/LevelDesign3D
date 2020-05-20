using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkourController : MonoBehaviour
{

    public float jumpStrength;
    public float wallJumpCooldown;
    float wallJumpCooldownTimer = 0;

    public float idealGrabDistance;
    float grabSuccessPercentage;
    public Text grabSuccessText;

    public float sphereCastYOffset;
    public float sphereCastRadius;
    public float sphereCastMaxDistance;

    public float swipeDownHeight;
    public LayerMask collidersMask;

    public float climbImpulse;
    public float climbSpeed;
    public float climbCloseEnough;
    public bool climbing;
    Vector3 climbToSpot;
    Vector3 preservedInertia;

    Rigidbody rb;
    InertiaCC cc;

    [HideInInspector]
    public bool jumpRequest = false;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        cc = this.GetComponent<InertiaCC>();

        InertiaCC.groundedAgain.AddListener(OnGroundedAgain);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnPlayerDied()
    {
        // DO SOMETHING ON PLAYER DEATH
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit castHit;

        bool hitWallForward = Physics.SphereCast(this.transform.position + new Vector3(0, sphereCastYOffset, 0), sphereCastRadius, this.transform.forward, out castHit, sphereCastMaxDistance, collidersMask);

        RaycastHit placementHit = new RaycastHit();

        bool ledgeHit = false;

        if (hitWallForward)
            ledgeHit = Physics.Raycast(new Vector3(castHit.point.x, this.transform.position.y + swipeDownHeight, castHit.point.z), Vector3.down, out placementHit, swipeDownHeight - 0.1f, collidersMask);

        if (ledgeHit)
            grabSuccessPercentage = 100 - (Mathf.Abs(castHit.distance - idealGrabDistance) % 1) * 100;
        else
            grabSuccessPercentage = -1;

       // grabSuccessText.text = (grabSuccessPercentage).ToString("F2");

        if (Input.GetButtonDown("Jump"))
        {
            if (cc.wallRuning && wallJumpCooldownTimer <= 0)
            {
                jumpRequest = true;
                wallJumpCooldownTimer = wallJumpCooldown;
            }
            else if (hitWallForward)
            {
                if (ledgeHit)
                {
                    climbing = true;
                    climbToSpot = Vector3.up * climbCloseEnough + placementHit.point + this.transform.forward * 0.5f;
                    preservedInertia = rb.velocity * (1 + grabSuccessPercentage / 100);
                }
                else if (hitWallForward && !cc.isGrounded && wallJumpCooldownTimer <= 0)
                {
                    rb.velocity = Vector3.zero;
                    jumpRequest = true;
                    wallJumpCooldownTimer = wallJumpCooldown;
                }
                else if (cc.isGrounded)
                {
                    jumpRequest = true;
                }
            }
            else if (cc.isGrounded)
            {
                jumpRequest = true;
            }
        }

        if (wallJumpCooldownTimer >= 0)
            wallJumpCooldownTimer -= Time.deltaTime;

        // CSGO Style
        else if (Input.GetAxis("Jump") != 0 || Input.GetButton("Jump"))
        {
            if (cc.isGrounded)
            {
                jumpRequest = true;
            }
        }
        

        if (climbing)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, climbToSpot, climbSpeed);

            if (Vector3.Distance(this.transform.position, climbToSpot) < climbCloseEnough)
            {
                climbing = false;
                rb.velocity = preservedInertia;

                rb.AddForce(new Vector3(this.transform.forward.x * climbImpulse, climbImpulse, this.transform.forward.z * climbImpulse) * (grabSuccessPercentage), ForceMode.Impulse);
            }

        }
    }

    private void FixedUpdate()
    {
        if (jumpRequest)
            Jump();
    }

    void Jump()
    {
        if (climbing)
            return;

        if (cc.wallRuning)
        {
            cc.WallJumped();

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(new Vector3(0, jumpStrength, 0), ForceMode.Impulse);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(new Vector3(0, jumpStrength, 0), ForceMode.Impulse);
        }


        jumpRequest = false;
    }

    void OnGroundedAgain()
    {
        
    }
}
