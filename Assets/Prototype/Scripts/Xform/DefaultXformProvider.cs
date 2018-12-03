using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultXformProvider : XformMonoBehaviour, XformProvider
{
    public Matrix4x4 localToWorldMatrix
    {
        get
        {
            return transform.localToWorldMatrix;
        }
    }

    public Matrix4x4 worldToLocalMatrix
    {
        get
        {
            return transform.worldToLocalMatrix;
        }
    }

    public Matrix4x4 inverseTransposeMatrix
    {
        get
        {
            return localToWorldMatrix.inverse.transpose;
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
