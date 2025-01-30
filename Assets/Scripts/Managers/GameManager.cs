using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool _isGamePaused;
    public bool IsGamePaused
    {
        get
        {
            return _isGamePaused;
        }
        set
        {
            _isGamePaused = value;
            IsPlayerInputEnabled = value;
            IsCursorVisible = !value;

            if (_isGamePaused)
                ScreenManager.Instance.ShowPopup("PauseMenuPopup", hasOpeningAnimation: true);
            else
                ScreenManager.Instance.HidePopup("PauseMenuPopup", hasClosingAnimation: true);

            OnPauseStateChanged?.Invoke(_isGamePaused);
        }
    }
    public static Action<bool> OnPauseStateChanged;
    public void TogglePause() => IsGamePaused = !IsGamePaused;

    private bool _isPlayerInputEnabled;
    public bool IsPlayerInputEnabled
    {
        get
        {
            return _isPlayerInputEnabled;
        }
        set
        {
            _isPlayerInputEnabled = value;
            OnPlayerInputStateChanged?.Invoke(_isPlayerInputEnabled);
        }
    }
    public static Action<bool> OnPlayerInputStateChanged;
    public void TogglePlayerInput() => IsPlayerInputEnabled = !IsPlayerInputEnabled;

    public bool IsCursorVisible
    {
        get
        {
            return Cursor.lockState != CursorLockMode.Locked;
        }
        set
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
            }
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        {
            Destroy(gameObject);
        }
    }

}
