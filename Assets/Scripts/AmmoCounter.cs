using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetCounter(byte newAmmo)
    {//20/20

        text.text = $"{newAmmo}/20";
    }
}
