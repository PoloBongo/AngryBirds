using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FarObj : MonoBehaviour
{
    [MenuItem("Tools/FindFarObjects")]
    public static void FindFarObjects() {
        List<GameObject> farObjs = new List<GameObject>();
        var allObjs = GameObject.FindObjectsOfType<GameObject>();
        for (var i = 0; i < allObjs.Length; i++) {
            if ((Mathf.Abs(allObjs[i].transform.position.x) > 1000) ||
                (Mathf.Abs(allObjs[i].transform.position.y) > 500) ||
                (Mathf.Abs(allObjs[i].transform.position.z) > 1000)
               ){
                farObjs.Add(allObjs[i]);
            }
        }

        if (farObjs.Count > 0) {
            for (var i = 0; i < farObjs.Count; i++) {
                Debug.LogError($"Found object {farObjs[i].name} at location {farObjs[i].transform.position}");
            }
        } else {
            Debug.Log("No Far objects");
        }
    }
}
