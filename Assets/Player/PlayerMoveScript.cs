using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEngine.ParticleSystem;

public class PlayerMoveScript : MonoBehaviour
{
    private Rigidbody rb;
    private CapsuleCollider coll;

    [HeaderAttribute("依存オブジェクト設定")]

    [SerializeField] private ParticleSystem particle;

    //オーディオマネージャー
    [SerializeField] private AudioManager audioManager;

    //UIキャンバス
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject UICanvas;
    private JetBarManager jetBarManager;
    CharacterController test;

    [SerializeField] private GameObject duck;

    //モデルのアニメーター
    [SerializeField] private Animator modelAnimator;


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
    private bool isTouchingWall = false;
    private bool canJet = true;

    private Vector3 moveVec;
    private Quaternion targetRotation;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        jetBarManager = UICanvas.GetComponent<JetBarManager>();
        camManager = mainCamera.GetComponent<CameraManager>();

        currentMoveSpeed = moveSpeed;
    }

    void Update()
    {
        //移動方向の更新
        moveVec = camManager.GetYawVec() * Input.GetAxisRaw("Vertical") +
                  camManager.GetYawVec(90f) * Input.GetAxisRaw("Horizontal");

        //ベクトルの正規化
        moveVec = Vector3.Normalize(moveVec);

        //もし移動を行っていたら
        if (moveVec != Vector3.zero)
        {
            //移動方向を振り向くようにする
            targetRotation = Quaternion.LookRotation(moveVec);

            //歩行アニメーションの切り替え
            modelAnimator.SetBool("running", true);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(camManager.GetYawVec());
            modelAnimator.SetBool("running", false);
        }


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
            modelAnimator.SetBool("jumping", false);
            
            //エフェクト無効
            particle.Stop();
        }
        else
        {
            modelAnimator.SetBool("jumping", true);


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
            StartCoroutine(ReleaseJet());
        }

        //ひよこ発射処理(レギュレーションのため)
        if (uiManager.GetShownScreen() == UIManager.SCREEN.GAME && Input.GetKeyDown(KeyCode.Mouse0))
            StartCoroutine(ShootDuck());

        //回転処理
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    void FixedUpdate()
    {
        //壁でチャージ中でなければ、
        if (!(isCharging && isTouchingWall))
            //移動処理を行う
            transform.position += moveVec * currentMoveSpeed * Time.fixedDeltaTime;
    }

    private void CheckOnGround()
    {
        Vector3 center = transform.position + Vector3.up * -0.7f;
        isGrounded = Physics.CheckSphere(center, 0.2f, GroundLayer);
    }

    private void CheckWallTouching()
    {
        // 壁に触れているかをチェック
        isTouchingWall = Physics.CheckBox(transform.position + Vector3.up * 0.1f, wallDetectionSize / 2, Quaternion.identity, WallLayer);
    }

    private void StartCharging()
    {
        if ((isGrounded || isTouchingWall) && canJet)
        {
            isCharging = true;
            modelAnimator.SetBool("charging", true);

            currentJetCharge = 0f; // チャージ開始時にリセット
            currentMoveSpeed = moveSpeed;

            bonusChargeRate = chargeBonusRateMax * Mathf.Clamp01(rb.velocity.magnitude / 30f);

            if(!isGrounded)
                rb.useGravity = false;

            //効果音
            audioManager.PlayChargeJet();

            //エフェクト無効
            particle.Stop();

        }
    }
    private void ChargePower()
    {
        // パワーを増加 (上限を超えない)
        currentJetCharge += (chargeRate + bonusChargeRate) * Time.deltaTime;
        currentJetCharge = Mathf.Clamp(currentJetCharge, 0f, maxJetCharge);

        //UI更新
        jetBarManager.SetJetBarFillAmount(currentJetCharge / maxJetCharge);
        jetBarManager.ShowJetBar();

        //チャージ時のブレーキ
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, chargeWallBrake * Time.deltaTime);

        currentMoveSpeed = Mathf.Clamp(currentMoveSpeed - chargeGroundBrake * Time.deltaTime, 0f, moveSpeed);

        if(!isGrounded)
            targetRotation = Quaternion.FromToRotation(Vector3.up, GetWallNormalVec());

        if (!isTouchingWall && !isGrounded)
            ResetJet();

    }

    private IEnumerator ReleaseJet()
    {

        if (isCharging)
        {
            canJet = false;


            ResetJet();

            //効果音
            audioManager.PlayReleaseJet(currentJetCharge / maxJetCharge);

            // ジェット移動
            Vector3 jetVelocity = mainCamera.transform.forward * currentJetCharge;
            rb.velocity = rb.velocity + jetVelocity;


            StartCoroutine(jetBarManager.HideJetBar());

            yield return new WaitForSeconds(0.1f);

            //エフェクト起動
            particle.Play();


            yield return new WaitForSeconds(0.2f);

            canJet = true;
        }
    }

    private void ResetJet()
    {
        isCharging = false; //フラグの変更
        modelAnimator.SetBool("charging", false); //アニメーションパラメータの変更

        rb.useGravity = true; //重力復活

        currentMoveSpeed = moveSpeed; //移動速度リセット

        targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up);

        StartCoroutine(jetBarManager.HideJetBar()); //ジェットゲージを隠す

    }

    private IEnumerator Jump()
    {
        if (isJumping)
            yield break;

        isJumping = true;


        //ジャンプ力を与える
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

        yield return new WaitForSeconds(0.1f);

        //チャージをキャンセルする
        ResetJet();

        yield return new WaitForSeconds(0.4f);

        isJumping = false;

    }
    private Vector3 GetWallNormalVec()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + Vector3.up * 0.9f, wallDetectionSize / 2, Quaternion.identity, WallLayer);

        foreach (Collider collider in colliders)
        {
            // オブジェクトの中心に向かってRayを飛ばす
            Vector3 rayOrigin = transform.position;
            Vector3 direction = (collider.bounds.center - rayOrigin).normalized;

            // Raycastで衝突点と法線を取得
            if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, Mathf.Infinity, WallLayer))
            {
                Debug.DrawRay(hit.point, hit.normal, Color.red, 1.0f); // 法線を可視化

                return hit.normal;
            }
        }

        return Vector3.zero;
    }

    private IEnumerator ShootDuck()
    {
        GameObject obj = Instantiate(duck, transform.position, Quaternion.identity);

        obj.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * 15.0f, ForceMode.Impulse);

        yield return new WaitForSeconds(5f);

        Destroy(obj);
    }

    void OnDrawGizmos()
    {
        // 壁の検知範囲を可視化
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.1f, wallDetectionSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + Vector3.up * -0.7f, 0.2f);
    }


}
