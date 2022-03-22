using UnityEngine;
#pragma warning disable 1998

public class CameraController : MonoBehaviour
{
    [SerializeField] Animator animator;

    [Header("Camera")]
    [SerializeField] Transform target;  // 追跡対象
    [SerializeField] float distance;  // 追跡対象との距離
    [SerializeField] float eyeHeight;  // カメラの高さ
    [SerializeField] [Range(1f, 100f)] float damping;  // 遅延
    [SerializeField] float mouseSensibility;  // マウス感度

    Vector3 targetPos;
    Vector3 cameraHeight;
    Vector3 cameraPos;
    Vector3 highAnglePos;
    float cameraRotY;
    float cameraRotX;  
    float cameraEye;
    float currentDistance;


    void Start()
    {
        // カメラの初期位置
        targetPos = target.position;
        cameraEye = eyeHeight;
        cameraHeight = targetPos + Vector3.up * cameraEye;
        transform.position = cameraHeight + Vector3.back * distance;
        currentDistance = distance;
    }

    async void LateUpdate()
    {
        // 移動、回転
        if(!GameManager.instance.isPause)
        {
            cameraRotY -= Input.GetAxis("Mouse Y") * mouseSensibility;
            cameraRotX -= Input.GetAxis("Mouse X") * mouseSensibility;
            cameraRotY = Mathf.Clamp(cameraRotY, -45, 70);
            cameraPos.x = Mathf.Sin(cameraRotX * Mathf.Deg2Rad) * Mathf.Cos(cameraRotY * Mathf.Deg2Rad) * currentDistance;
            cameraPos.y = Mathf.Sin(cameraRotY * Mathf.Deg2Rad) * currentDistance;
            cameraPos.z = -Mathf.Cos(cameraRotX * Mathf.Deg2Rad) * Mathf.Cos(cameraRotY * Mathf.Deg2Rad) * currentDistance;
            if (cameraRotY > -44 && cameraRotY < 69)
            {
                transform.RotateAround(targetPos, transform.right, -Input.GetAxis("Mouse Y") * mouseSensibility);
            }
            transform.RotateAround(targetPos, target.up, Input.GetAxis("Mouse X") * mouseSensibility);
        }

        // 距離
        if(animator.GetBool("IsDrawAim") && !animator.GetBool("IsDead"))  
        {
            highAnglePos = target.position + target.right * 0.5f;
            currentDistance = Mathf.Lerp(currentDistance, 2, Time.deltaTime * 5);
            targetPos = Vector3.Lerp(targetPos, highAnglePos, Time.deltaTime * damping);
            cameraEye = Mathf.Lerp(cameraEye, eyeHeight * 1.3f, Time.deltaTime * damping);
        }
        else 
        {
            if(cameraRotY < -15)
            {
                currentDistance = Mathf.Lerp(currentDistance, 2, Time.deltaTime * damping);
                targetPos = Vector3.Lerp(targetPos, target.position, Time.deltaTime * damping * 5);
            }
            else 
            {
                currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * damping);
                if (cameraRotY > 0 && Input.GetAxis("Vertical") < 0)
                {
                    highAnglePos = target.position - new Vector3(transform.forward.x, 0, transform.forward.z) * 8 / 175 * cameraRotY;
                    targetPos = Vector3.Lerp(targetPos, highAnglePos, Time.deltaTime * damping);
                }
                else
                {
                    targetPos = Vector3.Lerp(targetPos, target.position, Time.deltaTime * damping);
                }
            }
            cameraEye = Mathf.Lerp(cameraEye, eyeHeight, Time.deltaTime * damping);
        }

        // 更新
        cameraHeight = targetPos + Vector3.up * cameraEye;
        transform.position = cameraHeight + cameraPos;
    }
}
