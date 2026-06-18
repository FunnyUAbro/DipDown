using UnityEngine;
using UnityEngine.Rendering;

enum BoltState
{
    Closed,
    Open,
    OpenManual,
    Ready,
    Used
}

enum BoltType
{
    Bolt,
    SemiAuto,
    Auto,
    SelfReload
}

public class Gun : MonoBehaviour
{
    float timerShootCooldown = 0;
    [SerializeField] BoltType boltType;
    BoltState boltState;
    [SerializeField] float shootCooldown;
    [SerializeField] float punchPower;
    public int ammo;
    public int maxAmmo;
    public int reloadAmmo;
    public float reloadTime;
    public bool isReloading;
    [SerializeField] int damage;
    public AmmoType ammoType;
    public AnimationType animationType;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Transform weaponPoint;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] ParticleSystem particleSystem2;
    [SerializeField] ParticleSystem casePS;
    [SerializeField] ParticleSystem magPS;

    [SerializeField] SpriteRenderer topSprite;
    [SerializeField] SpriteRenderer sideSprite;
    [SerializeField] Sprite openGun;
    [SerializeField] Sprite closedGun;
    [SerializeField] Sprite sideOpenGun;
    [SerializeField] Sprite sideClosedGun;

    [SerializeField] Bullet bulletPref;

    [SerializeField] AudioClip shootSFX;
    [SerializeField] AudioClip reloadStartSFX;
    [SerializeField] AudioClip reloadFinishSFX; 
    [SerializeField] AudioClip boltOpenSFX;
    [SerializeField] AudioClip boltCloseSFX;
    [SerializeField] AudioSource sourceSFX;

    void Start()
    {
        ammo = maxAmmo;
    }

    void Update()
    {
        timerShootCooldown -= Time.deltaTime;

        if (timerShootCooldown < 0 && boltState == BoltState.Open && boltType != BoltType.Bolt)
        {
            CloseBolt();
        }
    }

    public void PullTrigger()
    {
        TryShoot();
    }
    public void HoldTrigger()
    {
        if (boltType > BoltType.SemiAuto)
        {
            TryShoot();
        }
    }

    void TryShoot()
    {
        if (boltState == BoltState.Ready)
        {
            Shoot();

            if (boltType != BoltType.Bolt)
            {
                OpenBolt();
            }
        }
    }

    void Shoot()
    {
        float shotPunchPower = punchPower;

        sourceSFX.PlayOneShot(shootSFX);
        particleSystem2.Play();
        particleSystem.Play();

        if (ammoType == AmmoType.Shotgun)
        {
            float maxRot = 3.5f;
            int shotCount = 6;

            for (int i = 0; i < shotCount; i++)
            {
                weaponPoint.localRotation = Quaternion.Euler(0, 0, Random.Range(-maxRot, maxRot));

                RaycastHit2D hit = Physics2D.Raycast(weaponPoint.position, weaponPoint.right, 15, enemyLayer);

                HitManage(hit);

                Instantiate(bulletPref, weaponPoint.position, weaponPoint.rotation).GetComponent<Bullet>().target = hit.point;
            }

        }
        else
        {



            RaycastHit2D[] hits = Physics2D.RaycastAll(weaponPoint.position, weaponPoint.right, 30, enemyLayer);

            foreach (RaycastHit2D hit in hits)
            {
                HitManage(hit);

                if (shotPunchPower <= 0)
                {
                    Instantiate(bulletPref, weaponPoint.position, weaponPoint.rotation).GetComponent<Bullet>().target = hit.point;


                    break;
                }
            }

        }

        timerShootCooldown = shootCooldown;

        ammo -= 1;

        boltState = BoltState.Used;

        void HitManage(RaycastHit2D hit)
        {
            if (hit.collider == null)
            {
                Debug.DrawRay(weaponPoint.position, weaponPoint.right);

            }
            else
            {

                if (hit.collider.CompareTag("Undestructable"))
                {
                    Debug.DrawRay(weaponPoint.position, (Vector3)hit.point - weaponPoint.position, Color.red, 1);

                    shotPunchPower -= 3;

                }
                //else if (hit.collider.CompareTag("Dummy"))
                //{

                //    Debug.DrawRay(weaponPoint.position, (Vector3)hit.point - weaponPoint.position, Color.green, 1);

                //    hit.collider.gameObject.GetComponent<Dummy>().TakeDamage();



                //}
                else if (hit.collider.CompareTag("Box"))
                {
                    Debug.DrawRay(weaponPoint.position, (Vector3)hit.point - weaponPoint.position, Color.green, 1);

                    shotPunchPower -= 0.5f;

                    hit.collider.gameObject.GetComponent<Box>().TakeDamage(damage);
                }
                else if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.DrawRay(weaponPoint.position, (Vector3)hit.point - weaponPoint.position, Color.cyan, 1);

                    shotPunchPower -= 1;

                    hit.collider.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                }

            }
        }

        //ammoCounter.SetCounter(ammo);
    }

    public void GetPickedUp()
    {
        //Выкл Side Sprite 
        //Выкл Side Collider
        sideSprite.gameObject.SetActive(false);
        
        //Вкл Top Sprite
        topSprite.gameObject.SetActive(true);

    }

    public void GetDropped()
    {
        //Выкл Top Sprite
        topSprite.gameObject.SetActive(false);

        //Вкл Side Sprite
        //Вкл Side Collider
        sideSprite.gameObject.SetActive(true);
        transform.SetParent(null);
    }

    public void ReloadStart(int newAmmo)
    {
        magPS.Play();
        sourceSFX.PlayOneShot(reloadStartSFX);
         
        isReloading = true;

        timerShootCooldown = reloadTime;

        reloadAmmo = newAmmo;

        Invoke("ReloadFinish", reloadTime);

    }

    void ReloadFinish()
    {
        sourceSFX.PlayOneShot(reloadFinishSFX);

        isReloading = false;

        ammo += reloadAmmo;

        //ammoCounter.SetCounter(ammo);
    }

    void OpenBolt(BoltState state = BoltState.Open)
    {
        if (boltState == BoltState.Ready)
        {
            ammo -= 1;

            casePS.Play();
        }
        else if (boltState == BoltState.Used)
        {
            casePS.Play();
        }
        boltState = state;
        topSprite.sprite = openGun;
        sideSprite.sprite = sideOpenGun;
        //openGun.gameObject.SetActive(true);
    }

    void CloseBolt()
    {
        if (ammo > 0)
        {
            boltState = BoltState.Ready;
        }
        else
            boltState = BoltState.Closed;

        //openGun.gameObject.SetActive(false);
        topSprite.sprite = closedGun;

        sideSprite.sprite = sideClosedGun;
    }

    public void OpenBoltManual()
    {
        OpenBolt(BoltState.OpenManual);
        sourceSFX.PlayOneShot(boltOpenSFX);
    }

    public void CloseBoltManual()
    {
        CloseBolt();
        sourceSFX.PlayOneShot(boltCloseSFX);
    }
}
