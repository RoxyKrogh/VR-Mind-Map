using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodlePen : MonoBehaviour
{

    private Mesh doodleMesh;
    private List<Vector3> linePoints;
    private List<Vector3> lineNormals;
    private List<Vector4> lineTangents;
    private List<int> lineIndices;

    public MeshFilter doodleTarget;

    private bool _isPenDown = false;

    private Activatable activatable;

    private Bubble _targetBubble;

    public Renderer debugRenderer;

    [SerializeField]
    public bool isPenDown
    {
        get { return _isPenDown; }
        set
        {
            bool wasDown = _isPenDown;
            _isPenDown = value;
            if (!wasDown && _isPenDown && isInBubble)
                NewDoodle();
            else if (wasDown && !_isPenDown && isInBubble)
                EndDoodle();
        }
    }

    public Bubble targetBubble
    {
        get { return _targetBubble; }
        set
        {
            Bubble wasIn = _targetBubble;
            _targetBubble = value;

            if ((!wasIn && isInBubble) || (wasIn != targetBubble))
                EnteredBubble();
            else if (wasIn && !isInBubble)
                ExitedBubble();

            
        }
    }

    public bool isInBubble
    {
        get { return _targetBubble != null; }
    }

    public bool isDrawing
    {
        get { return isPenDown && isInBubble; }
    }

    // Use this for initialization
    void Start()
    {
        //NewDoodle();
    }

    void EndDoodle()
    {
        if (targetBubble != null && doodleTarget.mesh != null)
        {
            MeshFilter newTarget = Instantiate(doodleTarget);
            newTarget.sharedMesh = null;
            Debug.Log("Binding doodle to " + targetBubble);
            //targetBubble.AddDoodle(doodleTarget);
            doodleTarget = newTarget;
        }
    }

    void NewDoodle()
    {
        bool haveDoodle = doodleTarget.mesh != null;
        if (haveDoodle)
            EndDoodle();
        doodleTarget.transform.position = transform.position;
        doodleMesh = new Mesh();
        linePoints = new List<Vector3>();
        lineNormals = new List<Vector3>();
        lineTangents = new List<Vector4>();
        lineIndices = new List<int>();
        doodleTarget.mesh = doodleMesh;
        UpdateDoodleMesh();

        activatable = GetComponent<Activatable>();

        targetBubble.AddDoodle(doodleTarget);
    }

    void UpdateDoodleMesh()
    {
        while (lineIndices.Count < linePoints.Count)
            lineIndices.Add(lineIndices.Count);
        doodleMesh.SetVertices(linePoints);
        doodleMesh.SetNormals(lineNormals);
        doodleMesh.SetTangents(lineTangents);
        doodleMesh.SetIndices(lineIndices.ToArray(), MeshTopology.LineStrip, 0);
    }

    void DrawLine()
    {
        Debug.Log("Bubble we're in: " + targetBubble.name);
        bool editLast = false;
        Vector3 doodlePoint = doodleTarget.transform.worldToLocalMatrix.MultiplyPoint(transform.position);

        Vector3 lastPoint = linePoints.Count > 0 ? linePoints[linePoints.Count - 1] : doodlePoint;
        Vector3 lastNormal = lineNormals.Count > 0 ? lineNormals[lineNormals.Count - 1] : Vector3.zero;
        Vector3 lastTangent = lineTangents.Count > 0 ? lineTangents[lineTangents.Count - 1] : Vector4.zero;
        Vector3 thisLine = doodlePoint - lastPoint;
        Vector3 lastLine = lastPoint - (linePoints.Count > 1 ? linePoints[linePoints.Count - 2] : lastPoint);
        if (linePoints.Count > 1)
        {
            float diff = Vector3.Dot(thisLine.normalized, lastLine.normalized);

            const float threshold = 0.99f;
            editLast = editLast || (diff > threshold);
        }
        Matrix4x4 lineSpace = Matrix4x4.LookAt(doodlePoint, lastPoint, Vector3.up);
        Vector3 thisNormal = lineSpace.MultiplyVector(Vector3.right).normalized; //thisLine.normalized;
        if (Vector3.Dot(thisNormal, lastNormal) < 0)
            thisNormal = -thisNormal;
        Vector3 thisTangent = lastLine.normalized;

        float straightness = Vector3.Dot(thisLine.normalized, thisTangent);

        if (editLast)
        {
            linePoints[linePoints.Count - 1] = doodlePoint;
            if (lineNormals.Count > 1)
            {
                lineTangents[lineNormals.Count - 2] = thisTangent;
                lineNormals[lineNormals.Count - 2] = thisNormal;
            }
        }
        else
        {
            linePoints.Add(doodlePoint);
            lineTangents.Add(thisTangent);
            lineNormals.Add(thisNormal);
        }
        UpdateDoodleMesh();
    }

    // Update is called once per frame
    void Update()
    {
        isPenDown = activatable == null || !activatable.enabled || activatable.isActive;

        if (isDrawing)
        {
            if (linePoints.Count == 0 || (transform.position - doodleTarget.transform.localToWorldMatrix.MultiplyPoint(linePoints[linePoints.Count - 1])).sqrMagnitude > 0.0001f)
            {
                DrawLine();
                transform.hasChanged = false;
            }
        }
        //Graphics.DrawMesh(doodleMesh, doodleTarget.localToWorldMatrix, doodleMaterial, 0);
    }

    public void EnteredBubble()
    {
        if (!isInBubble)
            return;
        Debug.Log("Entered bubble " + targetBubble);
        if (isPenDown)
            NewDoodle();
    }

    public void ExitedBubble()
    {
        if (isInBubble)
            return;
        Debug.Log("Exited bubble " + targetBubble);
        EndDoodle();
    }
}
