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

    public static int scoreToWin = 15;

    // Configurable params
    public AudioSource gameOverSound;
    public Animator nextLevelTextAnimator;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI scoreHint1Text;
    public TextMeshProUGUI scoreHint2Text;

    int level = DEATHMATCH;
    int rabbitsCollected = 0;
    ActionsController actions;
    bool isRestarting = false;
    bool isAnimationShow = true;
    PlayerController winnerPlayer = null;

    readonly int rabbitsCollectToWin = 3;
    readonly List<UnitController> enemies = new() { };
    readonly List<PlayerController> players = new() { };

    void Start()
    {
        ActionsController.OnRoundStart += OnRoundStart;
        ActionsController.OnRoundRestart += OnRoundRestart;
        ActionsController.OnPlayerWon += DisableAnimation;
        ActionsController.OnStartGame += EnableAnimation;
        ActionsController.OnItemPickUp += OnItemPickUp;
        ActionsController.OnUnitKilled += OnUnitKilled;
        ActionsController.OnScoreUpdate += OnScoreUpdate;

        actions = ActionsController.GetActions();
        FillPlayersAndEnemies();
        UpdateScoreHint2Text();

        // Duplicated in LogoController
        Cursor.visible = false;
        Screen.fullScreen = true;
        // Input.simulateMouseWithTouches = true;
    }

    void OnDestroy()
    {
        ActionsController.OnRoundStart -= OnRoundStart;
        ActionsController.OnRoundRestart -= OnRoundRestart;
        ActionsController.OnPlayerWon -= DisableAnimation;
        ActionsController.OnStartGame -= EnableAnimation;
        ActionsController.OnItemPickUp -= OnItemPickUp;
        ActionsController.OnUnitKilled -= OnUnitKilled;
        ActionsController.OnScoreUpdate -= OnScoreUpdate;
    }

    void FillPlayersAndEnemies()
    {
        foreach (var unit in GameObject.FindObjectsOfType<UnitController>())
        {
            if (unit.team == "enemy")
            {
                enemies.Add(unit);
            }
            else
            {
                var player = unit.GetComponent<PlayerController>();
                if (player != null)
                {
                    players.Add(player);
                }
            }
        }
    }

    void UpdateScoreHint2Text()
    {
        scoreHint2Text.text = $"Reach <u><b>{scoreToWin}</b></u> score points to win the tournament";
    }

    void DisableAnimation(PlayerController _player)
    {
        isAnimationShow = false;
    }

    void EnableAnimation()
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

    void OnRoundRestart()
    {
        level = LEVELS[Random.Range(0, LEVELS.Count)];
        rabbitsCollected = 0;
        winnerPlayer = null;

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
            scoreHint1Text.text = nextLevelText.text;
            StartCoroutine(StartAnimation());
        }
    }
    
    void OnItemPickUp(UnitController unit, ItemController item)
    {
        if (level == COLLECT_RABBITS)
        {
            rabbitsCollected++;

            var player = unit.GetComponent<PlayerController>();

            if (player != null)
            {
                var playerName = $"<color={PlayerWonController.playerColors[player.playerId]}>P{player.playerId + 1}</color>";
                var rabbitsLeft = rabbitsCollectToWin - rabbitsCollected;

                nextLevelText.text = $"{playerName} collected a rabbit! <u>{rabbitsLeft}</u> left!";
                scoreHint1Text.text = nextLevelText.text;
                
                Show();
                player.AddScore(1);
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
        nextLevelTextAnimator.Play("Show");
    }

    void OnRoundStart()
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

    void OnUnitKilled(UnitController dead, UnitController killer)
    {
        var killerPlayer = killer.GetComponent<PlayerController>();

        if (killerPlayer == null)
        {
            // Killer is not a player
            if (IsRoundEnded())
            {
                // Restart if boss killed all players
                RestartRound();
            }
        } else
        {
            // Killer is a player
            var scorePoints = dead.team == "enemy" ? 3 : 1;
            killerPlayer.AddScore(scorePoints);
        }
    }

    void OnScoreUpdate()
    {
        if (winnerPlayer != null)
        {
            return;
        }

        // Check current winner
        winnerPlayer = GetWinnerPlayer();

        if (winnerPlayer != null)
        {
            actions.EndRound();
            actions.PlayerWon(winnerPlayer);
            gameOverSound.Play();
        }
        else if(IsRoundEnded())
        {
            // Restart if no winner and players dead
            RestartRound();
        }
    }

    void RestartRound()
    {
        if (isRestarting)
        {
            return;
        }

        isRestarting = true;
        StartCoroutine(RestartAfterDelay());
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        actions.RestartRound();
        isRestarting = false;
    }

    PlayerController GetWinnerPlayer()
    {
        foreach (var player in players)
        {
            if (player.score >= scoreToWin)
            {
                return player;
            }
        }

        return null;
    }

    bool IsRoundEnded()
    {
        if (IsRabbitsCollection())
        {
            // Check if Rabbits level completed
            if (rabbitsCollected >= rabbitsCollectToWin)
            {
                return true;
            }
        }
        else
        {
            // Check if all players dead
            var alivePlayersCount = 0;
            var aliveControlledPlayersCount = 0;

            foreach (var player in players)
            {
                if (player.GetUnit().IsAlive())
                {
                    alivePlayersCount++;

                    if (player.GetUnit().GetDevice().IsSelected())
                    {
                        aliveControlledPlayersCount++;
                    }
                }
            }

            if (alivePlayersCount <= (IsBoss() ? 0 : 1) || aliveControlledPlayersCount == 0)
            {
                return true;
            }
        }

        if (IsBoss())
        {
            // Check if Boss dead
            var aliveEnemiesCount = 0;

            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive())
                {
                    aliveEnemiesCount++;
                }
            }

            if (aliveEnemiesCount == 0)
            {
                return true;
            }
        }

        return false;
    }
}
