using System.Collections;
using System.Linq;
using UnityEngine;
using System;

public abstract class Ability : MonoBehaviour
{

    public GameObject selfEffectPrefab;
    public GameObject targetEffectPrefab;
    public AudioSource effectSound;

    [System.NonSerialized]
    protected float manaRequired = 25f;

    [System.NonSerialized]
    protected UnitController unit;

    [System.NonSerialized]
    protected float castApplyDelay = 1f;

    [System.NonSerialized]
    protected string title = "NOT_SET";

    protected void Awake()
    {
        unit = GetComponent<UnitController>();
    }

    public void Cast()
    {
        CastApply();
    }

    abstract protected void CastApply();

    public bool IsEnoughMana()
    {
        return unit.GetMana() >= manaRequired;
    }

    public float GetManaRequired()
    {
        return manaRequired;
    }

    public string GetTitle()
    {
        return title;
    }

    protected Vector3 GetForwardAtDistance(float distance)
    {
        return transform.position + transform.forward * distance + transform.up;
    }

    protected GameObject CreateEffect(GameObject prefab, Vector3? atPosition)
    {
        return Instantiate(
            prefab,
            atPosition != null ? (Vector3)atPosition : (transform.position + transform.up + transform.forward * 0.25f),
            Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0)
        );
    }

    protected GameObject CreateEffect(GameObject prefab, GameObject parent)
    {
        var instantiatedPrefab = Instantiate(
            prefab,
            parent.transform.position + parent.transform.up + parent.transform.forward * 0.25f,
            Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0)
        );
        instantiatedPrefab.transform.parent = parent.transform;

        return instantiatedPrefab;
    }

    protected UnitController[] GetDeadAllies(float? radius = null, Vector3? atPosition = null)
    {
        return GetUnits(radius, atPosition)
            .Where(_unit => !_unit.IsAlive() && _unit.IsSameTeam(unit))
            .ToArray();
    }

    protected UnitController[] GetAliveEnemies(float? radius = null, Vector3? atPosition = null)
    {
        return GetAliveUnits(radius, atPosition).Where(_unit => unit != _unit && !_unit.IsSameTeam(unit)).ToArray();
    }

    protected UnitController[] GetAliveAllies(float? radius = null, Vector3? atPosition = null)
    {
        return GetAliveUnits(radius, atPosition).Where(_unit => _unit.IsSameTeam(unit)).ToArray();
    }

    protected UnitController[] GetAliveUnits(float? radius = null, Vector3? atPosition = null)
    {
        return GetUnits(radius, atPosition).Where(_unit => _unit.IsAlive()).ToArray();
    }

    protected UnitController[] GetUnits(float? radius = null, Vector3? atPosition = null)
    {
        return GetUnits((_unit) => {
            if (radius != null)
            {
                var castPosition = atPosition != null ? (Vector3)atPosition : transform.position;
                return Vector3.Distance(castPosition, _unit.transform.position) < radius;
            }

            return false;
        });
    }

    protected UnitController[] GetUnits(Func<UnitController, bool> comporator)
    {
        return GameObject.FindObjectsOfType<UnitController>()
            .Where(comporator)
            .ToArray();
    }
}
