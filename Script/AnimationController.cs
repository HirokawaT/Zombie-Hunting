using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] TPSController tps;
    [SerializeField] Animator animator;

    bool isShoot;


    void Start()
    {
        isShoot = true;
    }

    // 待機モーション
    void SetBoolIsIdleMotionFalse()
    {
        animator.SetBool("IsIdleMove", false);
    }

    // 攻撃
    void BeforeAttack()
    {
        if(isShoot)
        {
            Instantiate(tps.arrow, tps.shooting.transform.position, Camera.main.transform.rotation);
            isShoot = false;
        }
    }

    // 攻撃終わり
    void AfterAttack()
    {
        animator.SetBool("IsDrawAim", false);
        animator.SetBool("Attackable", false);
        isShoot = true;
    }
}
