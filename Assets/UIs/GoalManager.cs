using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] float scaleFactor;  // スケールの振幅（どれだけ大きく・小さくするか）
    [SerializeField] float scaleChangeSpeed;

    [SerializeField] AudioClip goalSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * scaleChangeSpeed) * scaleFactor;
        transform.localScale = new Vector3(scale, scale, scale);

        transform.eulerAngles = new Vector3(0f, rotationSpeed * Time.time, 0f);
    }

    public void PlayGoalSound()
    {
        audioSource.PlayOneShot(goalSound);
    }

}
