using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider; // スライダー
    [SerializeField] private Text sensitivityText;
    [SerializeField] private CameraManager mainCamera;
    [SerializeField] public static float sensitivity; // 現在の感度（グローバル）

    void Start()
    {
        // スライダーの初期値を設定
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity", 1.0f);

        // スライダーの値が変更されたときに呼ばれるイベントを登録
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    // スライダーの値が変更されたときに呼ばれるメソッド
    public void OnSensitivityChanged(float value)
    {
        mainCamera.SetMouseSensitivity(value);

        // テキストに感度を表示（オプション）
        if (sensitivityText != null)
        {
            sensitivityText.text = "マウス感度: " + value.ToString("F2");
        }

    }
}
