using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyFollowPlayer : MonoBehaviour
{
    [SerializeField] Vector2 _offset = Vector2.zero;
    [SerializeField] float _moveSpeed = 1f;
    [SerializeField] float _rotationSpeed = 1f;

    private void Update()
    {
        Vector3 target = Player.Instance.transform.position;
        target += (Vector3) _offset;


        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        float rotationSpeed = Mathf.Clamp(_rotationSpeed, 0f, 10f);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        //if (direction.sqrMagnitude < _minDistance) return;
        float moveSpeed = Mathf.Clamp(_moveSpeed, 0.5f, 1000f);
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + Vector2.right.Rotate(transform.localEulerAngles.z), moveSpeed * Time.deltaTime);

    }
}
