using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Door : MonoBehaviour
{
    [SerializeField] Transform objectDoor;
    [SerializeField] SpriteRenderer button;
    [SerializeField] bool isLocked;
    [SerializeField] GameObject key;
    Player playerInZone;
    bool isOpen;
    bool moving;
    [SerializeField] float speed;

    //[SerializeField] AudioSource audioSource;
    //[SerializeField] AudioClip openDoorSFX;

    void Start()
    {
        button.color = Color.yellow;
    }

    void Update()
    {
        if (playerInZone != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isLocked == true)
                {//Key check
                    KeyCheck();
                }
                else
                {
                    if (objectDoor.localPosition.y <= 0)
                    {
                        // Debug.Log("Opening, moving = true, isOpen = true");
                        moving = true;
                        isOpen = true;

                    }
                    else if (objectDoor.localPosition.y >= 2.48f)
                    {
                        // Debug.Log("Closing, moving = true, isOpnen = false");
                        moving = true;
                        isOpen = false;
                    }
                }

               // Debug.Log("playerInZone == true, KeyCode.E");



               //OpenDoor();
            }

        }

        if (moving == true && isOpen == true)
        {
            //Debug.Log("moving == true && isOpen == true");
            // Открывает дверь плавно

            OpenDoorByFrame();
             
        }
        else if (moving == true && isOpen == false)
        {
            //Debug.Log("moving == true && isOpen == false");

            CloseDoorByFrame();
            

        }



    }

    void KeyCheck()
    {
        if (key == null)
        {
            return;
        }

        for (int i = 0; i < playerInZone.inventory.Length; i++)
        {
            if (playerInZone.inventory[i] == key)
            {
                //play sound
                isLocked = false;
                Destroy(playerInZone.inventory[i]);
                return;
            }
        }
        //Can't open 'No Key'
        Debug.Log("You don't have the key");

    }

    void OpenDoorByFrame()
    {
        objectDoor.localPosition = new Vector2(objectDoor.localPosition.x, objectDoor.localPosition.y + speed * Time.deltaTime);

        //audioSource.PlayOneShot(openDoorSFX);

        if (objectDoor.localPosition.y >= 2.48f)
        {
            moving = false;
            isOpen = false;

            UpdateColor();
        }
    }

    void CloseDoorByFrame()
    {
        // Закрывает дверь

        objectDoor.localPosition = new Vector2(objectDoor.localPosition.x, objectDoor.localPosition.y - speed * Time.deltaTime);

        if (objectDoor.localPosition.y <= 0)
        {
            //Debug.Log("Y <= 0");
            moving = false;
            isOpen = true;

            UpdateColor();
        }
    }

    void PlayerEnteredZone(Collider2D collider)
    {
        

        //Функция реагирует на вход игрока в тригер зону

        playerInZone = collider.GetComponent<Player>();

        UpdateColor();
    }

    private void UpdateColor()
    {
        // Меняет цвет соответственно состояние (Открыта, закрыта дверь)

        if (isOpen == false)
        {
            button.color = Color.green;
        }
        else
        {
            button.color = Color.red;
        }
    }

    private void PlayerExitedZone()
    {
        //Функция реагирует на выход игрока из тригер зоны

        playerInZone = null;

        button.color = Color.yellow;
    }

    public void UnlockAndOpen()
    {
        isLocked = false;
        moving = true;
        isOpen = true;

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerEnteredZone(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerExitedZone();
    }


}
