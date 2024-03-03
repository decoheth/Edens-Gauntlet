using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName="Buildable", menuName="Scriptable Objects/New Buildable")]
public class BuildingSO : ScriptableObject
{
    public string title;
    public bool isUnlocked = false;
    public GameObject prefab;
    public SelectedBuildType buildType;
    public int woodCost;
    public int metalCost;
    public int stoneCost;
    public int seedCost;
    public Sprite image;
    public string description;


}



