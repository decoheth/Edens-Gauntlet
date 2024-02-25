using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourcePopupTemplate : MonoBehaviour

{
    public TMP_Text typeText;
    public TMP_Text amountText;
    public Image image;
    public float lifeTime;

    void Start()
    {
        Debug.Log("Destroy instance");
        Destroy(this.gameObject, lifeTime);
    }

}

