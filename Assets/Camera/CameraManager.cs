using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    private Rigidbody playerRb;

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // マウスカーソルをロック

        distance = offset.magnitude;
    }
    
    void LateUpdate()
    {

        //マウスの移動を取る
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        //回転角度の更新
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -40f, 60f); // 上下の回転角度を制限

        //カメラの回転
        Vector3 targetRotation = new Vector3(pitch, yaw);

        // X方向に一定量移動していれば横回転
        if (Mathf.Abs(mouseX) > 0.001f)
        {
            // 回転軸はワールド座標のY軸
            transform.RotateAround(player.transform.position, Vector3.up, mouseX);
        }

        // Y方向に一定量移動していれば縦回転
        if (Mathf.Abs(mouseY) > 0.001f)
        {
            // 回転軸はカメラ自身のX軸
            transform.RotateAround(player.transform.position, transform.right, -mouseY);
        }

        //カメラ移動先の基本ポジションの決定
        Vector3 standardPosition = player.transform.position + transform.rotation * offset;

        //めり込んでいるか判定

        Vector3 rayOrigin = player.transform.position + Vector3.up * 0.5f;
        Vector3 targetPosition;
        RaycastHit hit;

        Debug.DrawRay(standardPosition, rayOrigin - standardPosition, Color.yellow);

        if (Physics.SphereCast(rayOrigin, 0.15f, standardPosition - rayOrigin, out hit, distance, checkLayer))
        {
            targetPosition = hit.point + (rayOrigin - hit.point).normalized * 0.25f;
        }
        else
        {
            //めり込んでなければ基本ポジションに移動
            targetPosition = standardPosition;
        }

        //カメラポジションの移動
        transform.position = targetPosition;
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
