using InnominatumDigital.Base;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using DG.Tweening;

[Serializable]
public class PopupConfig
{
    public string PrefabName;
    public GameObject Prefab;
}

[Serializable]
public class SceneConfig
{
    public string SceneName;
    public GameObject Prefab;

}

public class ScreenManager : SingletonBase<ScreenManager>
{
    public string CurrentScene;
    public bool IsShowingMessage;


    [Header("Popups:")]
    public List<PopupConfig> PopupsConfigs;
    public List<KeyValuePair<string, GameObject>> OpenedPopups = new List<KeyValuePair<string, GameObject>>();


    [Header("Scenes:")]
    public List<SceneConfig> ScenesConfigs;


    [Header("Message Screen:")]
    [SerializeField] private GameObject _messageScreen;
    [SerializeField] private TMP_Text _messageText;

    [Header("Fade:")]
    [SerializeField] private Image _fade;

    private string _popupToOpen;

    private void Start()
    {
        // mixer.SetFloat("Master", PlayerPrefs.GetFloat("SoundVolume"));
        SceneManager.sceneLoaded += OnSceneLoaded;
       // ShowPopup("MainMenuPopup");
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
    }

    public GameObject ShowPopup(string name, bool openMultiple = false, bool hasOpeningAnimation = false, Action callingMethod = null)
    {
        if (OpenedPopups.FirstOrDefault(s => s.Key == name).Value != null && !openMultiple)
            return null;

        GameObject newPopup = Instantiate(PopupsConfigs.First(s => s.PrefabName == name).Prefab);

        OpenedPopups.Add(new KeyValuePair<string, GameObject>(name, newPopup));

        if (hasOpeningAnimation)
            if (newPopup.GetComponent<PopupAnimation>())
                newPopup.GetComponent<PopupAnimation>().PlayOpenAnimation();

        callingMethod?.Invoke();

        return newPopup;
    }

    public void HidePopup(string name, bool hasClosingAnimation = false, Action callingMethod = null)
    {
        if (OpenedPopups.FirstOrDefault(s => s.Key == name).Value == null)
            return;

        if (hasClosingAnimation)
        {
            if (OpenedPopups.FirstOrDefault(s => s.Key == name).Value.GetComponent<PopupAnimation>())
                OpenedPopups.FirstOrDefault(s => s.Key == name).Value.GetComponent<PopupAnimation>().PlayCloseAnimation();
        }
        else
        {
            Destroy(OpenedPopups.First(s => s.Key == name).Value);
        }

        callingMethod?.Invoke();

        OpenedPopups.Remove(OpenedPopups.First(s => s.Key == name));
    }

    public void ChangeScene(string name, string openOnChange = "")
    {
        ShowPopup("LoadingPopup");
        SceneManager.LoadSceneAsync(name);

        _popupToOpen = openOnChange;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CurrentScene = scene.name;
        HidePopup("LoadingPopup");
        if (!String.IsNullOrEmpty(_popupToOpen))
        {
            ShowPopup(_popupToOpen);
            _popupToOpen = "";
        }

    }

   
    public void ShowMessageText(string dialog)
    {
        IsShowingMessage = true;
        _messageScreen.SetActive(true);
        SetMessageText(dialog);
    }

    async void SetMessageText(string fullText)
    {
        SoundManager.Instance.PlaySFX("Type", Random.Range(0.8f, 1.3f));

        string currentText = "";
        for (int i = 0; i <= fullText.Length; i++)
        {
            if (!IsShowingMessage)
                return;

            currentText = fullText.Substring(0, i);
            _messageText.text = currentText;
            await UniTask.Delay(2);
        }

    }

    public void HideMessageText()
    {
        _messageScreen.SetActive(false);
        _messageText.text = "";
        IsShowingMessage = false;
    }

    public async Task LoadSceneWithFade(string sceneName, float fadeInTime, float fadeOutTime)
    {
        await DoFade(0, 1, fadeInTime);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        var taskCompletionSource = new TaskCompletionSource<bool>();
        asyncLoad.completed += _ => taskCompletionSource.SetResult(true);
        await taskCompletionSource.Task;
        await DoFade(1, 0, fadeOutTime);
        await Task.Delay(100);
    }

    public async Task DoFade(float startValue, float endValue, float duration)
    {
        _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, startValue);
        await _fade.DOFade(endValue, duration).AsyncWaitForCompletion();
    }

}
