using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnClouds : MonoBehaviour
{
    private void Update()
    {
        if (Player.Instance.transform.position.x - transform.position.x > 100) Destroy(gameObject);
    }
}
