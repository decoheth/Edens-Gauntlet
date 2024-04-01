using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="Perk", menuName="Scriptable Objects/New Perk")]
public class PerkSO : ScriptableObject
{
    public string title;
    public int code;
    public PerkTier tier;
    public bool isActive = false;
    public bool isKnown = true;
    public List<PerkSO> requiredPerks;
    public string description;
    public Sprite image;
    public int function = 0;
    public float value1 = 0f;
    public float value2 = 0f;


}
