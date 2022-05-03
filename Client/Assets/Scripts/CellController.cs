using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public Material redMat, greenMat;

    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMode(bool state)
    {
        if (state)
        {
            _meshRenderer.material = greenMat;
        }
        else
        {
            _meshRenderer.material = redMat;
        }
    }
}
