using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform pointA; // 移動先のポイントA
    [SerializeField] Transform pointB; // 移動先のポイントB
    public float speed = 2f; // 移動速度

    private Vector3 targetPosition;

    void Start()
    {
        // 最初の目標位置を設定
        targetPosition = pointB.position;
    }

    void Update()
    {
        // 足場を移動させる
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 目標位置に到達したらターゲットを切り替える
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = targetPosition == pointA.position ? pointB.position : pointA.position;
        }
    }
}
