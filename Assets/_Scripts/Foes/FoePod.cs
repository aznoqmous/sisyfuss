using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoePod : MonoBehaviour
{
    [SerializeField] float _preventAttachedMovementPrioty = 0f;
    [SerializeField] bool _preventMovementWhenAttached = false;
    [SerializeField] List<PodDirection> _allowedToConnectToDirections;
    [SerializeField] PodDirection _direction;
    [SerializeField] PodType _type;
    [SerializeField] CircleCollider2D _collider;

    public float PreventAttachedMovementPriority { get { return _preventAttachedMovementPrioty;  } }
    public List<PodDirection> AllowedToConnectToDirections { get { return _allowedToConnectToDirections; } }
    public PodDirection Direction { get { return _direction;  } }

    Foe _foe;
    public Foe Foe { get { return _foe; } }
    public PodType Type { get { return _type; } }
    FoePod _attachedPod;
    public FoePod AttachedPod { get { return _attachedPod; } }
    public Foe AttachedFoe { get { return _attachedPod.Foe; } }

    public bool IsAttached { get { return _attachedPod != null; } }
    public bool IsAvailable { get { return !IsAttached && !HasAwaitedFoeAttachment; } }

    Foe _awaitedFoeAttachment;
    bool HasAwaitedFoeAttachment { get { return _awaitedFoeAttachment!= null; } }



    public void SetFoe(Foe foe)
    {
        _foe = foe;
    }

    public void SetAttachedPod(FoePod foePod)
    {
        _attachedPod = foePod;
        _collider.enabled = foePod == null;
        if (foePod == null) return;
        if (PreventAttachedMovementPriority > _attachedPod.PreventAttachedMovementPriority)
        {
            _attachedPod.Foe.PreventMovement();

            Vector3 foeScale = _attachedPod.Foe.transform.localScale;
            if (Direction == PodDirection.Bottom && foePod.Direction == Direction)
            {
                foeScale.y *= -1f;
                _attachedPod.Foe.transform.localScale = foeScale;
            }
        }

    }
    public void AttachTo(FoePod foePod)
    {
        if (PreventAttachedMovementPriority < foePod.PreventAttachedMovementPriority)
        {
            foePod.AttachTo(this);
            return;
        }
        SetAttachedPod(foePod);

        foePod.Foe.transform.position += transform.position - foePod.transform.position;
        _foe.Joint.connectedBody = foePod.Foe.RigidBody;
        _foe.Joint.connectedAnchor = (Vector2) (_foe.transform.position - foePod.Foe.transform.position) / foePod.Foe.transform.localScale; 
        //(Vector2) (_foe.transform.position - AttachedFoe.transform.position) / _foe.transform.localScale / foePod.Foe.transform.localScale;
        _foe.Joint.enabled = true;


        foePod.SetAttachedPod(this);
    }

    public void Detach()
    {
        if (!IsAttached) return;
        FoePod attachedPod = _attachedPod;
        _attachedPod = null;

        if (PreventAttachedMovementPriority > attachedPod.PreventAttachedMovementPriority)
        {
            attachedPod.Foe.EnableMovement();

            Vector3 foeScale = attachedPod.Foe.transform.localScale;
            if (Direction == PodDirection.Bottom && attachedPod.Direction == Direction)
            {
                foeScale.y *= -1f;
                attachedPod.Foe.transform.localScale = foeScale;
            }
        }

        _foe.Joint.enabled = false;
        attachedPod.SetAttachedPod(null);
    }

    public void Disable()
    {
        _collider.enabled = false;
    }
    public void Enable()
    {
        _collider.enabled = true;
    }

    public bool CanConnectToPod(FoePod pod)
    {
        if (pod.Type == PodType.Both && _type == PodType.Both) return false;
        if (pod.Type == PodType.Plus && _type == PodType.Plus) return false;
        if (pod.Type == PodType.Minus && _type == PodType.Minus) return false;

        return _allowedToConnectToDirections.Contains(pod.Direction);
    }
    public FoePod CanConnectToFoe(Foe foe)
    {
        foreach(FoePod fp in foe.Pods)
        {
            if(CanConnectToPod(fp) && fp.CanConnectToPod(this)) return fp;
        }
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        FoePod pod = collider.GetComponent<FoePod>();
        if (pod != null && pod.Foe != _foe && CanConnectToPod(pod) && pod.CanConnectToPod(this))
        {
            AttachTo(pod);
        }
    }
}
public enum PodDirection
{
    Left, Right, Top, Bottom
}
public enum PodType
{
    Plus, 
    Minus, 
    Both, // plus, minus but not both
    All
}