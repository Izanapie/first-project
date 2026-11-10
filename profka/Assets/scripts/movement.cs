using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [Header("физика")]
    public Rigidbody2D rb; // сюда закинуть объект 

    [Header("наклон")]
    public float maxtiltangle = 78f; // вцелом понятно и по названиям
    public float tiltspeed = 160f;
    public float returnspeed = 2f;

    [Header("прыжок")]
    public float Force = 12f; // сила прыжка
    public float mintiltangle = 15f; // минимальный угол на который можно прыгнуть
    [Range(0f, 1f)]
    public float inertia = 0.55f; // кэф инерции
    public float friction = 0.95f; // кэф трения
    public Vector2 complace = new Vector2(0f, -1f); // complace - расположение центра массы, следовательно это меняет его положение

    [Header("проверка приземления")]
    public Transform groundCheck;
    public float checkRadius = 0.25f; // радиус круга который проверяет приземление
    public LayerMask groundLayer; // землёй считаются объекты слоя ground

    private bool grounded; // на земле или нет
    private float curangle = 0f; // тек. угол
    private int prevaimdirection = 0;    // предыдущее направление прыжка 1 = влево -1 = вправо 
    private bool jumpt = false; // прыгнул или нет
    private bool holding = false; // удержание кнопки или нет

    void Start()
    {
        rb.centerOfMass = complace; // подвязывание рб центра массы к тому центру массы
    }

    void Update() // update - каждый кадр fixedupdate - условно говоря физический кадр
    {
        if (Keyboard.current == null) return; // управление, надо будет чутка переделать когда сплитскрин надо будет делать, да и кнопки получше

        bool holdLeft = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        bool holdRight = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;

        bool releasedLeft = Keyboard.current.aKey.wasReleasedThisFrame || Keyboard.current.leftArrowKey.wasReleasedThisFrame;
        bool releasedRight = Keyboard.current.dKey.wasReleasedThisFrame || Keyboard.current.rightArrowKey.wasReleasedThisFrame;


        if (holdLeft && !holdRight) // если удерживается кнопка влево то
        {
            holding = true;
            prevaimdirection = 1;


            float speed = curangle < 0f ? tiltspeed * 2.2f : tiltspeed; // и отклонение
            curangle = Mathf.MoveTowards(curangle, maxtiltangle, speed * Time.deltaTime);
        }
        else if (holdRight && !holdLeft) // то же самое но для вправо
        {
            holding = true;
            prevaimdirection = -1;

            float speed = curangle > 0f ? tiltspeed * 2.2f : tiltspeed;
            curangle = Mathf.MoveTowards(curangle, -maxtiltangle, speed * Time.deltaTime);
        }
        else //иначе возвращение
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
        grounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer); // при приземлении

        rb.MoveRotation(curangle);
        rb.angularVelocity = 0f;

        if (grounded && !jumpt && !holding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * friction, rb.linearVelocity.y); // гасить скорость
        }

        if (jumpt)
        {
            Jump();
            jumpt = false;
        }
    }

    void Jump()
    {

        float jumpingangle = curangle; // угол прыжка будет равен текущему углу, ставим значение как mintiltangle для прыжка в зависимости от направления(похоже что переделаю)
        if (prevaimdirection == 1 && jumpingangle < mintiltangle)
        {
            jumpingangle = mintiltangle;
        }
        else if (prevaimdirection == -1 && jumpingangle > -mintiltangle)
        {
            jumpingangle = -mintiltangle;
        }
        Vector2 jumpDir = Quaternion.Euler(0f, 0f, jumpingangle) * Vector2.up; // непосредственно прыжок
        float curvelx = rb.linearVelocity.x;
        bool samedir = (jumpDir.x * curvelx) > 0f;
        float keptvelx = samedir ? curvelx * inertia : curvelx * 0.15f;

        rb.linearVelocity = new Vector2(keptvelx, 0f);
        rb.AddForce(jumpDir * Force, ForceMode2D.Impulse);
    }
}