using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetFace : MonoBehaviour
{
    public List<PlanetHandle> Handles;
    public GameObject EnergyBtnPrefab;

    private Vector3[] _corners;
    private readonly float epsilon = 0.01f;
    private GameObject energyBtn;

    public void Start()
    {
        var cornersUnsorted = Resources.Load<Mesh>("Models/face").vertices;
        _corners = new[]
        {
            cornersUnsorted[0],
            cornersUnsorted[1],
            cornersUnsorted[2],
            cornersUnsorted[4],
            cornersUnsorted[3]
        };
        var transformedCorners =
            _corners.Select(c => transform.TransformPoint(c)).ToArray();
        var sortedHandles = new List<PlanetHandle>();
        sortedHandles.Add(GetClosestHandle((transformedCorners[0] + 
            transformedCorners[1]) / 2));
        sortedHandles.Add(GetClosestHandle((transformedCorners[1] + 
            transformedCorners[2]) / 2));
        sortedHandles.Add(GetClosestHandle((transformedCorners[2] + 
            transformedCorners[3]) / 2));
        sortedHandles.Add(GetClosestHandle((transformedCorners[3] + 
            transformedCorners[4]) / 2));
        sortedHandles.Add(GetClosestHandle((transformedCorners[4] + 
            transformedCorners[0]) / 2));
        Handles = sortedHandles;
        energyBtn = Instantiate(EnergyBtnPrefab, transform);
        energyBtn.transform.localPosition = new Vector3(0, 0.0003f, 0);
        energyBtn.SetActive(true);
    }

    public bool IsPositionOnFace(Vector3 position)
    {
        if (Vector3.Dot(transform.up, Vector3.up) < 0)
            return false;
        var transformedCorners = 
            _corners.Select(c => transform.TransformPoint(c)).ToArray();
        float faceArea = GetPolygoneArea(transformedCorners);
        float faceWithPlayerArea = 
            GetMultiTriangleArea(position, transformedCorners);
        return Mathf.Abs(faceArea - faceWithPlayerArea) < epsilon;
    }

    public PlanetHandle GetClosestHandle(Vector3 position)
    {
        float minDist = float.MaxValue;
        PlanetHandle closestHandle = Handles[0];
        foreach (PlanetHandle handle in Handles)
        {
            float dist = Vector3.Distance(position, handle.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestHandle = handle;
            }
        }
        return closestHandle;
    }

    public PlanetHandle GetNextHandle(PlanetHandle handle)
    {
        int handleIdx = GetHandleIndex(handle);
        return handleIdx == -1 ? 
            null : 
            Handles[(handleIdx + 1) % Handles.Count];
    }

    public PlanetHandle GetPreviousHandle(PlanetHandle handle)
    {
        int handleIdx = GetHandleIndex(handle);
        return handleIdx == -1 ?
            null :
            Handles[(handleIdx - 1 + Handles.Count) % Handles.Count];
    }

    private int GetHandleIndex(PlanetHandle handle)
    {
        int handleIdx = -1;
        for (int i = 0; i < Handles.Count; ++i)
        {
            if (Handles[i] == handle)
            {
                handleIdx = i;
                break;
            }
        }
        return handleIdx;
    }

    private float GetTriangleArea(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        p1.y = p2.y = p3.y = 0;
        return Vector3.Cross(p1 - p3, p1 - p2).magnitude * 0.5f;
    }

    private float GetMultiTriangleArea(Vector3 basePt, Vector3[] othersPt)
    {
        float result = 0;
        for (int i = 0; i < othersPt.Length - 1; ++i)
        {
            result += GetTriangleArea(basePt, othersPt[i], othersPt[i + 1]);
        }
        result += GetTriangleArea(basePt, othersPt.First(), othersPt.Last());
        return result;
    }

    private float GetPolygoneArea(Vector3[] points)
    {
        float result = 0;
        for (int i = 1; i < points.Length - 1; ++i)
        {
            result += GetTriangleArea(points[0], points[i], points[i + 1]);
        }
        return result;
    }
}
