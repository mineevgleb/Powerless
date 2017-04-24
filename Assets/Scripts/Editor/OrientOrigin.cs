using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

public class OrientOrigin : MonoBehaviour {

    [MenuItem("TempTools/Orient %#o")]
    private static void Orient()
    {
        GameObject activeObject = Selection.activeGameObject;
        if (activeObject == null) return;
        Undo.RegisterCompleteObjectUndo(activeObject, "Orient Origin");
        var mesh = activeObject.GetComponent<MeshFilter>();
        if (mesh == null) return;
        var vertPositions = mesh.sharedMesh.vertices.
            Select(vert=>activeObject.transform.TransformPoint(vert)).
            ToArray();
        if (vertPositions.Length < 3) return;
        var vertTriangle = new[] {vertPositions.First(),
            vertPositions.Last(), vertPositions[vertPositions.Length / 2]};
        Vector3 newOriginPosition = vertPositions.Aggregate((aggr, el) => aggr + el);
        newOriginPosition /= vertPositions.Length;

        Vector3 up = Vector3.Cross(vertTriangle[0] - vertPositions[2], 
            vertTriangle[1] - vertPositions[2]).normalized;
        up *= -1;
        //if (Vector3.Dot(up, mesh.sharedMesh.normals[0]) < 0) up *= -1;
        Vector3 front = (vertTriangle[1] - newOriginPosition).normalized;
        Quaternion newOriginRotation = Quaternion.LookRotation(front, up);
        activeObject.transform.rotation = newOriginRotation;
        activeObject.transform.position = newOriginPosition;
        var newVertPositions = vertPositions.
            Select(vert => activeObject.transform.InverseTransformPoint(vert)).
            ToArray();
        mesh.sharedMesh.vertices = newVertPositions;
    }

    [MenuItem("TempTools/CreateHandle %#h")]
    private static void CreatePlanetHandle()
    {
        GameObject[] activeObjects = Selection.gameObjects;
        var m1 = activeObjects[0].GetComponentInChildren<MeshFilter>();
        var m2 = activeObjects[1].GetComponentInChildren<MeshFilter>();
        var vp1 = m1.sharedMesh.vertices.
            Select(vert => activeObjects[0].transform.TransformPoint(vert)).
            ToArray();
        var vp2 = m2.sharedMesh.vertices.
            Select(vert => activeObjects[1].transform.TransformPoint(vert)).
            ToArray();
        var twovert = (from v1 in vp1 from v2 in vp2 where 
                       (v1 - v2).magnitude <= 0.01 select v1).ToList();
        GameObject tmp = new GameObject(String.Format("World Handle - {0} - {1}", 
            activeObjects[0].name, activeObjects[1].name));
        tmp.AddComponent<PlanetHandle>();
        var face1 = activeObjects[0].GetComponent<PlanetFace>();
        var face2 = activeObjects[1].GetComponent<PlanetFace>();
        var handle = tmp.GetComponent<PlanetHandle>();
        handle.FirstFace = face1;
        handle.SecondFace = face2;
        Undo.RegisterCompleteObjectUndo(activeObjects[0], "Handle Created");
        Undo.RegisterCompleteObjectUndo(activeObjects[1], "Handle Created");
        if (face1.Handles == null) face1.Handles = new List<PlanetHandle>();
        if (face2.Handles == null) face2.Handles = new List<PlanetHandle>();
        face1.Handles.Add(handle);
        face2.Handles.Add(handle);
        tmp.transform.position = (twovert[0] + twovert[1]) / 2;
        Undo.RegisterCreatedObjectUndo(tmp, "Handle created");
    }

    [MenuItem("TempTools/OrientHandle")]
    private static void OrientHandle()
    {
        GameObject activeObject = Selection.activeGameObject;
        PlanetHandle handle = activeObject.GetComponent<PlanetHandle>();
        var m1 = handle.FirstFace.GetComponentInChildren<MeshFilter>();
        var m2 = handle.SecondFace.GetComponentInChildren<MeshFilter>();
        var vp1 = m1.sharedMesh.vertices.
            Select(vert => m1.gameObject.transform.TransformPoint(vert)).
            ToArray();
        var vp2 = m2.sharedMesh.vertices.
            Select(vert => m2.gameObject.transform.TransformPoint(vert)).
            ToArray();
        var twovert = (from v1 in vp1
                       from v2 in vp2
                       where (v1 - v2).magnitude <= 0.01
                       select v1).ToList();
        Vector3 front = (twovert[0] - twovert[1]).normalized;
        handle.transform.rotation = Quaternion.LookRotation(front, Vector3.up);
    }
}
