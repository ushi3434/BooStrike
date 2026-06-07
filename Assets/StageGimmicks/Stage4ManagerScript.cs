using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage4ManagerScript : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Text timeText;
    [SerializeField] GameObject goalObject;


    private float GoalTimer;

    // Start is called before the first frame update
    void Start()
    {
        GoalTimer = 30f;
        StartCoroutine(DisableStage4UI());
    }

    void Update()
    {
        GoalTimer -= Time.deltaTime;

        GoalTimer = Mathf.Max(GoalTimer, 0);

        timeText.text = GoalTimer.ToString();

        if (GoalTimer == 0f)
        {
            goalObject.SetActive(true);
            timeText.color = new Color(1.0f, 1.0f, 1.0f, 0f);
        }
    }


    private IEnumerator DisableStage4UI()
    {
        yield return new WaitForSeconds(3f);

        float elapsedTime = Time.time;

        while (elapsedTime < 1.0f)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / 1.0f);
            yield return null; // 次のフレームまで待機
        }

        canvasGroup.alpha = 0f;

    }
}
