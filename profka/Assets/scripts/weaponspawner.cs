using UnityEngine;

public class weaponspawner : MonoBehaviour
{
    [Header("настройки спавна")]
    public WeaponData[] weapons;        // список всех возможных пушек
    public float respawntime = 5f;      // через сколько секунд появится новая

    private SpriteRenderer sr;
    private WeaponData curweapon;
    private bool ready = false;
    private float timer = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        spawnrandom();
    }

    void Update()
    {
        // если пушку уже забрали начинается таймер
        if (!ready)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                spawnrandom();
            }
        }
    }

    void spawnrandom()
    {
        if (weapons == null || weapons.Length == 0) return;

        // выбираем случайную пушку из списка
        int rand = Random.Range(0, weapons.Length);
        curweapon = weapons[rand];

        if (sr != null)
        {
            sr.sprite = curweapon.gunsprite;
            sr.enabled = true;
        }

        ready = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ready) return;

        // ищем скрипт пушки в детях или в родителях(прекрасно звучит)
        gunscript gun = collision.GetComponentInChildren<gunscript>();
        if (gun == null) gun = collision.GetComponentInParent<gunscript>();

        // если это игрок и руки у него пустые то пушка подбирается
        if (gun != null && !gun.hasweapon)
        {
            gun.equipweapon(curweapon);

            ready = false;
            if (sr != null) sr.enabled = false;
            timer = respawntime;
        }
    }
}