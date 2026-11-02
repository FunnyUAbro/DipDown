using UnityEngine;

enum ChamberState
{
    Empty,
    Armed,
    Used
}
 
enum BoltState
{
    Closed,
    OpeningGas,
    Open,
    ClosingSpring,
    OpenManual,
    OpeningManual,
    ClosingManual,
    Ready
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
    ChamberState chamberState = ChamberState.Armed;
    BoltState boltState = BoltState.Ready;
    public float boltPositionCoef;
    [SerializeField] float shootCooldown;
    [SerializeField] float multiplierManualBoltOperation;
    [SerializeField] float punchPower;
    public int ammo;
    public int maxAmmo;
    public int reloadAmmo;
    public float reloadTime;
    public bool isReloading;
    public bool noMagLoad;
    bool lockBoltOnEmpty;
    [SerializeField] int damage;
    public AmmoType ammoType;
    public AnimationType animationType;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Transform weaponPoint;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] ParticleSystem particleSystem2;
    [SerializeField] ParticleSystem casePS;
    [SerializeField] ParticleSystem bulletPS;
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


    const float c_autoOpenCoef = 0.3f;
    const float c_autoCloseCoef = 1f - c_autoOpenCoef;
    const float c_caseDropCoef = 0.8f;
    const float c_bulletPickUpCoef = 0.8f;
    const float c_manualOpenCoef = 0.55f;
    const float c_manualCloseCoef = 1 - c_manualOpenCoef;

    void Start()
    {
        ammo = maxAmmo;
    }

    void Update()
    {
        //timerShootCooldown -= Time.deltaTime;

        //if (timerShootCooldown < 0 && boltState == BoltState.Open && boltType != BoltType.Bolt)
        //{
        //    CloseBolt();
        //}

        //boltPositionCoef += Time.deltaTime * boltDirection;

        //if (boltPositionCoef > 1f)
        //{
        //    boltDirection = 0f;
        //    boltPositionCoef = 1f;
        //}
        //else if (boltPositionCoef < 0)
        //{
        //    boltDirection = 0f;
        //    boltPositionCoef = f;
        //}

        switch (boltState)
        {
            case BoltState.OpeningGas:
                { 
                    float openTime = shootCooldown * c_autoOpenCoef;
                    float thisFrameTimeToOpenTime = Time.deltaTime / openTime;
                    boltPositionCoef += thisFrameTimeToOpenTime;


                    //if (boltPositionCoef > c_caseDropCoef)
                    //{
                        
                    //}

                    if (boltPositionCoef > 1f)
                    {
                        boltPositionCoef = 1f;
                        

                        if (ammo > 0)
                        {
                            boltState = BoltState.ClosingSpring;
                        }
                        else
                        {
                            boltState = BoltState.Open;
                        }

                        OpenBolt();
                    }
                }
                break;

            case BoltState.Open:

                break;

            case BoltState.ClosingSpring:
                //Двигает и закрывает Bolt
                {
                    float closeTime = shootCooldown * c_autoCloseCoef;
                    float thisFrameTimeToOpenTime = Time.deltaTime / closeTime;
                    boltPositionCoef -= thisFrameTimeToOpenTime;

                    if (boltPositionCoef < 0f)
                    {
                        //if (chamberState == ChamberState.Armed)
                        //{
                        //    //Выкидывает патрон
                        //}
                        //else if (chamberState == ChamberState.Used)
                        //{
                        //    //Выкидует гильдзу
                        //}

                        boltPositionCoef = 0f;
                        CloseBolt();
                    }
                }
                break;

            case BoltState.Closed:
                //Не готов/готов стрелять
                break;

            case BoltState.OpeningManual:
                { 
                    float openTime = shootCooldown * multiplierManualBoltOperation * c_manualOpenCoef;
                    float thisFrameTimeToOpenTime = Time.deltaTime / openTime;
                    boltPositionCoef += thisFrameTimeToOpenTime;

                    if (boltPositionCoef > 1f)
                    {
                        boltPositionCoef = 1f;
                        OpenBolt(); 
                        boltState = BoltState.OpenManual;
                    }
                }
                break;

            case BoltState.OpenManual:
                break;

            case BoltState.ClosingManual:
                {
                    float openTime = shootCooldown * multiplierManualBoltOperation * c_manualCloseCoef;
                    float thisFrameTimeToOpenTime = Time.deltaTime / openTime;
                    boltPositionCoef -= thisFrameTimeToOpenTime;

                    if (boltPositionCoef < 0f)
                    {
                        boltPositionCoef = 0f;
                        CloseBolt();
                    }
                }
                break;

            case BoltState.Ready:
                break;
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
            if (chamberState == ChamberState.Armed)
            {
                Shoot();

                if (boltType != BoltType.Bolt)
                {
                    boltState = BoltState.OpeningGas;
                }
            }
            else
            {
                //Проиграть вот этот глухой звук тошо нет патрона
                boltState = BoltState.Closed;
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

        chamberState = ChamberState.Used;

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

    void OpenBolt()
    {
        //if (boltState == BoltState.Ready)
        //{
        //    ammo -= 1;

        //    casePS.Play();
        //}
        //else if (boltState == BoltState.Used)
        //{
        //    casePS.Play();
        //}
        //boltState = state;

        //openGun.gameObject.SetActive(true);

        if (chamberState == ChamberState.Armed)
        {
            //Выкидывает патрон
            bulletPS.Play();
        }
        else if (chamberState == ChamberState.Used)
        {
            //Выкидует гильдзу
            casePS.Play();
        }
        //Заряжает патрон
        if (ammo > 0)
        {
            ammo--;
            chamberState = ChamberState.Armed;
        }
        else
            chamberState = ChamberState.Empty;

        topSprite.sprite = openGun;
        sideSprite.sprite = sideOpenGun;

    }

    void CloseBolt()
    {
        //if (ammo > 0)
        //{
        //    boltState = BoltState.Ready;
        //}
        //else
        //    boltState = BoltState.Closed;

        //openGun.gameObject.SetActive(false);

        boltState = BoltState.Ready;

        topSprite.sprite = closedGun;

        sideSprite.sprite = sideClosedGun;
    }

    public void StartOpenBoltManual()
    {
        boltState = BoltState.OpeningManual;
        sourceSFX.PlayOneShot(boltOpenSFX);
    }

    public void FinishBoltManual()
    {
        boltState = BoltState.ClosingManual;
        sourceSFX.PlayOneShot(boltCloseSFX);
    }
}
