using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 18f;
    public int damage = 25;
    public float lifetime = 2.5f;
    public float knockback = 5f; // отбрасывание

    public GameObject owner; // кто выстрелил
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // снаряд летит справа
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifetime); // смэрть объекта через время
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // игнорируем хозяина и его производные
        if (collision.gameObject == owner || collision.transform.IsChildOf(owner.transform))
            return;

        // проверка попадания
        health thealth = collision.GetComponentInParent<health>();
        if (thealth != null)
        {
            thealth.takedamage(damage);

            // отбраываение при попадании
            Rigidbody2D trb = thealth.GetComponent<Rigidbody2D>();
            if (trb != null)
            {
                trb.AddForce(transform.right * knockback, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
        // если попал в землю
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}