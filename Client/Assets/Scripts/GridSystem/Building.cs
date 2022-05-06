using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Building : MonoBehaviour
{
    public BuildingData data = new BuildingData();
    public Transform body;

    private void Awake()
    {
        data.building = this;
    }

    private void Start()
    {
        StartCoroutine(Build());
    }

    private IEnumerator Build()
    {
        var scale = body.localScale;
        scale.y = 0;
        body.localScale = scale;
        var targetScaleY = Random.Range(1, 6);

        while (body.localScale.y < targetScaleY)
        {
            scale.y += .1f;
            body.localScale = scale;
            yield return null;
        }
    }

    public static void DestroyBuilding(BuildingData data)
    {
        var buildings = FindObjectsOfType<Building>().ToList();
        var obj = buildings.Single(b => b.data.x == data.x && b.data.y == data.y).gameObject;
        Destroy(obj);
    }
}

[System.Serializable]
public struct BuildingData
{
    public int x;
    public int y;
    public int id;
    public int level;

    [System.NonSerialized]
    public Building building;
}