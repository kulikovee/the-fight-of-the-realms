using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    static readonly int DEATHMATCH = 0;
    static readonly int FIGHT_BOSS = 1;
    static readonly int COLLECT_RABBITS = 2;
    static readonly int SURVIVE = 3;
    static readonly List<int> LEVELS = new() { DEATHMATCH, FIGHT_BOSS, COLLECT_RABBITS, SURVIVE };

    public static int scoreToWin = 15;

    // Configurable params
    public AudioSource gameOverSound;
    public Animator nextLevelTextAnimator;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI scoreHint1Text;
    public TextMeshProUGUI scoreHint2Text;
    public GameObject bossPrefab;

    int level = DEATHMATCH;
    int rabbitsCollected = 0;
    ActionsController actions;
    bool isRestarting = false;
    bool isAnimationShow = true;
    PlayerController winnerPlayer = null;

    readonly int rabbitsCollectToWin = 3;

    void Start()
    {
        ActionsController.OnRoundStart += OnRoundStart;
        ActionsController.OnRoundEnd += OnRoundEnd;
        ActionsController.OnRoundRestart += OnRoundRestart;
        ActionsController.OnPlayerWon += DisableAnimation;
        ActionsController.OnStartGame += EnableAnimation;
        ActionsController.OnItemPickUp += OnItemPickUp;
        ActionsController.OnUnitKilled += OnUnitKilled;
        ActionsController.OnScoreUpdate += OnScoreUpdate;

        actions = ActionsController.GetActions();
        UpdateScoreHint2Text();

        // Duplicated in LogoController
        Cursor.visible = false;
    }

    void OnDestroy()
    {
        ActionsController.OnRoundStart -= OnRoundStart;
        ActionsController.OnRoundEnd -= OnRoundEnd;
        ActionsController.OnRoundRestart -= OnRoundRestart;
        ActionsController.OnPlayerWon -= DisableAnimation;
        ActionsController.OnStartGame -= EnableAnimation;
        ActionsController.OnItemPickUp -= OnItemPickUp;
        ActionsController.OnUnitKilled -= OnUnitKilled;
        ActionsController.OnScoreUpdate -= OnScoreUpdate;
    }

    PlayerController[] GetPlayers()
    {
        return GameObject.FindObjectsOfType<PlayerController>();
    }

    NpcController[] GetEnemies()
    {
        return GameObject.FindObjectsOfType<NpcController>();
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

    public bool IsSurvival()
    {
        return level == SURVIVE;
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

            if (IsSurvival())
            {
                levelTip = "SURVIVE!";
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

    void OnRoundEnd()
    {
        foreach (var enemy in GetEnemies())
        {
            Destroy(enemy.gameObject);
        }
    }

    void OnRoundStart()
    {
        foreach(var player in GetPlayers())
        {
            player.GetUnit().team = level == DEATHMATCH ? "" : "allies";
        }

        if (IsBoss())
        {
            CreateBoss(new Vector3(0, 10, 0), 0.9f);
        }

        if (IsSurvival())
        {
            CreateBoss(new Vector3(1, 10, 1), 1.1f);
            CreateBoss(new Vector3(-1, 10, 1), 1.2f);
            CreateBoss(new Vector3(1, 10, -1), 1.3f);
            CreateBoss(new Vector3(-1, 10, -1), 1.4f);
        }
    }

    void CreateBoss(Vector3 position, float speed)
    {
        var boss = Instantiate(bossPrefab, position, Quaternion.identity);
        boss.GetComponent<DeviceController>().SetFrozen(false);
        boss.GetComponent<UnitController>().speed = speed;
    }

    void OnUnitKilled(UnitController dead, UnitController killer)
    {
        var killerPlayer = killer.GetComponent<PlayerController>();

        if (killerPlayer == null)
        {
            // Killer is not a player
            if (IsRoundEnded())
            {
                var alivePlayers = GetAlivePlayers();

                if (IsSurvival() && alivePlayers.Count == 1)
                {
                    // OnScoreUpdate will restart the level
                    alivePlayers.ForEach(player => player.AddScore(3));
                } else
                {
                    // Restart if boss killed all players
                    RestartRound();
                }
            }
        } else
        {
            // Killer is a player
            var scorePoints = dead.team == "enemy" ? 3 : 1;
            killerPlayer.AddScore(scorePoints);
        }
    }

    List<PlayerController> GetAlivePlayers()
    {
        var alivePlayers = new List<PlayerController>() { };
        foreach (var player in GetPlayers())
        {
            if (player.GetUnit().IsAlive())
            {
                alivePlayers.Add(player);
            }
        }
        return alivePlayers;
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
        foreach (var player in GetPlayers())
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
            List<PlayerController> alivePlayers = new() { };
            var aliveControlledPlayersCount = 0;

            foreach (var player in GetPlayers())
            {
                if (player.GetUnit().IsAlive())
                {
                    alivePlayers.Add(player);

                    if (player.GetUnit().GetDevice().IsSelected())
                    {
                        aliveControlledPlayersCount++;
                    }
                }
            }

            if (alivePlayers.Count <= (IsBoss() ? 0 : 1) || aliveControlledPlayersCount == 0)
            {
                return true;
            }
        }

        if (IsBoss())
        {
            // Check if Boss dead
            var aliveEnemiesCount = 0;

            foreach (var enemy in GetEnemies())
            {
                if (enemy.unit.IsAlive())
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
