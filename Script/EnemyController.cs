using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float walkSpeed, runSpeed;
    [SerializeField] int HP;
    [SerializeField] Slider hpBar;
    [SerializeField] GameObject HPUI;
    [SerializeField] AudioClip[] voiceClip;
    [SerializeField] AudioClip punch, kick;

    GameObject target;
    TPSController tps;
    Animator animator;
    NavMeshAgent agent;
    AudioSource audioSource;
    enum STATE {IDLE, WANDER, CHASE, ATTACK, DAMAGE, DEAD};
    STATE state;
    int currentHP;
    float targetDistance;
    bool isAttack;
    bool isDead;


    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        tps = target.GetComponent<TPSController>();
        TryGetComponent(out animator);
        TryGetComponent(out agent);
        TryGetComponent(out audioSource);
    }

    void Start()
    {
        currentHP = HP;
        hpBar.maxValue = currentHP;
        hpBar.value = currentHP;
        state = STATE.IDLE;
        targetDistance = 100;
        isAttack = false;
        isDead = false;
    }

    void Update()
    {
        targetDistance = Vector3.Distance(target.transform.position, transform.position);

        switch(state)
        {
            // 棒立ち
            case STATE.IDLE:
                StopAnimation();

                // stateを遷移する
                if(CanSeePlayer())
                {
                    state = STATE.CHASE;
                }
                else if((Random.Range(0, 5000) < 1))
                {
                    state = STATE.WANDER;
                }
            break;
            
            // さまよい
            case STATE.WANDER:
                if(!agent.hasPath)
                {
                    // パスを設定
                    float posX = transform.position.x + Random.Range(-5, 5);
                    float posZ = transform.position.z + Random.Range(-5, 5);
                    Vector3 nextPos = new Vector3(posX, transform.position.y, posZ);
                    agent.SetDestination(nextPos);
                    agent.stoppingDistance = 0;

                    // アニメーションを遷移
                    StopAnimation();
                    agent.speed = walkSpeed;
                    animator.SetBool("IsWander", true);
                }

                // stateを遷移する
                if (CanSeePlayer())
                {
                    state = STATE.CHASE;
                }
                else if(Random.Range(0, 5000) < 5)
                {
                    agent.ResetPath();
                    state = STATE.IDLE;
                }
            break;

            // 追いかけ
            case STATE.CHASE:
                // パスを設定
                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 3;

                // アニメーションを遷移
                StopAnimation();
                agent.speed = runSpeed;
                animator.SetBool("IsChase", true);

                // stateを遷移する
                if(agent.remainingDistance <= agent.stoppingDistance)
                {
                    state = STATE.ATTACK;
                }
                if(targetDistance > 45)
                {
                    agent.ResetPath();
                    state = STATE.WANDER;
                }
            break;

            // 攻撃
            case STATE.ATTACK:
                // プレイヤーのほうを向く
                Vector3 lookAt = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                transform.LookAt(lookAt);

                // アニメーションを遷移
                if(!isAttack)
                {
                    float punch = Random.Range(0, 2);
                    animator.SetFloat("Punch", punch);
                    isAttack = true;
                }
                StopAnimation();
                animator.SetBool("IsAttack", true);

                // stateを遷移する
                if(targetDistance > agent.stoppingDistance + 2)
                {
                    state = STATE.CHASE;
                    isAttack = false;
                }
            break;

            // ダメージを受ける
            case STATE.DAMAGE:
                if (currentHP <= 0)  // stateを遷移する
                {
                    state = STATE.DEAD;
                }
                else  // アニメーションを遷移
                {
                    StopAnimation();
                    animator.SetBool("IsHit", true);
                    agent.ResetPath();
                }
            break;

            // 死亡
            case STATE.DEAD:
                if(!isDead)
                {
                    // アニメーションを遷移
                    StopAnimation();
                    agent.ResetPath();
                    float death = Random.Range(0, 3);
                    animator.SetFloat("Death", death);
                    animator.SetBool("IsDead", true);

                    // 当たり判定とUIを削除
                    HPUI.SetActive(false);
                    Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true;

                    // 断末魔
                    audioSource.PlayOneShot(voiceClip[Random.Range(0, voiceClip.Length)]);

                    // 敵を生成
                    GameManager.instance.CreateEnemy();
                    
                    isDead = true;
                }
            break;
        }

        // HPバーをカメラの正面にする
        HPUI.transform.rotation = Camera.main.transform.rotation;
    }

    // アニメーションを停止する
    void StopAnimation()
    {
        animator.SetBool("IsWander", false);
        animator.SetBool("IsChase", false);
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsHit", false);
    }

    // プレイヤーを見つけたかどうかを判定する
    bool CanSeePlayer()
    {
        if(targetDistance < 40)
        {
            return true;
        }
        return false;
    }

    // 殴る
    void Punch()
    {
        tps.GetDamage();
        audioSource.PlayOneShot(punch);
    }

    // 蹴る
    void Kick()
    {
        tps.GetDamage();
        audioSource.PlayOneShot(kick);
    }

    // 攻撃後
    void AfterAttack()
    {
        isAttack = false;
    }

    // ダメージを受けた後
    void AfterDamaged()
    {
        state = STATE.IDLE;
        StopAnimation();
    }

    // ダメージを受ける
    public void GetDamage()
    {
        currentHP--;
        hpBar.value = currentHP;
        state = STATE.DAMAGE;
    }

    // 倒れる
    public void GetDead() 
    {
        currentHP -= 3;
        hpBar.value = currentHP;
        state = STATE.DAMAGE;
    }

    public bool CheckDeath()
    {
        if(state == STATE.DEAD)
        {
            return true;
        }
        return false;
    }
}
