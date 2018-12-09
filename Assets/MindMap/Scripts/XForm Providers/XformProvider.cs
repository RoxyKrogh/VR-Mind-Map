using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface XformProvider
{
    Matrix4x4 localToWorldMatrix
    {
        get;
    }

    Matrix4x4 worldToLocalMatrix
    {
        get;
    }

    Matrix4x4 inverseTransposeMatrix
    {
        get;
    }

    Vector3 localPosition
    {
        get;
        set;
    }

    Quaternion localRotation
    {
        get;
        set;
    }

    Vector3 localScale
    {
        get;
        set;
    }

    Vector3 position
    {
        get;
        set;
    }

    Quaternion rotation
    {
        get;
        set;
    }

    Vector3 scale
    {
        get;
    }

    GameObject parent
    {
        get;
        set;
    }

    bool hasChanged
    {
        get;
        set;
    }
}

public class XformMonoBehaviour : MonoBehaviour
{
    private XformProvider xformProvider = null;

    public XformProvider xform
    {
        get { return xformProvider ?? (xformProvider = GetComponent<XformProvider>()); }
    }

    public static Matrix4x4 RelativeXform(XformProvider from, XformProvider to)
    {
        Vector3 relativeRight = from.worldToLocalMatrix.MultiplyVector(to.localToWorldMatrix.MultiplyVector(Vector3.right));
        Vector3 relativeUp = from.worldToLocalMatrix.MultiplyVector(to.localToWorldMatrix.MultiplyVector(Vector3.up));
        Vector3 relativeForward = from.worldToLocalMatrix.MultiplyVector(to.localToWorldMatrix.MultiplyVector(Vector3.forward));
        Vector3 relativePosition = from.worldToLocalMatrix.MultiplyPoint(to.position);
        Matrix4x4 relativeXform = Matrix4x4.identity;
        Vector4 r3 = relativeXform.GetRow(3); // save the bottom row
        relativeXform.SetColumn(0, relativeRight);
        relativeXform.SetColumn(1, relativeUp);
        relativeXform.SetColumn(2, relativeForward);
        relativeXform.SetColumn(3, relativePosition);
        relativeXform.SetRow(3, r3); // restore the identity bottom row
        return relativeXform;
    }

    public static Matrix4x4 RotationMatrix(Quaternion rotation)
    {
        Vector3 right = rotation * Vector3.right;
        Vector3 up = rotation * Vector3.up;
        Vector3 forward = rotation * Vector3.forward;
        Matrix4x4 m = Matrix4x4.identity;
        m.SetColumn(0, right);
        m.SetColumn(1, up);
        m.SetColumn(2, forward);
        m.SetColumn(3, Vector4.zero);
        return m;
    }
}
