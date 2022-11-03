using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{

    public UnitController unit;

    // This class needed only as a marker of non-playable character
    void Start()
    {
        unit = GetComponent<UnitController>();
    }
}
