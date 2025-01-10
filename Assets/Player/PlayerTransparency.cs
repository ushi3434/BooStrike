using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransparency : MonoBehaviour
{
    public Transform cameraTransform; // カメラのTransform
    public Renderer playerRenderer;   // プレイヤーのRenderer
    public float distanceThreshold = 2.0f; // 半透明にする距離のしきい値
    public float transparency = 0.5f; // 半透明のアルファ値

    [SerializeField] Material normalMaterial; // 元のマテリアル
    [SerializeField] Material hantoumeiMaterial;

    void Start()
    {
        hantoumeiMaterial.color = new Color(1.0f, 1.0f, 1.0f, transparency);
    }

    void Update()
    {
        // カメラとプレイヤーの距離を計算
        float distance = Vector3.Distance(cameraTransform.position, transform.position);

        if (distance < distanceThreshold)
        {
            if (playerRenderer.material != hantoumeiMaterial)
            {
                playerRenderer.material = hantoumeiMaterial;
            }

            playerRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, distance / distanceThreshold * 0.5f);
        }
        else
        {
            if (playerRenderer.material != normalMaterial)
            {
                playerRenderer.material = normalMaterial;
            }
        }

    }
}
