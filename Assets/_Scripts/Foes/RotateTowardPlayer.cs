using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class RotateTowardPlayer : MonoBehaviour
{
    private void Update()
    {
        Vector3 dist = transform.position - Player.Instance.transform.position;
        Vector3 angles = transform.localEulerAngles;
        angles.z = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg + 90;
        transform.localEulerAngles = angles;
    }
}
