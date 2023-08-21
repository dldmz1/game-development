using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target; //player rigidbody

    bool isLive;

    public ParticleSystem enemyDeadEffect;

    Rigidbody2D rigid; //enemy rigidbody
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;

        spriter.color = new Color(1, 1, 1, 1);//스프라이트 색 초기화
    }

    private void LateUpdate()
    {
        if (!isLive)
            return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    private void OnEnable() //스크립트 활성화시
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        enemyDeadEffect = GameManager.instance.deadEffect;

        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive) //총알에 부딛히거나 아직 살아있으면 리턴
            return;

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (health > 0)
        {
            spriter.color = new Color(1, 0.5f, 0.5f, 1);//히트시 스프라이트 색 변화
            anim.SetTrigger("Hit");
        }
        else
        {
            spriter.color = new Color(0.3f, 0.3f, 0.3f, 1); //죽을경우 색 변경\

            isLive = false;
            coll.enabled = false;
            spriter.sortingOrder = 0;
            anim.SetBool("Dead", true);

            Invoke("PlayDead", 0.1f);
        }
    }

    public void PlayDead()
    {
        rigid.simulated = false;

        GameManager.instance.kill++;
        GameManager.instance.GetExp();
        Invoke("EffectPlay", 0.9f);
    }

    IEnumerator KnockBack()
    {
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }

    void EffectPlay()
    {
        enemyDeadEffect.transform.position = transform.position;
        enemyDeadEffect.transform.localScale = transform.localScale;
        enemyDeadEffect.Play();
    }
}