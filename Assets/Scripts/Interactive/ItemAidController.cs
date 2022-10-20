using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAidController : MonoBehaviour
{
    public List<Vector3> respawnPoints;
    LevelController level;
    ItemController item;

    void Start()
    {
        level = LevelController.GetLevel();
        ActionsController.OnItemPickUp += OnPickUp;

        StartCoroutine(ShowAfterDelay());
        item = GetComponent<ItemController>();
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
            StartCoroutine(ShowAfterDelay());
        }
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(level.IsRabbitsCollection() ? 5f : 15f);
        item.Create(
            respawnPoints[Random.Range(0, respawnPoints.Count)],
            Quaternion.Euler(0, Random.Range(0, 360), 0)
        );
    }
}
