using UnityEngine;
using System.Collections;

public class health : MonoBehaviour
{
    public int maxhealth = 100;
    private int curhealth;

    private Vector3 spawnPosition;
    private movement moveScript;
    private Rigidbody2D rb;
    private bool dead = false;

    void Start()
    {
        curhealth = maxhealth;
        spawnPosition = transform.position; // точка респавна
        moveScript = GetComponent<movement>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void takedamage(int amount)
    {
        if (dead) return;

        curhealth -= amount;

        if (curhealth <= 0)
        {
            StartCoroutine(DieAndRespawn());
        }
    }

    private IEnumerator DieAndRespawn()
    {
        dead = true;

        // труп не двигается сам по себе
        if (moveScript != null) moveScript.enabled = false;

        // смэрть падение
        rb.AddForce(Vector2.up * 6f, ForceMode2D.Impulse);
        rb.AddTorque(25f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(2f); // респавн через 2 секунды

        // респавно
        transform.position = spawnPosition;
        transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        curhealth = maxhealth;
        if (moveScript != null) moveScript.enabled = true;
        dead = false;
    }
}