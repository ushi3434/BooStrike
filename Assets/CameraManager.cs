using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [HeaderAttribute("基本設定")]
    [SerializeField] GameObject player;
    [SerializeField] Vector3 offset;
    [SerializeField] float moveSmoothTime;

    [HeaderAttribute("視点移動設定")]
    [SerializeField] float mouseSensitivity; // マウス感度
    [SerializeField] float rotationSmoothTime; // カメラ回転のスムーズさ

    [HeaderAttribute("めり込み設定")]
    [SerializeField] LayerMask checkLayer;
    private float distance;

    private float pitch = 0f; // 上下方向の回転角度
    private float yaw = 0f; // 水平方向の回転角度
    private Vector3 currentRotation; // 現在のカメラ回転
    private Vector3 rotationVelocity; // 回転スムーズ処理用

    private Vector3 moveVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // マウスカーソルをロック

        distance = offset.magnitude;
    }
    
    void LateUpdate()
    {

        //マウスの移動を取る
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //回転角度の更新
        yaw += mouseX;

        Debug.Log(yaw);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -40f, 80f); // 上下の回転角度を制限

        //カメラの回転
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        //目標ポジションの決定
        Vector3 targetPosition = player.transform.position + transform.rotation * offset;

        //めり込んでいるか判定

        Vector3 rayOrigin = player.transform.position + Vector3.up * 0.5f;
        RaycastHit hit;

        Debug.DrawRay(rayOrigin, targetPosition - rayOrigin, Color.yellow);
        if (Physics.SphereCast(rayOrigin, 0.1f, targetPosition - rayOrigin, out hit, distance, checkLayer))
        {
            /////ここが怪しい//////
            targetPosition += transform.forward * Vector3.Distance(transform.position, hit.point) * 1.1f;
        }

        Debug.DrawRay(rayOrigin, targetPosition - rayOrigin, Color.blue);


        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref moveVelocity, moveSmoothTime);

        //transform.position = Vector3.Lerp(transform.position, targetPosition,0.8f);
    }

    public float GetYaw()
    {
        return yaw; 
    }

    public Vector3 GetYawVec(float offsetDeg = 0)
    {
        float rad = (yaw + offsetDeg) * Mathf.Deg2Rad;

        return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
    }
}
