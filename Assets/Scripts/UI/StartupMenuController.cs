using System.Collections.Generic;
using UnityEngine;

public class StartupMenuController : MonoBehaviour
{
    // configurable params
    public List<PlayerController> players;

    // sounds
    public AudioSource joinSound;
    public AudioSource showSound;
    public AudioSource hideSound;
    public AudioSource mainTheme;
    public AudioSource menuTheme;

    // animation
    Animator animator;

    // links
    ActionsController actions;

    // timer
    int secondsToStartGame = 16;
    float startTimerAt;

    void Start()
    {
        ActionsController.OnEndGame += ShowStartupMenu;

        actions = ActionsController.GetActions();
        animator = GetComponent<Animator>();
        SetVisible(true);
    }

    void OnDestroy()
    {
        ActionsController.OnEndGame -= ShowStartupMenu;
    }

    void Update()
    {
        if (animator.GetBool("visible"))
        {
            this.UpdateTimerText();
            this.CheckUserSelection();
            this.CheckSkipJoinTimeout();

            if (IsAllDevicesSelected())
            {
                TimerFinished();
            }
        }
    }

    public void ResetPlayerText()
    {
        actions.ResetJoinedPlayers();
    }

    public void SetVisible(bool visible)
    {
        if (animator.GetBool("visible") != visible)
        {
            animator.SetBool("visible", visible);

            if (visible)
            {
                Time.timeScale = 0;
                startTimerAt = Time.unscaledTime;
                mainTheme.Stop();
                menuTheme.Play();
            }
            else
            {
                Time.timeScale = 1;
                menuTheme.Stop();
                mainTheme.Play();
            }
        }
    }

    void ShowStartupMenu()
    {
        SetVisible(true);
    }

    public void PlayShowSound()
    {
        showSound.Play();
    }

    public void PlayHideSound()
    {
        hideSound.Play();
    }

    public List<PlayerController> GetPlayers()
    {
        return players;
    }

    void CheckSkipJoinTimeout()
    {
        var skipDeviceIds = new List<int>();
        if (InputWASDController.IsSkip() && IsDeviceSelected(DeviceController.KEYBOARD_WASD))
        {
            skipDeviceIds.Add(DeviceController.KEYBOARD_WASD);
        }

        InputGamepadController.GetSkipGamepadIds().ForEach((gamePadId) =>
        {
            if (IsDeviceSelected(gamePadId))
            {
                skipDeviceIds.Add(DeviceController.KEYBOARD_WASD);
            }
        });

        if (skipDeviceIds.Count > 0)
        {
            TimerFinished();
        }
    }

    void StartGame()
    {
        actions.StartGame();
        actions.RestartRound();
        SetVisible(false);
    }

    int GetSecondsToStart()
    {
        return (int)Mathf.Floor(secondsToStartGame - (Time.unscaledTime - startTimerAt));
    }

    void UpdateTimerText()
    {
        var timeLeft = GetSecondsToStart();

        if (timeLeft >= 0)
        {
            actions.UpdateTimer(timeLeft);
        }

        if (timeLeft <= 0)
        {
            this.TimerFinished();
        }
    }

    void TimerFinished()
    {
        if (IsAnyPlayersJoined())
        {
            StartGame();
        }
    }

    bool IsDeviceSelected(int deviceId)
    {
        var foundIndex = GetPlayers().FindIndex(_player => _player.GetUnit().GetDevice().IsEquals(deviceId));
        return foundIndex > -1;
    }

    bool IsAllDevicesSelected()
    {
        var players = GetPlayers();
        var selectedPlayers = players.FindAll(_player => _player.GetUnit().GetDevice().IsSelected());
        return selectedPlayers.Count == players.Count;
    }

    bool IsAnyPlayersJoined()
    {
        var players = GetPlayers();
        var foundIndex = players.FindIndex(_player => _player.GetUnit().GetDevice().IsSelected());
        return foundIndex > -1;
    }

    void CheckUserSelection()
    {
        var deviceId = DeviceController.NO_DEVICE;

        if (InputWASDController.IsPressed())
        {
            deviceId = DeviceController.KEYBOARD_WASD;
            Shake(deviceId);
        }
        
        if (InputNumpadController.IsPressed())
        {
            deviceId = DeviceController.KEYBOARD_NUMPAD;
            Shake(deviceId);
        }

        var pressedGamepadIds = InputGamepadController.GetPressedIds();

        if (pressedGamepadIds.Count > 0)
        {
            pressedGamepadIds.ForEach((gamePadId) => {
                if (IsDeviceSelected(gamePadId))
                {
                    Shake(gamePadId);
                } else
                {
                    deviceId = gamePadId;
                }
            });
        }

        if (deviceId != DeviceController.NO_DEVICE)
        {
            if (!IsDeviceSelected(deviceId) && !IsAllDevicesSelected())
            {
                joinSound.Play();
                AddSecondsToTimer();
                SetNextPlayerDevice(deviceId);
            }
        }
    }

    void Shake(int deviceId)
    {
        foreach (var player in GetPlayers())
        {
            if (player.GetUnit().GetDevice().IsEquals(deviceId))
            {
                player.ShakeJoin();
            }
        }
    }

    void SetNextPlayerDevice(int deviceId)
    {
        var players = GetPlayers();
        PlayerController previousDeviceOwner = GetPreviousDevicePlayer(deviceId);

        if (previousDeviceOwner != null)
        {
            JoinPlayer(previousDeviceOwner, deviceId);
        } else
        {
            foreach (var player in players)
            {
                if (!player.GetUnit().GetDevice().IsSelected())
                {
                    JoinPlayer(player, deviceId);
                    break;
                }
            }
        }
    }

    void JoinPlayer(PlayerController player, int deviceId)
    {
        player.GetUnit().GetDevice().SetId(deviceId);
        player.ResetShakeTimeout();
        player.Join();
        actions.PlayerJoined();
    }

    PlayerController GetPreviousDevicePlayer(int deviceId)
    {
        foreach (var _player in players)
        {
            if (_player.GetUnit().GetDevice().IsPrevious(deviceId) && !_player.GetUnit().GetDevice().IsSelected())
            {
                return _player;
            }
        }

        return null;
    }

    void AddSecondsToTimer(float seconds = 5f)
    {
        if (GetSecondsToStart() < seconds)
        {
            startTimerAt = Time.unscaledTime - secondsToStartGame + seconds + 1;
        }
    }
}
