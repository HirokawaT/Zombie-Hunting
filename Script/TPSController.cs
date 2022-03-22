using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 1998

public class TPSController : MonoBehaviour
{
    public GameObject arrow;
    public GameObject shooting;
    [SerializeField] Slider hpBar;
    [SerializeField] int HP;
    [SerializeField] float walkSpeed, runSpeed, sprintSpeed;
    [SerializeField] float waitMotion;
    [SerializeField] AudioClip clip;
    
    CharacterController controller;
    Animator animator;
    AudioSource audioSource;

    int currentHP;
    float moveSpeed;
    float x, z;
    float locomotion;
    Quaternion targetRotation;
    Vector3 gravity = new Vector3(0, -9.81f, 0);
    float pitchRange = 0.3f;
    float footTime;
    float elapsedTime;
    bool waitFrag;
    Vector3 target;


    void Awake()
    {
        TryGetComponent(out controller);
        TryGetComponent(out animator);
        TryGetComponent(out audioSource);
    }

    void Start()
    {
        currentHP = HP;
        hpBar.maxValue = currentHP;
        hpBar.value = currentHP;
        transform.position = new Vector3(532, 83.884f, 480);
        targetRotation = transform.rotation;
        waitFrag = true;
    }

    async void Update()
    {
        // 入力
        if(!animator.GetBool("IsDead"))
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }
        else
        {
            x = 0;
            z = 0;
        }
        Quaternion horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        Vector3 velocity = horizontalRotation * new Vector3(x, 0, z).normalized;
        float rotationSpeed = 600 * Time.deltaTime;

        if(!animator.GetBool("IsDrawAim"))  // 通常移動
        {
            // 移動方向を向く
            if (velocity.magnitude > 0.5f) targetRotation = Quaternion.LookRotation(velocity, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);

            // 移動
            moveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : runSpeed;
            locomotion = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
            controller.Move(velocity * moveSpeed * Time.deltaTime);
            animator.SetFloat("RunSpeed", velocity.sqrMagnitude * locomotion, 0.1f, Time.deltaTime);

            // 足音
            if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                footTime += Time.deltaTime;
                if(Input.GetKey(KeyCode.LeftShift))  // Sprit
                {
                    if (footTime >= 0.25f)
                    {
                        audioSource.pitch = 1.0f + Random.Range(0, pitchRange * 2);
                        audioSource.PlayOneShot(clip);

                        footTime = 0;
                    }
                }
                else  // Run
                {
                    if (footTime >= 0.5f)
                    {
                        audioSource.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
                        audioSource.PlayOneShot(clip);

                        footTime = 0;
                    }
                }
            }
            else
            {
                footTime = 0;
            }
        }
        else if(animator.GetBool("IsDrawAim"))  // 弓を構えて移動
        {
            // 前を向く
            transform.rotation = horizontalRotation;

            // 移動
            moveSpeed = walkSpeed;
            controller.Move(velocity * moveSpeed * Time.deltaTime);
            animator.SetFloat("Walk_Front", x, 0.1f, Time.deltaTime);
            animator.SetFloat("Walk_Side", z, 0.1f, Time.deltaTime);

            // 足音
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                footTime += Time.deltaTime;
                if(footTime >= 0.5f)
                {
                    audioSource.pitch = 1.0f + Random.Range(-pitchRange * 2, 0);
                    audioSource.PlayOneShot(clip);

                    footTime = 0;
                }
            }
            else
            {
                footTime = 0;
            }
        }
        controller.Move(gravity * Time.deltaTime);

        // 弓を構える
        if (Input.GetMouseButtonDown(1) && !animator.GetBool("IsDead"))
        {
            transform.rotation = horizontalRotation;
            animator.SetBool("IsDrawAim", true);
        }

        // 攻撃モーション
        if (Input.GetMouseButtonUp(1) && animator.GetBool("IsDrawAim"))
        {
            animator.SetBool("Attackable", true);
        }

        // 攻撃キャンセル
        if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0))
        {
            animator.SetBool("IsDrawAim", false);
        }

        // 待機モーション
        if (Mathf.Abs(x) < 0.1f && Mathf.Abs(z) < 0.1f && !animator.GetBool("IsDrawAim"))
        {
            if (!animator.GetBool("IsDrawAim"))
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > waitMotion)
                {
                    if (waitFrag)
                    {
                        animator.SetFloat("StandMotion", 0);
                    }
                    else
                    {
                        animator.SetFloat("StandMotion", 1);
                    }
                    animator.SetBool("IsIdleMove", true);
                    waitFrag = !waitFrag;
                    elapsedTime = 0;
                }
                targetRotation = transform.rotation;
            }
        }
        else
        {
            animator.SetBool("IsIdleMove", false);
            elapsedTime = 0;
        }

        // 射線
        if(animator.GetBool("IsDrawAim"))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 50))
            {
                target = hit.point;
            }
            else
            {
                target = Camera.main.transform.position + Camera.main.transform.forward * 15;
            }
        }
    }

    // ダメージを受ける
    public void GetDamage()
    {
        currentHP--;
        hpBar.value = currentHP;
        if(currentHP == 0)
        {
            animator.SetBool("IsDead", true);
            GameManager.instance.GameOver();
        }
    }

    public Vector3 GetTarget()
    {
        return target;
    }
}
