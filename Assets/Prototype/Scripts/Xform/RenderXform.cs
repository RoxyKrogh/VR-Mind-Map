using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RenderXform : XformMonoBehaviour {

    static Color gizmoColor = Color.green;

    private Renderer _renderer;

    [SerializeField][HideInInspector]
    private string _localToWorldMatrix_var = "_ModelM";
    [SerializeField][HideInInspector]
    private string _inverseTransposeMatrix_var = "_ModelM_IT";

    private int _localToWorldMatrix_id;
    private int _inverseTransposeMatrix_id;

    public string localToWorldMatrix_var
    {
        get
        {
            return _localToWorldMatrix_var;
        }
        set
        {
            _localToWorldMatrix_var = value;
            _localToWorldMatrix_id = Shader.PropertyToID(_localToWorldMatrix_var);
        }
    }

    public string inverseTransposeMatrix_var
    {
        get
        {
            return _inverseTransposeMatrix_var;
        }
        set
        {
            _inverseTransposeMatrix_var = value;
            _inverseTransposeMatrix_id = Shader.PropertyToID(_inverseTransposeMatrix_var);
        }
    }

    private void OnDrawGizmos()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null)
        {
            Mesh mesh = mf.sharedMesh;
            if (mesh != null && mesh.vertexCount > 0)
            {
                Gizmos.matrix = xform.localToWorldMatrix;
                Gizmos.color = gizmoColor;
                Gizmos.DrawWireMesh(mesh);
            }
        }
    }

    // Use this for initialization
    void Start () {
        _renderer = GetComponent<Renderer>();
        localToWorldMatrix_var = _localToWorldMatrix_var;
        inverseTransposeMatrix_var = _inverseTransposeMatrix_var;
        ApplyXformToMaterial();
    }

    private void OnEnable()
    {
        ApplyXformToMaterial();
        _renderer.material.EnableKeyword("OVERRIDE_TRANSFORM");
    }

    private void OnDisable()
    {
        _renderer.material.DisableKeyword("OVERRIDE_TRANSFORM");
    }

    private void OnPreCull()
    {
        if (xform.hasChanged)
            ApplyXformToMaterial();
    }

    private void ApplyXformToMaterial()
    {
        Matrix4x4 modelMatrix = xform.localToWorldMatrix;
        Matrix4x4 itMatrix = xform.inverseTransposeMatrix;
        Material material = _renderer.material;
        material.SetMatrix(_localToWorldMatrix_id, modelMatrix);
        material.SetMatrix(_inverseTransposeMatrix_id, itMatrix);
    }

    [UnityEditor.CustomEditor(typeof(RenderXform))]
    public class RenderXformEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            RenderXform rx = (RenderXform)target;
            rx.localToWorldMatrix_var = UnityEditor.EditorGUILayout.TextField("localToWorldMatrix", rx._localToWorldMatrix_var);
            rx.inverseTransposeMatrix_var = UnityEditor.EditorGUILayout.TextField("inverseTransposeMatrix", rx._inverseTransposeMatrix_var);
            //base.OnInspectorGUI();
        }
    }
}
