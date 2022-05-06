using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Cell : SerializedMonoBehaviour
{
    [Header("Stats")]
    public CellState state;
    public CellIndex index;

    [Header("Buildings")]
    public List<BuildingData> buildings = new List<BuildingData>();
    public bool CanBuild => buildings.Count == 0;

    [Header("Prefabs")]
    public Building buildingPrefab;

    [Header("Colors")]
    public Color emptyColor = Color.clear;
    public Color hoverColor = Color.white;
    public Color blockColor = Color.red;

    [Header("Renderers")]
    public MeshRenderer meshRenderer;

    private readonly string _inlineColor = "_InlineColor";

    private void Start()
    {
        emptyColor = meshRenderer.material.GetColor(_inlineColor);
    }

    public void SetStats(CellState state)
    {
        if (!CanBuild && state == CellState.Select)
        {
            state = CellState.Block;
        }
        this.state = state;
    }

    public void UpdateStats()
    {
        if (state == CellState.Empty)
        {
            meshRenderer.material.SetColor(_inlineColor, emptyColor);
        }
        else if (state == CellState.Select)
        {
            meshRenderer.material.SetColor(_inlineColor, hoverColor);
        }
        else
        {
            meshRenderer.material.SetColor(_inlineColor, blockColor);
        }
    }

    public void Build()
    {
        buildings.Add(Create().data);
    }

    private Building Create()
    {
        var building = Instantiate(buildingPrefab, GridManager.instance.buildingParant);
        building.transform.position = transform.position;
        building.data.x = index.x;
        building.data.y = index.y;
        return building;
    }

    public void BuildWithData()
    {
        foreach(var _ in buildings)
        {
            Create();
        }
    }

    public void DestroyBuilding()
    {
        foreach(var b in buildings)
        {
            Building.DestroyBuilding(b);
        }

        buildings.Clear();
    }
}
