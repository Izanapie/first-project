using UnityEngine;
using UnityEngine.InputSystem;

public class gunscript : MonoBehaviour
{
    [Header("привязки")]
    public Transform enemytarget;        // цель 
    public Transform firepoint;          // откуда летят пули
    public GameObject projectileprefab;  // префаб выстрела
    public Rigidbody2D playerrb;     // кто стреляет для отдачи
    public bool player1 = true;      // кто стреляет для игнора

    [Header("наводка")]
    public float aimrange = 8f;          // радиус
    public float aimtorque = 35f;        // сила притягивания руки 
    public float aimangleoffset = 90f;  //чисто для костыля, иначе римский салют

    [Header("характеристики оружия")]
    public int maxammo = 6;              // патроны в обойме
    public int projpershot = 1;       // снарядов за выстрел
    public float spreadangle = 3f;       // разброс
    public float firecooldown = 0.35f;   // задержка перед выстрелами
    public float recoil = 4f;       // отдача

    private int curammo;
    private float nextfiretime = 0f;
    private Rigidbody2D armrb;

    void Start()
    {
        curammo = maxammo;
        armrb = GetComponent<Rigidbody2D>(); // текущие патроны и прикол с рукой
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        bool shootPressed = player1
            ? (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
            : (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame); // стрельба

        if (shootPressed && Time.time >= nextfiretime && curammo > 0)
        {
            Shoot();
            nextfiretime = Time.time + firecooldown; // кд выстрела
        }
    }

    void FixedUpdate()
    {
        if (enemytarget == null || armrb == null) return;

        // проверка расстояния до врага и пермещении руки дальше
        float distance = Vector2.Distance(transform.position, enemytarget.position);

        if (distance <= aimrange)
        {
            Vector2 dir = enemytarget.position - transform.position;
            float targetangle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + aimangleoffset;

            float angleDiff = Mathf.DeltaAngle(armrb.rotation, targetangle);
            armrb.AddTorque(angleDiff * aimtorque * Time.fixedDeltaTime);
        }
    }

    void Shoot()
    {
        curammo--;

        for (int i = 0; i < projpershot; i++)
        {
            // рандомный разброс
            float spread = Random.Range(-spreadangle, spreadangle);
            Quaternion shotrotation = firepoint.rotation * Quaternion.Euler(0, 0, spread);

            GameObject proj = Instantiate(projectileprefab, firepoint.position, shotrotation);
            Projectile projectile = proj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.owner = playerrb.gameObject;
            }
        }

        // отдача
        if (playerrb != null)
        {
            playerrb.AddForce(-firepoint.right * recoil, ForceMode2D.Impulse);
        }
    }
}