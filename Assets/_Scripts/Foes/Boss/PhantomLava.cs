using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomLava : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(TimedDestroy());
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        PhantomStone ps = collision.GetComponent<PhantomStone>();
        if (ps != null)
        {
            ps.Erase();
            ps.Dust();
        }
    }
    [SerializeField] GameObject _containerGameObject;
    [SerializeField] ParticleSystem _particleSystem;
    IEnumerator TimedDestroy()
    {
        yield return new WaitForSeconds(2f);
        var emission = _particleSystem.emission;
        emission.enabled= false;
        Destroy(_containerGameObject, 2f);
    }
}
