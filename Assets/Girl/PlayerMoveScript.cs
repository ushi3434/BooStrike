using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerMoveScript : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    [SerializeField] private GameObject mainCamera;     //カメラ
    private CameraManager camManager;

    [SerializeField] private float moveSpeed;      //移動速度
    [SerializeField] private float jumpPower;      //ジャンプ力

    [SerializeField] private float jetPower;       //ジェットパワー
    [SerializeField] private float jetCooldown;    //クールダウン
    private bool canJet;                           //ジェットできるかどうか

    [SerializeField] private float rotationSpeed;  //振り向きスピード

    private bool isGrounded;

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        camManager = mainCamera.GetComponent<CameraManager>();
        
        canJet = true;

        isGrounded = true;
    }

    void Update()
    {
        //移動方向の更新
        Vector3 moveVec = camManager.GetYawVec() * Input.GetAxisRaw("Vertical") +
                          camManager.GetYawVec(90f) * Input.GetAxisRaw("Horizontal");

        //ベクトルの正規化
        moveVec = Vector3.Normalize(moveVec);

        //歩行アニメーションの切り替え
        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            //歩行アニメーションの切り替え
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
        isGrounded = Physics.CheckSphere(center, 0.16f, layer);
        anim.SetBool("jumping", !isGrounded);

        //ジャンプ処理
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
        
        //ジェット処理
        if (Input.GetKeyDown(KeyCode.Mouse1) && canJet)
            StartCoroutine(JetDash());

    }

    IEnumerator JetDash()
    {
        canJet = false;
        rb.AddForce(mainCamera.transform.forward * jetPower, ForceMode.VelocityChange);
        yield return new WaitForSeconds(jetCooldown);
        canJet = true;
    }

}
