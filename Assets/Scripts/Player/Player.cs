using UnityEngine;
using System.Text;
using System.Collections;
using System.Linq;
public class Player : MonoBehaviour
{
    Rigidbody2D rb;


    public Vector2 position
    {
        get => rb != null ? rb.position : Vector2.zero;
        set {
            if(rb != null){
                rb.MovePosition(value);
            }
        }
    }

    
    public Vector2 MoveDir {
        get {
            if(rb != null) 
                return rb.linearVelocity.normalized;

            return Vector2.zero;
        }
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundMask == 0)
            groundMask = Physics.DefaultRaycastLayers & ~(1 << LayerMask.NameToLayer("Player"));
        if (wallMask == 0)
            wallMask = groundMask;

        if (pressureSensors == null && Application.platform == RuntimePlatform.Android)
            pressureSensors = gameObject.AddComponent<BluetoothSensorReader>();
    }



    [SerializeField] BluetoothSensorReader pressureSensors;

    [SerializeField]
    GameObject deathEffect;

    public bool isDead {get; private set;}

    public float midairMovementForce = 200.0f;
    public float midairCounterMovementForce = 800.0f;

    public void Die()
    {
        if (isDead)
            return;

        GameManager.Instance.StopTimer(false);

        isDead = true;

        deathEffect.transform.SetParent(null, true);
        deathEffect.SetActive(true);

        GameManager.Instance.GameOver();

        StartCoroutine(Shrink());
    }

    private IEnumerator Shrink(){
        enabled = false;

        rb.bodyType = RigidbodyType2D.Static;

        float shrinkTime = (float)Fractions.ThreeFifths;

        float t = 0.0f;
        while(t <= shrinkTime){
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / shrinkTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    public float speed = 3f;
    public float counterTorque = 5f;

    public float jumpForce = 6f;


    public float PInput{
        get => _input;
    }

    private float _input;

    [SerializeField]
    AudioSource rollingAudio;

    bool jumpPressed;

    bool jumped;

    [SerializeField] private int groundRayCount = 5;
    [SerializeField] private float radius = 0.5f;

    bool IsGrounded()
    {
        Vector2 origin = transform.position;

        // Cast rays in an arc across the bottom hemisphere
        for (int i = 0; i < groundRayCount; i++)
        {
            // -45° to +45° in radians
            float angle = Mathf.Lerp(-45f, 45f, i / (float)(groundRayCount - 1)) * Mathf.Deg2Rad;

            // Direction from center toward edge of lower hemisphere
            Vector2 dir = new (Mathf.Sin(angle), -Mathf.Cos(angle)); // Points down and slightly out
            Vector2 start = origin + dir * (radius - 0.01f); // Just inside collider edge

            if (Physics2D.Raycast(start, dir, groundRayLength, groundMask))
                return true;         

            // Debug visualization
            Debug.DrawRay(start, dir * groundRayLength, Color.green);
        }

        return false;
    }


    void Update()
    {
        _input = GetInput();
    }

    [SerializeField] private float coyoteTime = 0.2f; // How long after leaving ground a jump is still allowed
    [SerializeField] private float jumpCooldown = 0.2f; // How much time must pass between jumps

    private float coyoteTimer = 0f;
    private float lastJumpTime = float.NegativeInfinity;
        
    bool grounded;

    [SerializeField] private float wallCheckDistance = 0.05f;
    [SerializeField] private LayerMask wallMask;

    bool IsTouchingWall(int direction)
    {
        Vector2 origin = transform.position;
        Vector2 dir = new (direction, 0);
        float length = radius + wallCheckDistance;
        Debug.DrawRay(origin, dir * length);
        return Physics2D.Raycast(origin, dir, length, wallMask);
    }



    void FixedUpdate()
    {
        var signInput = Mathf.Sign(_input);

        bool touchingWallL = IsTouchingWall(1);
        bool touchingWallR = IsTouchingWall(-1);
        

        if (!Mathf.Approximately(_input, 0.0f))
        {
            int dir = -Mathf.RoundToInt(signInput);
            if ((dir == 1 && !touchingWallL) || (dir == -1 && !touchingWallR))
            {
                rb.AddTorque(_input * speed * Time.deltaTime);
                if (signInput != Mathf.Sign(rb.angularVelocity))
                    rb.AddTorque(-rb.angularVelocity * counterTorque * Time.deltaTime);
            }
        }
        else
        {
            rb.AddTorque(-rb.angularVelocity * counterTorque * Time.deltaTime);
        }

        // Ground check
        grounded = IsGrounded();

        float velocitySign = Mathf.Sign(rb.linearVelocityX);

        if (!grounded && ((velocitySign == 1 && !touchingWallR) || (velocitySign == -1 && !touchingWallL))) {
            //Debug.Log("Midair force applied");
            rb.AddForce(_input * (signInput != -velocitySign ? midairCounterMovementForce : midairMovementForce) * Time.deltaTime * Vector2.left);
        }

        // Jump input
        // Update coyote timer
        coyoteTimer = grounded ? coyoteTime : Mathf.Clamp(coyoteTimer - Time.deltaTime, 0.0f, float.MaxValue);


        // Jump logic with coyote time and cooldown
        if (jumpPressed && !jumped && coyoteTimer >= Mathf.Epsilon && (Time.time - lastJumpTime > jumpCooldown))
        {
            jumped = true;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastJumpTime = Time.time;
            coyoteTimer = 0f; // prevent double-jump during buffer
        }

        UpdateRollingSound(!grounded);
    }


    [SerializeField] LayerMask groundMask;

    public float maxVolume = 1.0f;
    public float maxPitch = 1.5f;
    public float minPitch = 0.8f;

    public float velocityDivider = 100.0f;

    public float groundRayLength = 0.05f;

    public float pressureSensitivity = 0.15f;
    public float pressureThreshold = 100.0f;
    void UpdateRollingSound(bool mute){

        if(Mathf.Approximately(rb.angularVelocity, 0.0f) || mute)
        {
            rollingAudio.mute = true;
            return;
        }

        float angularSpeed = Mathf.Abs(rb.angularVelocity);

        // Normalize angular speed to [0,1] range for sound control
        float normalizedSpeed = Mathf.Clamp01(angularSpeed / velocityDivider); // Tune denominator as needed

        rollingAudio.volume = normalizedSpeed * maxVolume;
        rollingAudio.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);

        rollingAudio.mute = false;
    }

    float resetCounter;

    public float jumpThreshold = 50.0f;

    float GetInput(){
        var _i = 0.0f;

        if(Input.GetKey(KeyCode.Escape)) {
            resetCounter += Time.deltaTime;
            if(resetCounter > 2.0f) {
                resetCounter = 0.0f;
                GameManager.Instance.ReloadCurrentLevel();
                return 0.0f;
            }
        }
        else
            resetCounter = 0.0f;

        bool jumpedSeat = false;
        if (pressureSensors != null) {
            jumpedSeat = pressureSensors.sensorMappedValues[4] < jumpThreshold;

            float leftSidePressure = pressureSensors.sensorInterpolatedValues[0] + pressureSensors.sensorInterpolatedValues[3] +  pressureSensors.sensorInterpolatedValues[6];
            leftSidePressure /= 3.0f;
            float rightSidePressure = pressureSensors.sensorInterpolatedValues[2] + pressureSensors.sensorInterpolatedValues[5] +  pressureSensors.sensorInterpolatedValues[8];
            rightSidePressure /= 3.0f;

            float pressureDifference = leftSidePressure - rightSidePressure;
            if (Mathf.Abs(pressureDifference) >= pressureThreshold)
                _i += (pressureDifference - (pressureThreshold * Mathf.Sign(pressureDifference))) * pressureSensitivity;
        }
            
        if (!jumpPressed) {
            jumpPressed = (Input.GetButton("Jump") || jumpedSeat) && grounded && !jumped;
        }
        else if (jumped && !Input.GetButton("Jump")) {
            jumpPressed = false;
            jumped = false;
        }

        for(int i = 0; i < Input.touchCount; ++i){
            var touch = Input.GetTouch(i);
            
            var pos = touch.rawPosition;

            if(!jumpPressed && touch.phase == TouchPhase.Began && pos.x > Screen.width * Fractions.ThreeFifths && pos.y > Screen.height * Fractions.ThreeFifths)
                jumpPressed = true;
            
            //Debug.Log(pos);
            _i += pos.x < Screen.width / 2 ? 1f : -1f;
        }

        _i += -Input.GetAxisRaw("Horizontal"); // get input (-1, 0, or 1)

        return Mathf.Clamp(_i, -1f, 1f);    
    }
}

