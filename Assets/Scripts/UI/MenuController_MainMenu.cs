using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController_MainMenu : MonoBehaviour
{
    private void Start()
    {
        _versionText.text = Application.version;
    }

    [SerializeField] private TextMeshProUGUI _versionText;
    public async void PlayGame()
    {
        await ScreenManager.Instance.LoadSceneWithFade("GameplayScene", 0.4f, 0.4f);
        //GameManager.Instance.IsCursorVisible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
