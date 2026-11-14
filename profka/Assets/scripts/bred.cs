using UnityEngine;


public class finale : MonoBehaviour
{
    [Header("ну это бред")]
    public GameObject bred;

    private bool fin = false;

    void Start()
    {
      
        if (bred != null)
        {
            bred.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (!fin  && collision.CompareTag("Player"))
        {
            fin = true;

            if (bred != null)
            {
                bred.SetActive(true);
            }

            Time.timeScale = 0.2f;
        }
    }
}