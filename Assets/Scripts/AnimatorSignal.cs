using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSignal : MonoBehaviour
{
    [SerializeField] Player pl;
    
    void HoldWeapon()
    {
        pl.HoldWeapon();
    }
}
