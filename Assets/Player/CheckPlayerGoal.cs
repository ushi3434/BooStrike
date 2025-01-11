using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerGoal : MonoBehaviour
{
    [SerializeField] UIManager uiManager;

    private Rigidbody rb;

    private string targetLayer = "Goal";

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayer))
        {
            collision.gameObject.GetComponent<GoalManager>().PlayGoalSound();
            uiManager.ShowGoalMenu();
        }
    }
}
