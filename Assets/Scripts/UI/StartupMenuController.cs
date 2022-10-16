using System.Collections;
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
    public bool isVisible = false;
    private Animator animator;

    // links
    private ActionsContoller actions;

    // timer
    private int secondsToStartGame = 16;
    private float startTimerAt;

    void Start()
    {
        ActionsContoller.OnFirstShowStartupMenu += FirstShowStartupMenu;
        ActionsContoller.OnShowStartupMenu += ShowStartupMenu;
        ActionsContoller.OnEndGame += ShowAfterDelay;

        actions = ActionsContoller.GetActions();
        animator = GetComponent<Animator>();

        Cursor.visible = false;
    }

    void OnDestroy()
    {
        ActionsContoller.OnFirstShowStartupMenu -= FirstShowStartupMenu;
        ActionsContoller.OnShowStartupMenu -= ShowStartupMenu;
        ActionsContoller.OnEndGame -= ShowAfterDelay;
    }

    void Update()
    {
        if (isVisible)
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

    private void CheckSkipJoinTimeout()
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

    private void FirstShowStartupMenu()
    {
        SetVisible(true, true);
    }

    private void ShowStartupMenu()
    {
        SetVisible(true);
    }

    public void ResetPlayerText()
    {
        actions.ResetPlayersText();
    }

    public void SetVisible(bool visible, bool skipSounds = false)
    {
        if (isVisible != visible)
        {
            isVisible = visible;

            if (visible)
            {
                Time.timeScale = 0;
                startTimerAt = Time.unscaledTime;
                animator.Play("Startup Menu Show");

                if (!skipSounds)
                {
                    mainTheme.Stop();
                    menuTheme.Play();
                }
            }
            else
            {
                Time.timeScale = 1;
                menuTheme.Stop();
                mainTheme.Play();
                animator.Play("Startup Menu Hide");
            }
        }
    }
    void ShowAfterDelay()
    {
        StartCoroutine(_ShowAfterDelay());
    }
    
    IEnumerator _ShowAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        Show();
    }

    void Show()
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

    void StartGame()
    {
        actions.StartGame();
        actions.RoundRestart();
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
            actions.TimerUpdate(timeLeft);
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
        } else if (InputNumpadController.IsPressed())
        {
            deviceId = DeviceController.KEYBOARD_NUMPAD;
        } else
        {
            var pressedGamepadIds = InputGamepadController.GetPressedIds();

            if (pressedGamepadIds.Count > 0)
            {
                pressedGamepadIds.ForEach((gamePadId) => {
                    if (!IsDeviceSelected(gamePadId))
                    {
                        deviceId = gamePadId;
                    }
                });
            }
        }

        if (deviceId != DeviceController.NO_DEVICE && !IsDeviceSelected(deviceId) && !IsAllDevicesSelected())
        {
            joinSound.Play();
            AddSecondsToTimer();
            SetNextPlayerDevice(deviceId);
        }
    }

    private void SetNextPlayerDevice(int deviceId)
    {
        PlayerController player = null;
        var players = GetPlayers();

        foreach (var _player in players)
        {
            if (_player.GetUnit().GetDevice().IsPrevious(deviceId) && !_player.GetUnit().GetDevice().IsSelected())
            {
                player = _player;
                break;
            }
        }

        if (player != null)
        {
            player.GetUnit().GetDevice().SetId(deviceId);
            actions.JoinedPlayersText();
        } else
        {
            foreach (var _player in players)
            {
                if (!_player.GetUnit().GetDevice().IsSelected())
                {
                    _player.GetUnit().GetDevice().SetId(deviceId);
                    actions.JoinedPlayersText();
                    break;
                }
            }
        }
    }

    private void AddSecondsToTimer(float seconds = 5f)
    {
        if (GetSecondsToStart() < seconds)
        {
            startTimerAt = Time.unscaledTime - secondsToStartGame + seconds + 1;
        }
    }
}
