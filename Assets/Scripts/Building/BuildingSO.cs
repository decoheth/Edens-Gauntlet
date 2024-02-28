using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName="Buildable", menuName="Scriptable Objects/New Buildable")]
public class BuildingSO : ScriptableObject
{
    public string title;
    public bool isUnlocked = false;
    public GameObject prefab;
    public SelectedBuildType buildType;
    public float woodCost;
    public float metalCost;
    public float stoneCost;
    public float seedCost;
    public Sprite image;
    public string description;


}



