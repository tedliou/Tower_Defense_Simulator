using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public TowerData[] towers;

#if UNITY_EDITOR
    private void OnValidate()
    {
        towers =
            AssetDatabase.FindAssets($"t:{nameof(TowerData)}")
                         .Select(g => AssetDatabase.LoadAssetAtPath<TowerData>(AssetDatabase.GUIDToAssetPath(g)))
                         .ToArray();
    }
#endif
}
