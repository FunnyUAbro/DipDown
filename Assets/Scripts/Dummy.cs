using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Dummy : MonoBehaviour
{
    Color[] colors = { Color.red, Color.blue, Color.green, Color.cyan, Color.yellow, Color.black };
    [SerializeField] float speedMax;
    float realSpeed;
    [SerializeField] Vector2 startPos;
    [SerializeField] float distance;
    [SerializeField] bool vertical;

    float timer = 5f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D collider;
    public bool countTime = false;
    Color oldColor;




    void Start()
    {
        realSpeed = speedMax;

        startPos = transform.position;
    }

    void Update()
    {


        if (countTime == true)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                Respawn();

                timer = 5f;


            }
        }

        //if (transform.position.x <= 0.4f)
        //{
        //    Debug.Log("Position.x = 0.4");

        //    transform.position = new Vector2(transform.position.x + speed * Time.deltaTime , transform.position.y);
        //    Debug.Log("Srabotalo 1");

        //}
        //else if (transform.position.x >= 7.35f)
        //{
        //    Debug.Log("Position.x = 7.35");

        //    transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);

        //    Debug.Log("Srabotalo 2");

        //}








        if (vertical == false)
        {

            transform.position = new Vector2(transform.position.x + realSpeed * Time.deltaTime, transform.position.y);

            if (transform.position.x > startPos.x + distance)
            {
                realSpeed = speedMax * -1;
            }

            if (transform.position.x < startPos.x)
            {
                realSpeed = speedMax;

            }                   

        }
        else
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + realSpeed * Time.deltaTime);

            if (transform.position.y > startPos.y + distance)
            {
                realSpeed = speedMax * -1;
            }

            if (transform.position.y < startPos.y)
            {
                realSpeed = speedMax;

            }            

        }


    }

    public void TakeDamage()
    {
        //spriteRenderer.enabled = false;
        //collider.enabled = false;

        countTime = true;

        realSpeed = 0;

        ChangeColor();

    }

    void Respawn()
    {
        //spriteRenderer.enabled = true;
        //collider.enabled = true;

        countTime = false;

        realSpeed = speedMax;

        spriteRenderer.color = Color.white;      
    }

    void ChangeColor()
    {
        //Color newColor = colors[Random.Range(0, colors.Length)];

        spriteRenderer.color = Color.green;

        //if (newColor == oldColor)
        //{
        //    Debug.Log("Цвет повторился");

        //    ChangeColor();
        //}
        //else
        //{
        //    spriteRenderer.color = newColor;

        //    oldColor = newColor;

        //}


    }
}