using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Stats")]
    public CellState state;

    [Header("Buildings")]
    public List<Building> buildings = new List<Building>();
    public bool CanBuild => buildings.Count == 0;

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
        if (!CanBuild && state == CellState.Select) state = CellState.Block;
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

    public void Build(Building building)
    {
        buildings.Add(building);
    }
}
