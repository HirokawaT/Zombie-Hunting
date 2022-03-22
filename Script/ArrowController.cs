using UnityEngine;
#pragma warning disable 1998

public class ArrowController : MonoBehaviour
{
    [SerializeField] float arrowSpeed;

    TPSController tps;
    Rigidbody rb;
    bool isImpulse;
    float elapsedTime;


    void Awake()
    {
        tps = GameObject.FindGameObjectWithTag("Player").GetComponent<TPSController>();
        TryGetComponent(out rb);
    }

    void Start()
    {
        isImpulse = true;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime > 10)
        {
            Destroy(gameObject);
        }
        float dir = Vector3.Distance(transform.position, tps.gameObject.transform.position);
    }

    void FixedUpdate()
    {
        if (isImpulse)
        {
            gameObject.transform.LookAt(tps.GetTarget());
            rb.AddForce(transform.forward * arrowSpeed, ForceMode.Impulse);
            isImpulse = false;
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if(collisionInfo.gameObject.tag != "Player")
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
