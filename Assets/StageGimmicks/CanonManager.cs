using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonManager : MonoBehaviour
{
    public GameObject bulletPrefab;        // 発射する弾のプレハブ
    public float coolTime = 2f;           // 弾を発射する間隔（秒）
    public float bulletSpeed = 10f;       // 弾の速度

    public Vector3 boxCenterOffset;       // 索敵範囲の中心位置（大砲からのオフセット）
    public Vector3 boxSize = new Vector3(5f, 5f, 5f); // 索敵範囲のサイズ
    public LayerMask targetLayer;         // 索敵対象のレイヤーマスク

    bool playerInArea;
    bool isCooldown = false;

    void Update()
    {
        DetectPlayer();

        Debug.Log("playerInArea: " + playerInArea + "}");
        Debug.Log("isCooldown: " + isCooldown + "}");

        if (playerInArea && isCooldown == false)
        {
            Debug.Log("shooting");

            StartCoroutine(FireAtPlayer());
        }
    }

    private void DetectPlayer()
    {
        // 索敵範囲の中心位置を計算
        Vector3 boxCenter = transform.position + transform.TransformDirection(boxCenterOffset);

        // BoxCastで索敵を行う
        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize * 0.5f, transform.rotation, targetLayer);

        if (hits.Length > 0)
        {
            playerInArea = true;
            transform.LookAt(hits[0].transform.position);
        }
        else
        {
            playerInArea = false;
        }

    }

    private IEnumerator FireAtPlayer()
    {

        isCooldown = true;

        // 弾を生成
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // 弾に力を加える
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = transform.forward * bulletSpeed;
        }

        yield return new WaitForSeconds(coolTime);

        isCooldown = false;
    }

    private void OnDrawGizmosSelected()
    {
        // 索敵範囲をGizmoで描画
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + transform.TransformDirection(boxCenterOffset);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
