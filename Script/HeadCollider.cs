using System.Collections;
using UnityEngine;

// HeadColliderにアタッチ
public class HeadCollider : MonoBehaviour
{
    [SerializeField] EnemyController enemy;


    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Arrow")
        {
            enemy.GetDead();
            Destroy(collisionInfo.gameObject);
            Debug.Log("Head Hit!");

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
