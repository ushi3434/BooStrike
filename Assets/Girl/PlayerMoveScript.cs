using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMoveScript : MonoBehaviour
{
    Rigidbody rb;
    Animator  anim;
    [SerializeField] float moveSpeed = 15.0f;
    [SerializeField] float jumpPower = 6.5f; //ジャンプ力

    Vector3 moveVec;
    Vector3 jumpVec; //ジャンプする方向

    float startAngle = 0.0f; //回転開始角度
    float targetAngle = 90.0f; //回転後角度

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        moveVec = new Vector3(0.0f, 0.0f, 0.0f);
        jumpVec = new Vector3(0.0f, 1.0f, 0.0f); //ジャンプの方向

        if (Input.GetKey(KeyCode.A)) //左が入力されたとき
        {
            moveVec.x = -1.0f;

            startAngle = this.transform.eulerAngles.y;
            targetAngle = -90.0f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveVec.x = 1.0f;

            startAngle = this.transform.eulerAngles.y;
            targetAngle = 90.0f;
        }
        else
        {
            moveVec.x = 0;
        }

        if (Input.GetKey(KeyCode.W)) //左が入力されたとき
        {
            moveVec.z = 1.0f;

            startAngle = this.transform.eulerAngles.y;
            targetAngle = 0.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVec.z = -1.0f;

            startAngle = this.transform.eulerAngles.y;
            targetAngle = 180.0f;
        }
        else
        {
            moveVec.z = 0;
        }

        Vector3 center = transform.position + Vector3.up * 0.10f;
        float radius = 0.16f;
        LayerMask layer = LayerMask.GetMask("Ground");
        bool isGround = Physics.CheckSphere(center, radius, layer);
        anim.SetBool("jumping", !isGround);

        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpVec * jumpPower, ForceMode.Impulse);
        }

        //現在角度と目標角度から、動くべき角度を求める
        float angle = Mathf.LerpAngle(startAngle, targetAngle, 1.0f);

        //現在の角度に適用する
        this.transform.eulerAngles = new Vector3(0.0f, angle, 0.0f);

        //歩行アニメーションの切り替え
        if (moveVec != Vector3.zero)
        {
            anim.SetBool("running", true);
        }
        else
        {
            anim.SetBool("running", false);
        }
    }
    void FixedUpdate()
    {
        transform.position += moveVec * moveSpeed * Time.deltaTime;
    }
}
