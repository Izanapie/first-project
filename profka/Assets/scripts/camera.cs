using UnityEngine;

public class camera : MonoBehaviour
{
    [Header("объект")]
    public Transform target;         

    [Header("еастройки")]
    public float speed = 5f;    
    public Vector3 offset = new Vector3(0f, 1.5f, -10f); 

    void LateUpdate()
    {
        if (target == null) return;


        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }
}