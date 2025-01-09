using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerMoveScript : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    [HeaderAttribute("依存オブジェクト設定")]
    
    [SerializeField] private GameObject UICanvas;   //UIキャンバス
    private UIManager uiManager;
    CharacterController test;

    [SerializeField] private GameObject mainCamera; //カメラ
    private CameraManager camManager;

    [HeaderAttribute("基本移動設定")]
    [SerializeField] private float moveSpeed; //移動速度
    [SerializeField] private float jumpPower; //ジャンプ力

    [HeaderAttribute("ジェット設定")]
    [SerializeField] private float jetPower;          //ジェットパワー
    [SerializeField] private float maxJetCharge; //最大ジェットパワー
    [SerializeField] private float chargeRate;  //パワーのチャージ速度 (1秒あたりの増加量)
    
    private float currentJetPower = 0f;
    private bool isJumping = false;
    private bool isCharging = false;

    private bool isGrounded;

    void Start()
    {
        uiManager = UICanvas.GetComponent<UIManager>();

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        camManager = mainCamera.GetComponent<CameraManager>();
        
        isGrounded = true;
    }

    void Update()
    {
        //移動方向の更新
        Vector3 moveVec = camManager.GetYawVec() * Input.GetAxisRaw("Vertical") +
                          camManager.GetYawVec(90f) * Input.GetAxisRaw("Horizontal");

        //ベクトルの正規化
        moveVec = Vector3.Normalize(moveVec);

        Debug.Log(moveVec);

        //もし移動を行っていたら
        if (moveVec != Vector3.zero)
        {
            //移動方向を振り向くようにする
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

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

        //ジャンプ処理
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Jump());
        }

        //もし地上にいたら
        if (isGrounded)
        {
            anim.SetBool("jumping", false);
        }
        else
        {
            anim.SetBool("jumping", true);

            isCharging = false; //ジェットチャージをキャンセル
            StartCoroutine(uiManager.HideJetBar()); //ジェットゲージを隠す
        }

        //ジェット処理
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!isCharging)
                StartCharging();
            else
                ChargePower();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ReleaseJet();
        }

    }


    private void StartCharging()
    {
        if (isGrounded)
        {
            isCharging = true;

            currentJetPower = 0f; // チャージ開始時にリセット

        }
    }

    private void ChargePower()
    {
        // パワーを増加 (上限を超えない)
        currentJetPower += chargeRate * Time.deltaTime;
        currentJetPower = Mathf.Clamp(currentJetPower, 0f, maxJetCharge);

        //UI更新
        uiManager.SetJetBarFillAmount(currentJetPower / maxJetCharge);
        uiManager.ShowJetBar();
    }

    private void ReleaseJet()
    {
        if (isCharging)
        {
            isCharging = false;

            // ジェット移動
            Vector3 jetVelocity = mainCamera.transform.forward * currentJetPower;
            rb.velocity = rb.velocity + jetVelocity;

            StartCoroutine(uiManager.HideJetBar());
        }
    }

    private IEnumerator Jump()
    {
        if (isJumping)
            yield break;

        isJumping = true;

        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        isJumping = false;

    }

}
