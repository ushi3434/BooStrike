using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAct : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
