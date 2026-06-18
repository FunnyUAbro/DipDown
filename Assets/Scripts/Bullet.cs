using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 target;
    [SerializeField] float speed;

    void Start()
    {
        
    }


    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) <= 0.01f)
        {
            Destroy(gameObject, 0.3f);
        }
    }
}
