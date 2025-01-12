using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Vector3 delta;

    [SerializeField] float amplitude; //振幅
    [SerializeField] float speed; //速度
    [SerializeField] float offset; //オフセット

    private Vector3 targetPosition;

    private Vector3 defaultPos;

    void Start()
    {
        defaultPos = transform.position;
    }

    void Update()
    {
        // 足場を移動させる
        transform.position = defaultPos + delta * Mathf.Sin(Time.time + offset) * amplitude;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 触れたobjの親を移動床にする
            other.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 触れたobjの親をなくす
            other.transform.SetParent(null);
        }
    }
}
