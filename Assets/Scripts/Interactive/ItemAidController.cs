using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAidController : MonoBehaviour
{
    void Start()
    {
        ActionsController.OnItemPickUp += OnPickUp;
    }

    void OnDestroy()
    {
        ActionsController.OnItemPickUp -= OnPickUp;
    }

    void OnPickUp(UnitController unit, ItemController pickedUpItem)
    {
        if (pickedUpItem.GetComponent<ItemAidController>() == this)
        {
            unit.AddHp(50);
        }
    }
}
