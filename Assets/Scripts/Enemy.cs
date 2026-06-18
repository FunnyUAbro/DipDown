using UnityEngine;

public class Enemy : MonoBehaviour
{
    float attackCooldown;
    float bushTimer;
    
    [SerializeField] int MaxHp;
    [SerializeField] int damage;
    int hp;

    [SerializeField] bool isCrawler;
    bool inBush;

    [SerializeField] float speed;
    [SerializeField] float attackRate;

    [SerializeField] Transform target;
    [SerializeField] Player pl;
    [SerializeField] Animator animator;

    [SerializeField] float stepTime;
    [SerializeField] AudioClip[] stepsSounds;
    [SerializeField] AudioSource audioSource;

    [SerializeField] Collider2D[] hitCollider;

    [SerializeField] SpriteRenderer enemySprite;

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        attackCooldown -= Time.deltaTime;

        Move();
        Rotate();

        if (inBush)
        {
            bushTimer -= Time.deltaTime;

            if (bushTimer < 0)
            {
                int random = Random.Range(0, stepsSounds.Length);

                audioSource.PlayOneShot(stepsSounds[random]);

                bushTimer = stepTime;
            }
        }
    }

    void Spawn()
    {        
        if (isCrawler == true)
        {
            animator.Play("ZombieCrawl");
        }
        else
        {
            int randomNum = Random.Range(1, 3);

            animator.Play($"ZomnieWalk{randomNum}");
        }

        hp = MaxHp;

        pl = FindAnyObjectByType<Player>();
        target = pl.transform;
    }

    void Move()
    {
        transform.position = transform.position + (target.position - transform.position).normalized * speed * Time.deltaTime;
    }

   void Rotate()
   {
        Vector2 dir = target.position - transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        
        transform.rotation = Quaternion.Euler(0, 0, angle);

   } 

    void Attack()
    {
        pl.TakeDamage(damage);
        attackCooldown = attackRate;
    }

    public void TakeDamage(int damageIn)
    {
        hp -= damageIn;

        if (hp <= 0 )
        {
            Die();
        }
    }

    void Die()
    {
        if (isCrawler)
        {
            animator.Play("ZombieCrawlDead");
        }
        else
        {
            int randomNum = Random.Range(1, 4);

            animator.Play($"ZombieDead{randomNum}");
        }

        Debug.Log("Противник убит");

        pl.AddMoney(1);

        enemySprite.sortingOrder = 0;

        for  (int i = 0; i < hitCollider.Length; i++)
        {
            hitCollider[i].enabled = false;
        }

        this.enabled = false;
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && attackCooldown < 0)
        {
            Attack();
        }

        if (collision.tag == "Bush")
        {
            inBush = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Bush")
        {
            inBush = false;
        }
    }
}
