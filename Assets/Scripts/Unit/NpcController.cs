using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{

    public UnitController unit;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<UnitController>();
    }
}
