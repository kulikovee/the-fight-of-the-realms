using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    static readonly string DEATHMATCH = "DEATHMATCH";
    static readonly string FIGHT_BOSS = "FIGHT_BOSS";
    static readonly string COLLECT_RABBITS = "COLLECT_RABBITS";
    static readonly string SURVIVE = "SURVIVE";
    static readonly string PLATFORMER = "PLATFORMER";
    static readonly List<string> LEVELS = new()
    {
        DEATHMATCH,
        COLLECT_RABBITS,
        FIGHT_BOSS,
        DEATHMATCH,
        SURVIVE,
        PLATFORMER,
    };

    public static int scoreToWin = 20;

    // Configurable params
    public AudioSource gameOverSound;
    public Animator nextLevelTextAnimator;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI scoreHint1Text;
    public TextMeshProUGUI scoreHint2Text;
    public GameObject bossPrefab;
    public GameObject bossPrefab1;
    public GameObject bossPrefab2;
    public GameObject rabbitPrefab;
    public GameObject platform;
    public GameObject arena;
    public List<Vector3> arenaRabbitRespawns;
    public Vector3 platformerRabbitRespawn = new Vector3(-7, 0.02f, 0);

    int levelId = 0;
    int rabbitsCollected = 0;
    ActionsController actions;
    bool isRestarting = false;
    bool isAnimationShow = true;
    bool isFirstLevelRun = true;
    PlayerController winnerPlayer = null;
    ItemController levelRabbit = null;

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
        StartCoroutine(CreateLevelRabbitAfterDelay());

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

    ItemController[] GetItems()
    {
        return GameObject.FindObjectsOfType<ItemController>();
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
        return LEVELS[levelId] == DEATHMATCH;
    }

    public bool IsBoss()
    {
        return LEVELS[levelId] == FIGHT_BOSS;
    }

    public bool IsRabbitsCollection()
    {
        return LEVELS[levelId] == COLLECT_RABBITS;
    }

    public bool IsSurvival()
    {
        return LEVELS[levelId] == SURVIVE;
    }

    public bool IsPlatformer()
    {
        return LEVELS[levelId] == PLATFORMER;
    }

    void OnRoundRestart()
    {
        if (isFirstLevelRun)
        {
            isFirstLevelRun = false;
        } else
        {
            levelId++;
        }

        if (levelId >= LEVELS.Count)
        {
            levelId = 0;
        }

        if (IsPlatformer())
        {
            foreach (var item in GetItems())
            {
                if (item == levelRabbit)
                {
                    item.transform.position = platformerRabbitRespawn;
                }
            }
        }

        rabbitsCollected = 0;
        winnerPlayer = null;

        platform.SetActive(IsPlatformer());
        arena.SetActive(!IsPlatformer());

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
                levelTip = "COLLECT CHICKEN!";
            }

            if (IsSurvival())
            {
                levelTip = "SURVIVE!";
            }

            if (IsPlatformer())
            {
                levelTip = "KILL THE ORCS!";
            }

            nextLevelText.text = levelTip;
            scoreHint1Text.text = nextLevelText.text;
            StartCoroutine(StartAnimation());
        }
    }
    
    void OnItemPickUp(UnitController unit, ItemController item)
    {
        if (IsRabbitsCollection())
        {
            rabbitsCollected++;

            var player = unit.GetComponent<PlayerController>();

            if (player != null)
            {
                var playerName = $"<color={PlayerWonController.playerColors[player.playerId]}>P{player.playerId + 1}</color>";
                var rabbitsLeft = rabbitsCollectToWin - rabbitsCollected;

                nextLevelText.text = $"{playerName} collected a chicken! <u>{rabbitsLeft}</u> left!";
                scoreHint1Text.text = nextLevelText.text;
                
                Show();
                player.AddScore(1);
            }
        }

        if (item == levelRabbit)
        {
            StartCoroutine(CreateLevelRabbitAfterDelay());
        }
    }

    IEnumerator CreateLevelRabbitAfterDelay()
    {
        yield return new WaitForSeconds(IsRabbitsCollection() ? 5f : 15f);

        var newLevelRabbitPosition = IsPlatformer()
            ? platformerRabbitRespawn
            : arenaRabbitRespawns[Random.Range(0, arenaRabbitRespawns.Count)];

        var newLevelRabbit = CreateRabbit(newLevelRabbitPosition);
        levelRabbit = newLevelRabbit.GetComponent<ItemController>();
    }

    GameObject CreateRabbit(Vector3 position)
    {
        return Instantiate(rabbitPrefab, position, Quaternion.Euler(0, Random.Range(0, 360), 0));
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

        foreach (var item in GetItems())
        {
            if (item != levelRabbit)
            {
                Destroy(item.gameObject);
            }
        }
    }

    void OnRoundStart()
    {
        foreach(var player in GetPlayers())
        {
            if (IsDeathmatch() || IsSurvival())
            {
                player.GetUnit().MakeTeamEmpty();
            } else
            {
                player.GetUnit().MakeTeamAlly();
            }
        }

        if (IsBoss())
        {
            CreateBoss(bossPrefab, new Vector3(0, 10, 0), 800f);
        }

        if (IsSurvival())
        {
            CreateBoss(bossPrefab1, new Vector3(1, 10, 1), 800f, 1.4f, 60f, 1.5f);
            CreateBoss(bossPrefab, new Vector3(-1, 10, 1), 800f, 1.4f, 60f, 1.5f);
            CreateBoss(bossPrefab, new Vector3(1, 10, -1), 800f, 1.4f, 60f, 1.5f);
            CreateBoss(bossPrefab1, new Vector3(-1, 10, -1), 800f, 1.4f, 60f, 1.5f);
        }

        if (IsPlatformer())
        {
            CreateBoss(bossPrefab, new Vector3(14, 1, -1f), 100f, 0.5f, 10f);
            CreateBoss(bossPrefab, new Vector3(14, 1, 1f), 100f, 0.5f, 10f);
            CreateBoss(bossPrefab1, new Vector3(15, 1, 0), 170f, 0.6f, 25f);

            CreateBoss(bossPrefab, new Vector3(29, 1, -1f), 120f, 0.5f, 15f);
            CreateBoss(bossPrefab, new Vector3(29, 1, 1f), 120f, 0.5f, 15f);
            CreateBoss(bossPrefab1, new Vector3(30, 1, 0), 250f, 0.6f, 35f);

            CreateBoss(bossPrefab, new Vector3(44, 1, -1f), 120f, 0.6f, 20f);
            CreateBoss(bossPrefab, new Vector3(44, 1, 0), 120f, 0.6f, 20f);
            CreateBoss(bossPrefab, new Vector3(44, 1, 1f), 130f, 0.6f, 20f);
            CreateBoss(bossPrefab1, new Vector3(45, 1, 1), 250f, 0.7f, 35f);

            CreateBoss(bossPrefab, new Vector3(59, 1, -1f), 175f, 0.6f, 20f);
            CreateBoss(bossPrefab, new Vector3(59, 1, 0), 175f, 0.6f, 20f);
            CreateBoss(bossPrefab, new Vector3(59, 1, 1f), 175f, 0.6f, 20f);
            CreateBoss(bossPrefab1, new Vector3(60, 1, -1), 350f, 0.7f, 35f);

            CreateBoss(bossPrefab2, new Vector3(75, 1, 0), 900f, 0.8f, 40f);
        }
    }

    void CreateBoss(GameObject prefab, Vector3 position, float hp = 400f, float speed = 0.9f, float attackPower = 45f, float attackRaidus = 0.8f)
    {
        var boss = Instantiate(prefab, position, Quaternion.Euler(0, Random.Range(0, 360), 0));
        boss.GetComponent<DeviceController>().SetFrozen(false);
        boss.transform.Find("Grunt").localScale = Vector3.one * (0.55f + (hp / 300f) / 10f);
        var unit = boss.GetComponent<UnitController>();
        unit.speed = speed;
        unit.attackPower = attackPower;
        unit.maxHp = hp;
        unit.attackRadius = attackRaidus;
        unit.MakeTeamEnemy();
    }

    void OnUnitKilled(UnitController dead, UnitController killer)
    {
        var killerPlayer = killer.GetComponent<PlayerController>();
        var isKilledHimself = killer == dead;

        if (killerPlayer == null || isKilledHimself)
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
            var scorePoints = dead.IsTeamEnemy() && IsBoss() ? 3 : 1;
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

            if (alivePlayers.Count <= (IsBoss() || IsPlatformer() ? 0 : 1) || aliveControlledPlayersCount == 0)
            {
                return true;
            }
        }

        if (IsBoss() || IsPlatformer())
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
