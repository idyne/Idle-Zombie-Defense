using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshesOnStart : MonoBehaviour
{
    private void Start()
    {
        MeshCombiner meshCombiner = gameObject.AddComponent<MeshCombiner>();
        meshCombiner.CreateMultiMaterialMesh = false;
        meshCombiner.DestroyCombinedChildren = true;
        meshCombiner.CombineMeshes(false);
    }
}
