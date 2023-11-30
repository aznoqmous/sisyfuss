using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFireBullet : RandomBehaviour
{
    [SerializeField] Vector2 _minDirection;
    [SerializeField] Vector2 _maxDirection;
    [SerializeField] float _bulletSize = 1f;
    [SerializeField] Bullet _bulletPrefab;
    [SerializeField] List<AudioClip> _fireClips;

    protected override void Do()
    {
        Bullet bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        bullet.transform.localScale = Vector3.one * _bulletSize;
        bullet.AddForce((Vector2.Lerp(_minDirection, _maxDirection, Random.value) * 100f).Rotate(transform.eulerAngles.z));
        AudioManager.Instance.PlaySound(_fireClips.PickRandom());
    }
}
