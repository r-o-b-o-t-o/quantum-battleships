using System.Linq;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public MeshFilter ocean;

    [SerializeField] private Transform oceanSnapPoint;
    [SerializeField] private Transform rotationOffset;
    [SerializeField] private bool automaticallySnapToOcean = true;
    [SerializeField] private string id;

    public float pitchAndRollMultiplier = 0.5f;
    public int size;
    public int pivotBlockIndex;
    public string identifier => this.id;

    private int closestVertex = -1; // The first vertex of the closest water plane face

    private void Start()
    {
        if (this.automaticallySnapToOcean)
        {
            this.SetupSnap();
        }
    }

    public void SetupSnap()
    {
        if (this.ocean == null)
        {
            Debug.LogWarning($"No ocean found for ship {this.gameObject.name}, disabling script");
            this.enabled = false;
            return;
        }

        this.closestVertex = -1;
        float minDist = 0.0f;
        int[] triangles = this.ocean.sharedMesh.triangles;
        Vector3[] vertices = this.ocean.sharedMesh.vertices;

        for (int triangle = 0; triangle < triangles.Length; triangle += 6)
        {
            Vector3[] face = this.GetGridFace(triangles[triangle], vertices);
            Vector3 avgPos = this.GetGridFaceAverage(face);

            float sqrDist = (avgPos - this.transform.position).sqrMagnitude;
            if (this.closestVertex < 0 || sqrDist < minDist)
            {
                this.closestVertex = triangles[triangle];
                minDist = sqrDist;
            }
        }

        if (this.closestVertex < 0)
        {
            Debug.LogWarning($"No ocean vertex found for ship {this.gameObject.name}, disabling script");
            this.enabled = false;
        }
    }

    public void Rotate(float offset)
    {
        this.rotationOffset.Rotate(Vector3.up * offset);
    }

    public void SetRotation(float angle)
    {
        this.rotationOffset.rotation = Quaternion.Euler(Vector3.up * angle);
    }

    public float GetRotationDeg()
    {
        return this.rotationOffset.rotation.eulerAngles.y;
    }

    public float GetRotationRad()
    {
        return Mathf.Deg2Rad * this.GetRotationDeg();
    }

    private Vector3[] GetGridFace(int firstIdx, Vector3[] data)
    {
        Vector3[] face = new Vector3[4];
        int[] indices = { 0, 1, 2, 4 };

        for (int i = 0; i < indices.Length; ++i)
        {
            int idx = firstIdx + indices[i];
            face[i] = this.ocean.transform.TransformPoint(data[idx]);
        }

        return face;
    }

    private Vector3 GetGridFaceAverage(Vector3[] face)
    {
        Vector3 avg = Vector3.zero;
        foreach (Vector3 v in face)
        {
            avg += v;
        }
        return avg / face.Length;
    }

    private void Update()
    {
        if (this.closestVertex >= 0)
        {
            Vector3[] vertices = this.ocean.sharedMesh.vertices;
            Vector3 deltaPos = this.transform.position - this.oceanSnapPoint.position;
            Vector3[] face = this.GetGridFace(this.closestVertex, vertices);
            this.transform.position = this.GetGridFaceAverage(face) + (deltaPos.y * Vector3.up);

            Vector3[] normals = this.ocean.sharedMesh.normals;
            face = this.GetGridFace(this.closestVertex, normals);
            Vector3 normal = this.GetGridFaceAverage(face);
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
            this.transform.rotation = Quaternion.Slerp(Quaternion.identity, rot, this.pitchAndRollMultiplier);
        }
    }

    public Vector2Int[] GetBlockCoords()
    {
        Vector2Int[] coords = new Vector2Int[this.size];
        Vector2Int pivotCoords = WorldPosToGridCoords(this.transform.position);
        float rot = this.GetRotationRad();

        for (int i = 0; i < this.size; ++i)
        {
            int offset = i - this.pivotBlockIndex;
            coords[i] = new Vector2Int(
                pivotCoords.x + Mathf.RoundToInt(offset * Mathf.Cos(rot)),
                pivotCoords.y - Mathf.RoundToInt(offset * Mathf.Sin(rot))
            );
        }
        return coords;
    }

    public bool OverlapsWith(Ship other)
    {
        if (other == this)
        {
            return false;
        }

        Vector2Int[] myBlocks = this.GetBlockCoords();
        Vector2Int[] othersBlocks = other.GetBlockCoords();

        return myBlocks.Any(b => othersBlocks.Any(o => b == o));
    }

    public bool OverlapsWith(Vector2Int coords)
    {
        Vector2Int[] myBlocks = this.GetBlockCoords();
        return myBlocks.Any(b => b == coords);
    }

    public static Vector2Int WorldPosToGridCoords(Vector3 worldPos)
    {
        Vector2 coords = new Vector2(worldPos.x, worldPos.z);
        coords += 100.0f * Vector2.one;
        coords /= 2.0f;
        coords -= 5.0f * Vector2.one;
        coords /= 10.0f;
        return new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
    }

    public static Vector3 GridCoordsToWorldPos(Vector2Int coords)
    {
        Vector3 pos = new Vector3(coords.x, 0.0f, coords.y);
        pos *= 10.0f;
        pos += 5.0f * Vector3.one;
        pos *= 2.0f;
        pos -= 100.0f * Vector3.one;
        return pos;
    }
}
