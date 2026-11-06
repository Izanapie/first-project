using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [Header("физика")]
    public Rigidbody2D rb;

    [Header("наклон")]
    public float maxtiltangle = 78f;
    public float tiltspeed = 160f;
    public float returnspeed = 2f;

    [Header("прыжок")]
    public float Force = 12f;
    public float mintiltangle = 15f;
    [Range(0f, 1f)]
    public float inercia = 0.55f;
    public float friction = 0.95f;
    public Vector2 complace = new Vector2(0f, -0.5f);

    [Header("проверка приземления")]
    public Transform groundCheck;
    public float checkRadius = 0.25f;
    public LayerMask groundLayer;

    private bool grounded;
    private float curangle = 0f;
    private int prevaimdirection = 0;    // 1 = влево, -1 = вправо
    private bool jumpt = false;
    private bool holding = false;

    void Start()
    {
        rb.centerOfMass = complace;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        bool holdLeft = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        bool holdRight = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;

        bool releasedLeft = Keyboard.current.aKey.wasReleasedThisFrame || Keyboard.current.leftArrowKey.wasReleasedThisFrame;
        bool releasedRight = Keyboard.current.dKey.wasReleasedThisFrame || Keyboard.current.rightArrowKey.wasReleasedThisFrame;


        if (holdLeft && !holdRight)
        {
            holding = true;
            prevaimdirection = 1;


            float speed = curangle < 0f ? tiltspeed * 2.2f : tiltspeed;
            curangle = Mathf.MoveTowards(curangle, maxtiltangle, speed * Time.deltaTime);
        }
        else if (holdRight && !holdLeft)
        {
            holding = true;
            prevaimdirection = -1;

            float speed = curangle > 0f ? tiltspeed * 2.2f : tiltspeed;
            curangle = Mathf.MoveTowards(curangle, -maxtiltangle, speed * Time.deltaTime);
        }
        else
        {
            holding = false;
            curangle = Mathf.Lerp(curangle, 0f, Time.deltaTime * returnspeed);
        }

        // Отпустили кнопку — прыжок
        if ((releasedLeft && prevaimdirection == 1) || (releasedRight && prevaimdirection == -1))
        {
            if (grounded)
            {
                jumpt = true;
            }
        }
    }

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        rb.MoveRotation(curangle);
        rb.angularVelocity = 0f;

        if (grounded && !jumpt && !holding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * friction, rb.linearVelocity.y);
        }

        if (jumpt)
        {
            Jump();
            jumpt = false;
        }
    }

    void Jump()
    {

        float jumpingangle = curangle;
        if (prevaimdirection == 1 && jumpingangle < mintiltangle)
        {
            jumpingangle = mintiltangle;
        }
        else if (prevaimdirection == -1 && jumpingangle > -mintiltangle)
        {
            jumpingangle = -mintiltangle;
        }
        Vector2 jumpDir = Quaternion.Euler(0f, 0f, jumpingangle) * Vector2.up;
        float curvelx = rb.linearVelocity.x;
        bool samedir = (jumpDir.x * curvelx) > 0f;
        float keptvelx = samedir ? curvelx * inercia : curvelx * 0.15f;

        rb.linearVelocity = new Vector2(keptvelx, 0f);
        rb.AddForce(jumpDir * Force, ForceMode2D.Impulse);
    }
}