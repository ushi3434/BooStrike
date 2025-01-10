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

    //UIキャンバス
    [SerializeField] private GameObject UICanvas;   
    private UIManager uiManager;
    CharacterController test;

    //カメラ
    [SerializeField] private GameObject mainCamera; 
    private CameraManager camManager;

    //レイヤー
    [SerializeField] private LayerMask GroundLayer; //地面のレイヤー
    [SerializeField] private LayerMask WallLayer;   //壁のレイヤー

    [HeaderAttribute("基本移動設定")]
    [SerializeField] private float moveSpeed; //移動速度
    [SerializeField] private float jumpPower; //ジャンプ力

    [HeaderAttribute("ジェット設定")]
    [SerializeField] private float jetPower;            //ジェットパワー
    [SerializeField] private float maxJetCharge;        //最大ジェットパワー
    [SerializeField] private float chargeRate;          //パワーのチャージ速度 (1秒あたりの増加量)
    [SerializeField] private float chargeGroundBrake;   //チャージ中の地面のブレーキ力
    [SerializeField] private float chargeWallBrake;     //チャージ中の壁のブレーキ力
    [SerializeField] private float chargeBonusRateMax;      //チャージ開始時のボーナスチャージ
    [SerializeField] private Vector3 wallDetectionSize;


    private float currentMoveSpeed = 0f;
    private float currentJetCharge = 0f;
    private float bonusChargeRate = 0f;

    private bool isJumping = false;
    private bool isCharging = false;
    private bool isGrounded = true;
    private bool isWallTouching = false;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        uiManager = UICanvas.GetComponent<UIManager>();
        camManager = mainCamera.GetComponent<CameraManager>();

        currentMoveSpeed = moveSpeed;
    }

    void Update()
    {
        //移動方向の更新
        Vector3 moveVec = camManager.GetYawVec() * Input.GetAxisRaw("Vertical") +
                          camManager.GetYawVec(90f) * Input.GetAxisRaw("Horizontal");

        //ベクトルの正規化
        moveVec = Vector3.Normalize(moveVec);

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

        //壁でチャージ中でなければ、
        if(!(isCharging && isWallTouching))
            //移動処理を行う
            transform.position += moveVec * currentMoveSpeed * Time.deltaTime;

        //接地判定
        CheckOnGround();

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

        }

        //壁に触れているか確認
        CheckWallTouching();

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

    private void CheckOnGround()
    {
        Vector3 center = transform.position + Vector3.up * 0.10f;
        isGrounded = Physics.CheckSphere(center, 0.2f, GroundLayer);
    }

    private void CheckWallTouching()
    {
        // 壁に触れているかをチェック
        isWallTouching = Physics.CheckBox(transform.position + Vector3.up * 0.9f, wallDetectionSize / 2, Quaternion.identity, WallLayer);
    }

    private void StartCharging()
    {
        if (isGrounded || isWallTouching)
        {
            isCharging = true;
            anim.SetBool("charging", true);

            currentJetCharge = 0f; // チャージ開始時にリセット
            currentMoveSpeed = moveSpeed;

            bonusChargeRate = chargeBonusRateMax * Mathf.Clamp01(rb.velocity.magnitude / 30f);

            rb.useGravity = false;
        }
    }
    private void ChargePower()
    {
        // パワーを増加 (上限を超えない)
        currentJetCharge += (chargeRate + bonusChargeRate) * Time.deltaTime;
        currentJetCharge = Mathf.Clamp(currentJetCharge, 0f, maxJetCharge);

        //UI更新
        uiManager.SetJetBarFillAmount(currentJetCharge / maxJetCharge);
        uiManager.ShowJetBar();

        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, chargeWallBrake * Time.deltaTime);

        currentMoveSpeed = Mathf.Clamp(currentMoveSpeed - chargeGroundBrake * Time.deltaTime, 0f, moveSpeed);
    }

    private void ReleaseJet()
    {
        if (isCharging)
        {
            ResetJet();

            // ジェット移動
            Vector3 jetVelocity = mainCamera.transform.forward * currentJetCharge;
            rb.velocity = rb.velocity + jetVelocity;


            StartCoroutine(uiManager.HideJetBar());
        }
    }

    private void ResetJet()
    {
        isCharging = false; //フラグの変更
        anim.SetBool("charging", false); //アニメーションパラメータの変更

        rb.useGravity = true; //重力復活

        currentMoveSpeed = moveSpeed; //移動速度リセット

        StartCoroutine(uiManager.HideJetBar()); //ジェットゲージを隠す

    }

    private IEnumerator Jump()
    {
        if (isJumping)
            yield break;

        isJumping = true;

        //チャージをキャンセルする
        ResetJet();

        //ジャンプ力を与える
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        isJumping = false;

    }

    void OnDrawGizmos()
    {
        // 壁の検知範囲を可視化
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.9f, wallDetectionSize);
    }

}
