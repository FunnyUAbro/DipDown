using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public enum AmmoType
{
    None,
    Pistol,
    AssaultRifle,
    Shotgun,
    SniperRifle
}

public class Rounds : MonoBehaviour
{
    public int amount;
    public AmmoType type;


    public int TryGetAmmo(int getAmount)
    {
        int returningAmount;
        if (amount <= getAmount)
        {
            returningAmount = amount;
            EmptyBox();

            return returningAmount;
        }
        else
        {
            returningAmount = getAmount;
            amount -= getAmount;

            return returningAmount;
        }
    }

    void EmptyBox()
    {
        amount = 0;
        
        Destroy(gameObject);
    }

}
