using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShakeUI : MonoBehaviour
{
    [SerializeField] RectTransform uiElement;    // —h‚ç‚µ‚½‚¢UI‚ÌRectTransform
    [SerializeField] float shakeDuration = 0.5f; // —h‚ê‚éŠÔ
    [SerializeField] float shakeAmount = 10f;    // —h‚ê‚Ì‹­‚³
    [SerializeField] int shakeFrequency = 30;    // —h‚ê‚Ì‰ñ”(1s)

    private Vector3 originalPosition;

    void Start()
    {
        if (uiElement == null)
        {
            uiElement = GetComponent<RectTransform>();
        }
        originalPosition = uiElement.localPosition;
    }

    public async void StartShake()
    {
        float elapsedTime = 0f;
        float interval = 1f / shakeFrequency;

        while (elapsedTime < shakeDuration)
        {
            // ƒ‰ƒ“ƒ_ƒ€‚È—h‚ê‚ğŒvZ
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount),
                0);

            // UI‚ÌˆÊ’u‚ğ•ÏX
            uiElement.localPosition = originalPosition + randomOffset * (1 - elapsedTime / shakeDuration);

            // —h‚ê‚ÌŠÔŠu‚ğ‘Ò‚Â
            await Task.Delay((int)(interval * 1000));

            // Œo‰ßŠÔ‚ğXV
            elapsedTime += interval;
        }

        // Œ³‚ÌˆÊ’u‚É–ß‚·
        uiElement.localPosition = originalPosition;
    }
}
