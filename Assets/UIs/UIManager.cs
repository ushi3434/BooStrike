using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [HeaderAttribute("依存オブジェクト設定")]
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject player;
    [SerializeField] GameObject jetBar;
    [SerializeField] Image jetBarFill;

    [HeaderAttribute("ジェットチャージバー設定")]
    [SerializeField] Vector3 jetBarOffset;

    private PlayerMoveScript playerMoveScript;

    private Vector3 jetBarVelocity = Vector3.zero;
    private CanvasGroup canvasGroup;
    private bool jetBarFading = false;

    void Start()
    {
        playerMoveScript = player.GetComponent<PlayerMoveScript>();

        //ジェットバー
        jetBar.GetComponent<CanvasGroup>().alpha = 0f;

        jetBar.transform.position = mainCamera.WorldToScreenPoint(player.transform.position + Vector3.up * 0.5f) + jetBarOffset;

        canvasGroup = jetBar.GetComponent<CanvasGroup>();

    }

    void Update()
    {
        Debug.Log(jetBar.GetComponent<CanvasGroup>().alpha);
        //ジェットバー

        //ポジション移動
        jetBar.transform.position = Vector3.SmoothDamp(jetBar.transform.position, mainCamera.WorldToScreenPoint(player.transform.position + Vector3.up * 0.5f) + jetBarOffset, ref jetBarVelocity, 0.03f);

    }

    public void SetJetBarFillAmount(float ratio)
    {
        jetBarFill.fillAmount = ratio;
    }

    public void ShowJetBar()
    {
        jetBar.GetComponent<CanvasGroup>().alpha = 1f;
    }

    public IEnumerator HideJetBar()
    {
        //もし、消去処理実行中か既に消えている場合は
        if (jetBarFading || canvasGroup.alpha == 0f)
            yield break; //中断

        jetBarFading = true;

        float elapsedTime = 0f;
        float fadeDuration = 0.3f;

        // 現在の透明度から0まで徐々に変化
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 0f;

        jetBarFading = false;

    }

}
