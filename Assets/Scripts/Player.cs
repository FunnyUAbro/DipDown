using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum AnimationType
{
    None,
    Pistol,
    SMG,
    Assault,
    Shotgun,
    Sniper,
    Ammo
}

public class Player : MonoBehaviour
{
    [SerializeField] public GameObject[] inventory = new GameObject[6];

    bool canMove = true;
    bool canRotate = true;
    bool canAttack = true;
    bool canReload = true;
    bool canSwapWeapon = true;
    bool inBush;
    bool isMoving;
    bool rightHandClose;

    float bushTimer;
    [SerializeField] float bushMaxTime;

    [SerializeField] int maxHp;
    int currentHp;

    [SerializeField] float speed;
    [SerializeField][Range(0, 1)] float rotSpeed;
    [SerializeField] Gun gunInHand;
    [SerializeField] Rounds roundsInHand;
    [SerializeField] TMP_Text moneyCount;
    [SerializeField] TMP_Text bulletCount;
    [SerializeField] Transform mainHand;

    [SerializeField] Transform reSpawn;

    float dropDistance = 0.5f;

    int balance;

    Collider2D itemOnGround;



    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip deadSFX;
    [SerializeField] AudioClip walkingSFX;
    [SerializeField] AudioClip[] softSteps;
    [SerializeField] AudioClip[] loudSteps;

    [SerializeField] WeaponAnimation[] weaponAnimations;
    [SerializeField] Animator bottomAnimator;
    [SerializeField] Animator topAnimator;
    [SerializeField] float rightHandTimer;
    float rightHandTimerRemember;
    [SerializeField] Animator legsAnimator;


    //[SerializeField] AmmoCounter ammoCounter;

    void Start()
    {
        rightHandTimerRemember = rightHandTimer;
        currentHp = maxHp;
        AddMoney(0);
    }

    void Update()
    {
        if (rightHandClose)
        {
            rightHandTimer -= Time.deltaTime;

            if (rightHandTimer < 0)
            {
                rightHandClose = false;

                rightHandTimer = rightHandTimerRemember;

                //rightHandAnimator.Play("DefaultRightHandHoldGun");
                topAnimator.Play(weaponAnimations[(int)AnimationType.None].Hold.TopAnim);
            }
        }

        if (canRotate)
        {
            Rotate();
        }

        if (gunInHand != null)
        {
            UpdateAmmoText();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && gunInHand != null && canAttack)
        {
            gunInHand.PullTrigger();
        }
        else if (Input.GetKey(KeyCode.Mouse0) && gunInHand != null && canAttack)
        {
            gunInHand.HoldTrigger();
        }

        if (Input.GetKeyDown(KeyCode.R) && gunInHand != null && canReload)
        {
            TryReload();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && gunInHand != null)
        {
            //if (gunInHand.animationType == AnimationType.Shotgun)
            //{
            //    bodyAnimator.Play("ShotgunOpen");   
            //}
            //else if (gunInHand.animationType == AnimationType.Pistol)
            //{
            //    //Po pistoletnomy open the thing 

            //}
            //else
            //{
            //    rightHandAnimator.Play("RightHandOpenGun");
            //}

            bottomAnimator.Play(weaponAnimations[(int)gunInHand.animationType].BoltOpen.BottomAnim);
            topAnimator.Play(weaponAnimations[(int)gunInHand.animationType].BoltOpen.TopAnim);

            gunInHand.OpenBoltManual();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1) && gunInHand != null)
        {
            //if (gunInHand.animationType == AnimationType.Shotgun)
            //{
            //    bodyAnimator.Play("ShotgunClose");

            //}
            //else
            //{
            //    rightHandAnimator.Play("RightHandCloseGun");

            //    rightHandClose = true;
            //}

            bottomAnimator.Play(weaponAnimations[(int)gunInHand.animationType].BoltClose.BottomAnim);
            topAnimator.Play(weaponAnimations[(int)gunInHand.animationType].BoltClose.TopAnim);

            gunInHand.CloseBoltManual();
        }

        if (canMove == true)
        {
            transform.position = transform.position + new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * speed * Time.deltaTime;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                legsAnimator.SetBool("isWalking", true);

                isMoving = true;
            }
            else
            {
                legsAnimator.SetBool("isWalking", false);

                isMoving = false;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                DropItem();
            }

            if (Input.GetKeyDown(KeyCode.E) && itemOnGround != null)
            {
                TakeItem(itemOnGround);
            }
        }

        if (canSwapWeapon)
        {
            if ((Input.GetKeyDown(KeyCode.Alpha1)))
            {
                TrySwitch(0);
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha2)))
            {
                TrySwitch(1);
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha3)))
            {
                TrySwitch(2);
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha4)))
            {
                TrySwitch(3);
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha5)))
            {
                TrySwitch(4);
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha6)))
            {
                TrySwitch(5);

            }
        }
        
        if (inBush && isMoving)
        {
            PlaySound();
        }
    }

    void Rotate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mousePos - transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, dir);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotSpeed);
    }

    void PlaySound()
    {
        bushTimer -= Time.deltaTime;

        if (bushTimer < 0)
        {
            int random = Random.Range(0, softSteps.Length);
            
            audioSource.PlayOneShot(softSteps[random]);
            bushTimer = bushMaxTime;
        }
    }


    #region Item
    private void TakeItem(Collider2D colider)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                Debug.Log($"Я поднимаю: {colider.gameObject.name}");

                inventory[i] = colider.gameObject;

                colider.gameObject.SetActive(false);

                colider.transform.SetParent(transform);
                break;
            }
        }



    }
    void DropItem()
    {
        //Исчезнуть из инвентаря
        for (int i = 0; i < inventory.Length; i++)
        {
            if (gunInHand != null && inventory[i] == gunInHand.gameObject)   
            {
                inventory[i] = null;
                gunInHand.GetDropped();
                gunInHand.transform.position += transform.right * dropDistance;
                gunInHand = null;
                break;
            }
            else if (roundsInHand != null && inventory[i] == roundsInHand.gameObject)
            {
                inventory[i] = null;
                roundsInHand.transform.position += transform.right * dropDistance;
                roundsInHand.transform.SetParent(null);
                roundsInHand = null;
                break;
            }

            //Side Sprite true

            //Появиться на земле перед игроком



        }

        //bodyAnimator.Play("Idle");
        bottomAnimator.Play(weaponAnimations[(int)AnimationType.None].Hold.BottomAnim);
    }

    void TrySwitch(int slotIndex)
    {
        if (gunInHand)
        {
            //Спрятать старое (SetActive = false)
            gunInHand.gameObject.SetActive(false);
            gunInHand = null;
        }
        else if (roundsInHand)
        {
            roundsInHand.gameObject.SetActive(false);
            roundsInHand = null;
        }

        if (inventory[slotIndex])
        {
            //Взять новое в руки
            inventory[slotIndex].SetActive(true);

            if (inventory[slotIndex].tag == "Item")
            {
                gunInHand = inventory[slotIndex].GetComponent<Gun>();
                gunInHand.GetPickedUp();
                topAnimator.gameObject.SetActive(true);
                if (gunInHand.animationType == AnimationType.Pistol)
                {

                }
                else
                {
                    //rightHandAnimator.Play("DefaultRightHandHoldGun");
                    topAnimator.Play(weaponAnimations[(int)AnimationType.None].Hold.TopAnim);
                }
                gunInHand.transform.localPosition = mainHand.localPosition;
                gunInHand.transform.localRotation = mainHand.localRotation;

                HoldWeapon();
            }
            else if (inventory[slotIndex].tag == "Rounds")
            {
                roundsInHand = inventory[slotIndex].GetComponent<Rounds>();
                roundsInHand.transform.localPosition = mainHand.localPosition;
            }


            //Передывинуть в главную руку
            //И показать (SetActive = true)
        }
        else
        {
            //bodyAnimator.Play("Idle");
            bottomAnimator.Play(weaponAnimations[(int)AnimationType.None].Hold.BottomAnim);
            topAnimator.gameObject.SetActive(false);
        }

    }
    #endregion


    #region Weapon
    public void HoldWeapon()
    {
        bottomAnimator.Play(weaponAnimations[(int)gunInHand.animationType].Hold.BottomAnim);
        topAnimator.Play(weaponAnimations[(int)gunInHand.animationType].Hold.TopAnim);
    }
    bool CheckForRounds()
    {
        int missingAmmo = gunInHand.maxAmmo - gunInHand.ammo;
        if (gunInHand.noMag == true)
        {
            missingAmmo = 1;
        }

        int maxMissingAmmo = missingAmmo;


        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                continue;
            }
            if (inventory[i].tag == "Rounds")
            {
                Rounds pack = inventory[i].GetComponent<Rounds>();

                if (pack.type == gunInHand.ammoType)
                {
                    missingAmmo -= pack.TryGetAmmo(missingAmmo);
                }
            }
        }
        if (missingAmmo < maxMissingAmmo)
        {
            gunInHand.ReloadStart(maxMissingAmmo - missingAmmo);

            return true;
        }
        else
        {
            return false;
        }

    }
    void TryReload()
    {
        if (gunInHand.ammo < gunInHand.maxAmmo)
        {
            if (gunInHand.isReloading == false)
            {  
                if (CheckForRounds())
                {
                    //if (gunInHand.animationType == AnimationType.Pistol || gunInHand.animationType == AnimationType.SMG)
                    //{
                    //    bodyAnimator.Play("PistolReload");

                    //    bodyAnimator.speed = 1 / gunInHand.reloadTime;
                    //}
                    //else if (gunInHand.animationType == AnimationType.Assault || gunInHand.animationType == AnimationType.Sniper)
                    //{
                    //    bodyAnimator.Play("AssaultReload");

                    //}

                    bottomAnimator.Play(weaponAnimations[(int)gunInHand.animationType].Reload.BottomAnim);
                    topAnimator.Play(weaponAnimations[(int)gunInHand.animationType].Reload.TopAnim);

                    bottomAnimator.speed = 1 / gunInHand.reloadTime ;
                    //else
                    //    Debug.LogError("Такой перезарядки нема");

                }
            }
        }
    }
    public void EndReload()
    {
        HoldWeapon();
    }
    void UpdateAmmoText()
    {
        bulletCount.text = $"{gunInHand.ammo}/{gunInHand.maxAmmo}";
    }

    #endregion

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp < 0)
        {
            Die();

        }
    }


    void Die()
    {
        audioSource.PlayOneShot(deadSFX);

        canMove = false;
        canRotate = false;
        canAttack = false;
        canReload = false;
        canSwapWeapon = false;

        Debug.Log("Player is dead");

        Time.timeScale = 0.2f;

        Invoke("ReSpawn", 1f);
    }

    void ReSpawn()
    {
        canMove = true;
        canRotate = true;
        canAttack = true;
        canReload = true;
        canSwapWeapon = true;

        transform.position = reSpawn.position;
        Time.timeScale = 1f;
        currentHp = maxHp;
    }

    public void AddMoney(int cash)
    {
        balance += cash;

        moneyCount.text = $"$ {balance}";
    }




    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            itemOnGround = collision;
        }
        else if (collision.tag == "Rounds")
        {
            itemOnGround = collision;
        }

        if (collision.tag == "Bush")
        {
            inBush = true;
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            if (collision == itemOnGround)
            {
                itemOnGround = null;
            }
        }

        if (collision.tag == "Rounds")
        {
            if (collision == itemOnGround)
            {
                itemOnGround = null;
            }
        }
        
        if (collision.tag == "Bush")
        {
            inBush = false;
        }
    }
}
        