using System.Collections;
using UnityEngine;

// mremireh_body/BodyColliderにアタッチ
public class BodyCollider : MonoBehaviour
{
    [SerializeField] EnemyController enemy;


    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Arrow")
        {
            enemy.GetDamage();
            Destroy(collisionInfo.gameObject);
            Debug.Log("Body Hit!");

            StartCoroutine(Coroutine());
        }
    }

    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(0.2f);

        if (enemy.CheckDeath())
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}
