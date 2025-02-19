using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class Dialog
{
    public int ID;
    public int Next; // Próximo ID (se não houver respostas)
    public string Background; // Nome do background
    public List<string> CharactersOnScreen; // Lista de personagens
    public string SpeakingCharacter; // Nome do personagem que está falando
    public string Effect; // Efeito especial (ex: fade-in, shake)
    public List<Response> Responses; // Opções de resposta
    public LanguageOptions Languages; // Textos em diferentes idiomas
}

[System.Serializable]
public class Response
{
    public LanguageOptions Languages; // Textos em diferentes idiomas
    public int NextDialogID; // ID do próximo diálogo
}

[System.Serializable]
public class LanguageOptions
{
    public string ENGLISH;
    public string PORTUGUESE;
}

[System.Serializable]
public class DialogWrapper
{
    public List<Dialog> Dialogues;
}

public class VisualNovelGameplayManager : MonoBehaviour
{
    public string jsonFileName = "Dialogues.json";
    public List<Dialog> dialogues;
    public string languageSel = "ENGLISH";

    [SerializeField] private VisualNovelScreenController _visualNovelScreen;

    void Start()
    {
        string filePath = GetFilePath();

        if (File.Exists(filePath))
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                DialogWrapper wrapper = JsonUtility.FromJson<DialogWrapper>(jsonContent);

                if (wrapper != null && wrapper.Dialogues != null && wrapper.Dialogues.Count > 0)
                {
                    dialogues = wrapper.Dialogues;
                    ShowDialogAsync(1);
                }
                else
                {
                    Debug.LogWarning("The JSON file is empty or invalid.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error reading JSON: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError($"Couldn't find JSON file at: {filePath}");
        }
    }

    public async void ShowDialogAsync(int id)
    {
        var mainDialog = dialogues.Find(d => d.ID == id);
        if (mainDialog != null)
        {
            await _visualNovelScreen.SetBackground(mainDialog.Background);
            _visualNovelScreen.SetCharacters(mainDialog.CharactersOnScreen, mainDialog.SpeakingCharacter);

            string dialogText = GetLanguageText(mainDialog.Languages);
            await _visualNovelScreen.ShowDialogText($"{dialogText}", mainDialog.SpeakingCharacter);

            if (mainDialog.Responses != null && mainDialog.Responses.Count > 0)
            {
                await UniTask.Delay(1000);
                await ShowOptionsAsync(mainDialog.Responses);
            }
            else if (mainDialog.Next != -1)
            {
                await UniTask.WaitUntil(() => Input.anyKeyDown);
                ShowDialogAsync(mainDialog.Next);
            }
            else
                _visualNovelScreen.HideDialogText();
        }
        else
        {
            Debug.LogError($"Dialog with ID {id} not found.");
        }
    }

    private async Task ShowOptionsAsync(List<Response> responses)
    {
        Debug.Log("Showing options...");

        for (int i = 0; i < responses.Count; i++)
        {
            _visualNovelScreen.ShowOption(i + 1, GetLanguageText(responses[i].Languages));
        }

        int selectedOption = await _visualNovelScreen.GetPlayerChoiceAsync();
        ShowDialogAsync(responses[selectedOption - 1].NextDialogID);
    }

    private string GetLanguageText(LanguageOptions languages)
    {
        switch (languageSel.ToUpper())
        {
            case "ENGLISH":
                return languages.ENGLISH ?? "Text not available";
            case "SPANISH":
                return languages.PORTUGUESE ?? "Texto no disponible";
            default:
                return "Language not supported";
        }
    }

    private string GetFilePath()
    {
#if UNITY_EDITOR
        // Caminho para o Editor
        return Path.Combine(Application.dataPath, "StreamingAssets", jsonFileName);
#else
        // Caminho para builds
        return Path.Combine(Application.streamingAssetsPath, jsonFileName);
#endif
    }
}