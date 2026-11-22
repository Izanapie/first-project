using UnityEngine;
using UnityEngine.InputSystem;

// прикол для инспектора чтобы накидывать удобно
[System.Serializable]
public class WeaponData
{
    public string weaponname = "pistol";    // название
    public Sprite gunsprite;                // спрайт
    public Vector2 firepointpos;            // сдвиг дула чтобы подогнать под спрайт
    public GameObject projectileprefab;     // префаб 

    public int maxammo = 6;                 // патроны в обойме
    public int projpershot = 1;             // снарядов за выстрел
    public float spreadangle = 3f;          // разброс
    public float firecooldown = 0.35f;      // задержка между выстрелами
    public float recoil = 4f;               // отдача
}

public class gunscript : MonoBehaviour
{
    [Header("привязки")]
    public Transform enemytarget;           // цель 
    public Transform firepoint;             // откуда летят пули
    public SpriteRenderer gunrenderer;      // SpriteRenderer на gunholder
    public Rigidbody2D playerrb;            // кто стреляет для отдачи
    public bool player1 = true;             // 1 или 2 игрок

    [Header("наводка")]
    public float aimrange = 8f;             // радиус
    public float aimtorque = 35f;           // сила притягивания руки 
    public float aimangleoffset = 90f;      // костыль для авитоаима, иначе римский салют

    [Header("состояние")]
    public bool hasweapon = false;          // есть ли сейчас пушка в руках

    // текущие статы по названию понятно что есть что
    private GameObject projectileprefab;
    private int curammo;
    private int projpershot;
    private float spreadangle;
    private float firecooldown;
    private float recoil;

    private float nextfiretime = 0f;
    private Rigidbody2D armrb;

    void Start()
    {
        armrb = GetComponent<Rigidbody2D>();

        // на старте руки пустые
        if (!hasweapon && gunrenderer != null)
        {
            gunrenderer.enabled = false;
        }
    }

    void Update()
    {
        // если пушки нет то выстрела нет
        if (!hasweapon || Keyboard.current == null) return;

        bool shootPressed = player1
            ? (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
            : (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame);

        if (shootPressed && Time.time >= nextfiretime && curammo > 0)
        {
            Shoot();
            nextfiretime = Time.time + firecooldown;
        }
    }

    void FixedUpdate()
    {
        // нет пушки  значи без автоаима
        if (!hasweapon || enemytarget == null || armrb == null) return;

        float distance = Vector2.Distance(transform.position, enemytarget.position);

        if (distance <= aimrange)
        {
            Vector2 dir = enemytarget.position - transform.position;
            float targetangle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + aimangleoffset;

            float angleDiff = Mathf.DeltaAngle(armrb.rotation, targetangle);
            armrb.AddTorque(angleDiff * aimtorque * Time.fixedDeltaTime);
        }
    }

    void Shoot() // что происходит во время выстрела
    {
        curammo--;

        for (int i = 0; i < projpershot; i++)
        {
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

        // кончились патроны значит без пушки
        if (curammo <= 0)
        {
            dropweapon();
        }
    }

    // подвязка под спавнеры
    public void equipweapon(WeaponData data)
    {
        projectileprefab = data.projectileprefab;
        curammo = data.maxammo;
        projpershot = data.projpershot;
        spreadangle = data.spreadangle;
        firecooldown = data.firecooldown;
        recoil = data.recoil;

        if (gunrenderer != null)
        {
            gunrenderer.sprite = data.gunsprite;
            gunrenderer.enabled = true;
        }

        // меняем позицию дула под спрайт если надо
        if (firepoint != null && data.firepointpos != Vector2.zero)
        {
            firepoint.localPosition = data.firepointpos;
        }

        hasweapon = true;
    }

    public void dropweapon()
    {
        hasweapon = false;
        curammo = 0;

        if (gunrenderer != null)
        {
            gunrenderer.enabled = false;
        }
    }
}