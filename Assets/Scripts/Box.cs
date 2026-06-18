using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteBox;
    [SerializeField] Collider2D colliderBox;
    [SerializeField] ParticleSystem PSBreak;
    [SerializeField] int hp;
    int fullHp;
    [SerializeField] Sprite spriteFullHp;
    [SerializeField] Sprite spriteHighHp;
    [SerializeField] Sprite spriteHalfHp;
    [SerializeField] Sprite spriteLowHp;

    [SerializeField] GameObject[] items;
    [SerializeField] float chanceOfNothing;

    //string[] boxItems = {"Nothing", "ammo", "medKit", "pistol", "assaultRifle", "shotgun" };



    void Start()
    {
        fullHp = hp;
    }

    void Update()
    {


        
       
    }

    public void TakeDamage(int damage)
    {
        if (hp <= 0) 
        {
            Die();
        }
        else
        {
            float percentHp;

            hp -= damage;

            percentHp = (float)hp / (float)fullHp;

            if (percentHp <= 0f)
            {
                Die();
            }
            else if (percentHp <= 0.25f)
            {
                spriteBox.sprite = spriteLowHp;
            }
            else if (percentHp <= 0.5f)
            {
                spriteBox.sprite = spriteHalfHp;
            }
            else if (percentHp <= 0.75f)
            {
                spriteBox.sprite = spriteHighHp;
            }
            
        }

    }

    void DropItem()
    {

        Instantiate(items[Random.Range(0, items.Length)], position: transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));



        //Debug.Log($"{boxItems[Random.Range(0,items.Length)]}");
    }

    void Die()
    {
        spriteBox.enabled = false;
        colliderBox.enabled = false;

        if (Random.Range(0f, 100f) > chanceOfNothing)
        {
            DropItem();
        }

        PSBreak.Play();

    }





    private void OnTriggerEnter2D(Collider2D collision)
    {

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

}
