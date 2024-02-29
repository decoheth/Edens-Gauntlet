using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NoteFragment", menuName="Scriptable Objects/New Note")]
public class NoteSO : ScriptableObject
{
    public int index;
    public string title;
    public bool isUnlocked;
    public string  description;
}
