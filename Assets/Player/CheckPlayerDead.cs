using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPlayerDead : MonoBehaviour
{
    [SerializeField] UIManager uiManager;

    private Rigidbody rb;

    private string targetLayer = "Death";

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayer))
            uiManager.ShowDeadMenu();
    }
}
