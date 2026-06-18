using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyLock : MonoBehaviour
{
    [SerializeField] Dummy dummy;
    [SerializeField] Dummy dummy2;
    [SerializeField] Door door;
    void Start()
    {
        
    }

    void Update()
    {
        if (dummy.countTime && dummy2.countTime == true)
        {
            door.UnlockAndOpen();
        }
    }
}
