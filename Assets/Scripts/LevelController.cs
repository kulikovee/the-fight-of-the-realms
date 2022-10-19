using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    static readonly int DEATHMATCH = 0;
    static readonly int FIGHT_BOSS = 1;
    static readonly int COLLECT_RABBITS = 2;
    static readonly List<int> LEVELS = new() { DEATHMATCH, FIGHT_BOSS, COLLECT_RABBITS };

    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI scoreLevelText;
    public Animator animator;
    public int rabbitsCollected = 0;
    public readonly int rabbitsCollectToWin = 3;

    ActionsContoller actions;
    int level = DEATHMATCH;
    bool isAnimationShow = true;
    readonly List<UnitController> enemies = new() { };
    readonly List<PlayerController> players = new() { };

    void Start()
    {
        ActionsContoller.OnRoundStart += StartNextLevel;
        ActionsContoller.OnRoundRestart += ResetLevel;
        ActionsContoller.OnPlayerWon += DisableAnimation;
        ActionsContoller.OnStartGame += EnableAnimation;
        ActionsContoller.OnItemPickUp += OnItemPickUp;

        actions = ActionsContoller.GetActions();

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
        ActionsContoller.OnRoundRestart -= ResetLevel;
        ActionsContoller.OnPlayerWon -= DisableAnimation;
        ActionsContoller.OnStartGame -= EnableAnimation;
        ActionsContoller.OnItemPickUp -= OnItemPickUp;
    }

    private void DisableAnimation(PlayerController _player)
    {
        isAnimationShow = false;
    }

    private void EnableAnimation()
    {
        isAnimationShow = true;
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

    public bool IsRabbitsCollection()
    {
        return level == COLLECT_RABBITS;
    }

    void ResetLevel()
    {
        level = LEVELS[Random.Range(0, LEVELS.Count)];
        rabbitsCollected = 0;

        if (isAnimationShow)
        {
            var levelTip = "";

            if (IsDeathmatch())
            {
                levelTip = "DEATH MATCH!";
            }

            if (IsBoss())
            {
                levelTip = "FIGHT THE BOSS!";
            }

            if (IsRabbitsCollection())
            {
                levelTip = "COLLECT RABBITS!";
            }

            nextLevelText.text = levelTip;
            scoreLevelText.text = nextLevelText.text;
            StartCoroutine(StartAnimation());
        }
    }
    
    void OnItemPickUp(UnitController unit, AidKitController item)
    {
        if (level == COLLECT_RABBITS)
        {
            rabbitsCollected++;

            var player = unit.GetComponent<PlayerController>();

            if (player != null)
            {
                var playerName = $"<color={PlayerWonController.playerColors[player.playerId]}>P{player.playerId + 1}</color>";
                nextLevelText.text = $"{playerName} collected a rabbit! <u>{rabbitsCollectToWin - rabbitsCollected}</u> left!";
                scoreLevelText.text = nextLevelText.text;
                Show();
                player.UpdateScore(1);
            }
        }
    }

    IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        Show();
    }

    void Show()
    {
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

        if (level == FIGHT_BOSS || level == COLLECT_RABBITS)
        {
            foreach (var player in players)
            {
                player.GetUnit().team = "players";
            }
        }

        if (level == FIGHT_BOSS)
        {
            foreach(var enemy in enemies)
            {
                enemy.SetPosition(new Vector3(0, 10, 0));
            }
        }
    }
}
