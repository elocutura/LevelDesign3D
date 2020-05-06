using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InertiaCC : MonoBehaviour
{
    public static UnityEvent groundedAgain = new UnityEvent();

    public float speed;
    public float maxSpeed;
    public float extraGravity;

    public float horizontalSens;
    public float verticalSens;

    public float airTurningAccel;
    public float maxAirAccel;
    public float maxAirSpeed;
    float airTurn;

    public float maxPitch;
    public float minPitch;

    public LayerMask groundedLayers;
    public bool isGrounded;
    public float groundCheckRadius;
    public float maxSpeedDampRatio;

    public float minimumSpeedToWallRide;
    public float wallRunningAngleThreshold;
    public float wallCatchJumpCooldown;
    public float wallRunCheckRadius;
    [HideInInspector]
    public bool wallRuning;
    [HideInInspector]
    public Vector3 wallNormal;
    float wallCatchTimer;

    Rigidbody rb;
    Camera mainCam;
    ParkourController pk;

    float yaw = 0;
    float pitch = 0;

    float inputX;
    float inputY;

    public Text speedText;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        mainCam = Camera.main;
        pk = this.GetComponent<ParkourController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("Horizontal") * speed;
        inputY = Input.GetAxis("Vertical") * speed;

        RaycastHit isGroundedHit;

        bool newGrounded = Physics.SphereCast(new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z), groundCheckRadius, -this.transform.up, out isGroundedHit, groundedLayers);

        // Landed
        if (!isGrounded && newGrounded)
        {
            groundedAgain.Invoke();
        }
        // Take off
        else if (isGrounded && !newGrounded)
        {
            airTurn = 0;
        }

        isGrounded = newGrounded;

        yaw += Input.GetAxis("Mouse X") * horizontalSens * 10 * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * verticalSens * 10 * Time.deltaTime;

        this.transform.eulerAngles = new Vector3(0, yaw, 0);

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        mainCam.transform.eulerAngles = new Vector3(pitch, mainCam.transform.eulerAngles.y, 0);

        if (speedText)
            speedText.text = new Vector2(rb.velocity.x, rb.velocity.z).magnitude.ToString("F2");
    }

    private void FixedUpdate()
    {
        Vector3 forward = this.transform.forward * inputY;
        Vector3 right = this.transform.right * inputX;

        rb.AddForce(new Vector3(0, -extraGravity, 0));

        wallNormal = CheckForWalls();

        // Wallriding
        if (wallCatchTimer <= 0)
        {
            if (!isGrounded && !wallRuning && new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude >= minimumSpeedToWallRide)
            {
                if (wallNormal != Vector3.zero && Vector3.Angle(wallNormal, this.transform.forward) < wallRunningAngleThreshold)
                {
                    wallRuning = true;
                }
            }
        }
        else
        {
            wallCatchTimer -= Time.deltaTime;
        }

        if (wallRuning)
        {
            if (Vector3.Angle(wallNormal, this.transform.forward) > wallRunningAngleThreshold || wallNormal == Vector3.zero || new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude < minimumSpeedToWallRide)
                wallRuning = false;
            else
            {
                float clampedSpeed = Mathf.Clamp(new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude, maxSpeed, maxAirSpeed);
                rb.velocity = wallNormal * clampedSpeed;

                return;
            }
        }

        // Ground movement
        if (isGrounded && !pk.climbing && !Input.GetButton("Jump"))
        {
            rb.velocity = forward + right;
            if (rb.velocity.magnitude > maxSpeed)
            {
                Vector3 desiredVelocity = new Vector3(rb.velocity.normalized.x * maxSpeed, rb.velocity.y, rb.velocity.normalized.z * maxSpeed);
                rb.velocity = Vector3.Lerp(rb.velocity, desiredVelocity, maxSpeedDampRatio);
            }

            return;
        }
        // Air velocity and movement
        if (!isGrounded && !pk.climbing)
        {
            if (Input.GetAxis("Mouse X") == 0)
                airTurn = 0;

            airTurn += Input.GetAxis("Mouse X") * airTurningAccel / 100 * Time.deltaTime;
            airTurn = Mathf.Clamp(airTurn, -maxAirAccel / 100, maxAirAccel / 100);

            rb.velocity = (this.transform.forward * new Vector2(rb.velocity.x, rb.velocity.z).magnitude) + new Vector3(0, rb.velocity.y, 0) + (this.transform.forward * Mathf.Abs(airTurn));

            if (rb.velocity.magnitude > maxAirSpeed)
            {
                Vector3 desiredVelocity = new Vector3(rb.velocity.normalized.x * maxAirSpeed, rb.velocity.y, rb.velocity.normalized.z * maxAirSpeed);
                rb.velocity = Vector3.Lerp(rb.velocity, desiredVelocity, maxSpeedDampRatio);
            }
        }
    }

    Vector3 CheckForWalls()
    {
        RaycastHit rayHit;
        Vector3 wallNormal = Vector3.zero;

        if (Physics.SphereCast(new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z), wallRunCheckRadius, (this.transform.right + this.transform.forward*2).normalized, out rayHit, groundedLayers)
            || Physics.SphereCast(new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z), wallRunCheckRadius, this.transform.right, out rayHit, groundedLayers))
        {
            wallNormal = -Vector3.Cross(rayHit.normal, Vector3.up);
        }
        else if (Physics.SphereCast(new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z), wallRunCheckRadius, (-this.transform.right + this.transform.forward*2).normalized, out rayHit, groundedLayers)
            || Physics.SphereCast(new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z), wallRunCheckRadius, -this.transform.right + this.transform.forward, out rayHit, groundedLayers))
        {
            wallNormal = Vector3.Cross(rayHit.normal, Vector3.up);
        }

        return wallNormal;
    }

    public void WallJumped()
    {
        wallCatchTimer = wallCatchJumpCooldown;
        wallRuning = false;
    }
}
