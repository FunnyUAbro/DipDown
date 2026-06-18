using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndReloader : MonoBehaviour
{
    [SerializeField] Player player;

    public void EndReload()
    {
        player.EndReload();
    }

}
