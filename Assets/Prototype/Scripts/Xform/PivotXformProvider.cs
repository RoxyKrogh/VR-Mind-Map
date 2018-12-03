using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotXformProvider : XformMonoBehaviour, XformProvider
{
    [SerializeField]
    private Vector3 _pivot = Vector3.zero;

    public Vector3 pivot
    {
        get { return _pivot; }
        set
        {
            _pivot = value;
            transform.hasChanged = true;
        }
    }

    void Start()
    {
        pivot = _pivot;
    }

    public Matrix4x4 localToWorldMatrix
    {
        get
        {
            return transform.localToWorldMatrix * Matrix4x4.Translate(-pivot);
        }
    }

    public Matrix4x4 worldToLocalMatrix
    {
        get
        {
            return Matrix4x4.Translate(pivot) * transform.worldToLocalMatrix;
        }
    }

    public Matrix4x4 inverseTransposeMatrix
    {
        get
        {
            return localToWorldMatrix.transpose.inverse;
        }
    }

    public Vector3 localPosition
    {
        get { return transform.localPosition; }
        set
        {
            transform.localPosition = value;
        }
    }
    public Quaternion localRotation
    {
        get { return transform.localRotation; }
        set
        {
            transform.localRotation = value;
        }
    }

    public Vector3 localScale
    {
        get
        {
            return transform.localScale;
        }
        set
        {
            transform.localScale = value;
        }
    }

    public Vector3 position
    {
        get { return transform.position; }
        set
        {
            transform.position = value;
        }
    }

    public Quaternion rotation
    {
        get { return transform.rotation; }
        set
        {
            transform.rotation = value;
        }
    }

    public Vector3 scale
    {
        get
        {
            return transform.lossyScale;
        }
    }

    public GameObject parent
    {
        get { return transform.parent.gameObject; }
        set
        {
            transform.parent = value.transform;
        }
    }

    public bool hasChanged
    {
        get
        {
            return transform.hasChanged;
        }

        set
        {
            transform.hasChanged = value;
        }
    }
}
