using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshReadableCheck : MonoBehaviour
{
    public bool meshReadWrite;

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        if(meshReadWrite)
        {
            if(mesh.isReadable == false)
            {
                var so = new SerializedObject(mesh);
                mesh.UploadMeshData(true);

                so.Update();
                var sp = so.FindProperty("m_IsReadable");
                sp.boolValue = true;
                so.ApplyModifiedProperties();
                
                //Debug.Log("Mesh is now readable");

                // Not saved if exit RunTime at the moment
            }
        }


        //print("NavMesh Readable: " + mesh.isReadable);
    }

}
