using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMoveScript : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    [SerializeField] private GameObject mainCamera; //カメラ
    [SerializeField] private float moveSpeed = 15.0f; //移動速度
    [SerializeField] private float jumpPower = 6.5f; //ジャンプ力

    [HideInInspector] public Vector3 moveVec; //移動ベクトル

    private Vector3 jumpVec; //ジャンプベクトル

    private float radius = 0.16f;

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        jumpVec = Vector3.up; //ジャンプの方向

    }

    void Update()
    {
        //移動方向の更新

        moveVec = mainCamera.transform.forward * Input.GetAxisRaw("Vertical") +
                  mainCamera.transform.right * Input.GetAxisRaw("Horizontal");

        //ベクトルの正規化
        moveVec = Vector3.Normalize(moveVec);

        //歩行アニメーションの切り替え
        if (moveVec != Vector3.zero)
        {
            anim.SetBool("running", true);
        }
        else
        {
            anim.SetBool("running", false);
        }

        //移動処理
        transform.position += moveVec * moveSpeed * Time.deltaTime;

        //接地判定

        Vector3 center = transform.position + Vector3.up * 0.10f;
        LayerMask layer = LayerMask.GetMask("Ground");
        bool isGround = Physics.CheckSphere(center, radius, layer);
        anim.SetBool("jumping", !isGround);

        //ジャンプ処理

        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpVec * jumpPower, ForceMode.Impulse);
        }
    }

    //プレイヤーのアングル取得
    float GetAngle()
    {
        return Mathf.Atan2(moveVec.x, moveVec.z * Mathf.Rad2Deg);
    }
}
