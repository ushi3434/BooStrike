using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Vector3 offset;
    [SerializeField] float mouseSensitivity; // マウス感度
    [SerializeField] float rotationSmoothTime; // カメラ回転のスムーズさ

    private float pitch = 0f; // 上下方向の回転角度
    public  float yaw = 0f; // 水平方向の回転角度
    private Vector3 currentRotation; // 現在のカメラ回転
    private Vector3 rotationSmoothVelocity; // 回転スムーズ処理用

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // マウスカーソルをロック
    }
    
    void LateUpdate()
    {

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //回転角度の更新
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // 上下の回転角度を制限

        //カメラの回転
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        transform.position = target.transform.position + transform.rotation * offset;

        target.transform.eulerAngles = new Vector3(0, yaw + Mathf.Atan2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * Mathf.Rad2Deg, 0);
    }
}
