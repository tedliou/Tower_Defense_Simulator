using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public bool IsBuilded => GameManager.instance.build[gridIndex.x, gridIndex.y];
    public bool state = true;
    public Material redMat, greenMat;
    public Vector2Int gridIndex = new Vector2Int(0, 0);

    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        gridIndex = new Vector2Int(0, 0);
    }

    public void SetMode(bool state)
    {
        this.state = state;
        if (state)
        {
            _meshRenderer.material = greenMat;
        }
        else
        {
            _meshRenderer.material = redMat;
        }
    }

    public void Spawn(GameObject obj)
    {
        obj.transform.position = transform.position;
        StartCoroutine(BuildHouse(Instantiate(obj)));
        GameManager.instance.build[gridIndex.x, gridIndex.y] = true;
    }

    private IEnumerator BuildHouse(GameObject obj)
    {
        var scale = obj.transform.localScale;
        scale.y = 0;
        obj.transform.localScale = scale;
        var targetScaleY = Random.Range(1, 6);

        while(obj.transform.localScale.y < targetScaleY)
        {
            scale.y += .1f;
            obj.transform.localScale = scale;
            yield return new WaitForSecondsRealtime(.02f);
        }
    }

    public void SetGrid(int x, int y)
    {
        gridIndex = new Vector2Int(x, y);
    }
}
