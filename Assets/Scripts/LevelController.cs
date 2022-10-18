using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    static readonly int DEATHMATCH = 0;
    static readonly int FIGHT_BOSS = 1;
    static List<int> LEVELS = new() { DEATHMATCH, FIGHT_BOSS };

    public TextMeshProUGUI nextLevelText;
    public Animator animator;

    int level = DEATHMATCH;
    List<UnitController> enemies = new() { };
    List<PlayerController> players = new() { };

    void Start()
    {
        ActionsContoller.OnRoundStart += StartNextLevel;
        ActionsContoller.OnRoundEnd += ResetLevel;

        foreach(var unit in GameObject.FindObjectsOfType<UnitController>())
        {
            if (unit.team == "enemy")
            {
                enemies.Add(unit);
            } else
            {
                var player = unit.GetComponent<PlayerController>();
                if (player != null)
                {
                    players.Add(player);
                }
            }
        }
    }

    void OnDestroy()
    {
        ActionsContoller.OnRoundStart -= StartNextLevel;
    }

    public static LevelController GetLevel()
    {
        return GameObject.FindObjectOfType<LevelController>();
    }

    public bool IsDeathmatch()
    {
        return level == DEATHMATCH;
    }

    public bool IsBoss()
    {
        return level == FIGHT_BOSS;
    }

    void ResetLevel()
    {
        level = LEVELS[Random.Range(0, LEVELS.Count)];

        nextLevelText.text = IsDeathmatch() ? "DEATH MATCH" : "FIGHT THE BOSS";
        StartCoroutine(StartAnimation());
    }

    IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        animator.Play("Show");
    }

    void StartNextLevel()
    {
        if (level == DEATHMATCH)
        {
            foreach(var player in players)
            {
                player.GetUnit().team = "";
            }
        } 

        if (level == FIGHT_BOSS)
        {
            foreach(var player in players)
            {
                player.GetUnit().team = "players";
            }

            foreach(var enemy in enemies)
            {
                enemy.SetPosition(new Vector3(0, 10, 0));
            }
        }
    }
}
