using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OreChance
{
    public OreType ore;
    [Range(0f, 1f)] public float chance;
}

[CreateAssetMenu(menuName = "WorldMiner/Region Ore Table")]
public class RegionOreTable : ScriptableObject
{
    public string regionId; // "NA", "ME"
    public List<OreChance> chances = new();
}
