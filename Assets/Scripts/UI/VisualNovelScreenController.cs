using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class Background
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public Sprite SpriteImage;
}
[Serializable]
class Character
{
    public string Name;
    public GameObject CharacterPrefab;
}

public class VisualNovelScreenController : MonoBehaviour
{

    [Header("Dialog Screen:")]
    [SerializeField] private GameObject _dialogScreen;
    [SerializeField] private TMP_Text _characterName;
    [SerializeField] private TMP_Text _dialogText;
    private bool _isDialogOngoing;

    [Header("Environment:")]
    [SerializeField] private Image _background;
    [SerializeField] private Transform _characterContainer;
    [SerializeField] private GameObject _optionButtonPrefab;
    [SerializeField] private Transform _optionButtonContainer;
    [SerializeField] private Image _fadeImage;


    private List<Character> _currentCharacters = new List<Character>();
    private List<GameObject> _currentOptions = new List<GameObject>();
    private Background _currentBackground = new Background();

    [Header("AssetReferences:")]

    [SerializeField] private List<Background> _backgrounds;
    [SerializeField] private List<Character> _characters;


    int selectedOption = -1;

    public async Task SetBackground(string backgroundName)
    {

        if (backgroundName != _currentBackground.Name)
        {
            _fadeImage.DOFade(1, 1f);
            await UniTask.Delay(1000);
            _currentBackground = _backgrounds.Find(b => b.Name == backgroundName);
            _background.sprite = _currentBackground.SpriteImage;
            _fadeImage.DOFade(0, 1f);
            await UniTask.Delay(1000);

        }
    }

    public async Task SetCharacters(List<string> characters, string mainCharacter)
    {
        // Limpa os personagens atuais

        List<Character> charactersClone = new List<Character>(_currentCharacters);
        foreach (var character in charactersClone)
        {
            if (!characters.Contains(character.Name))
            {
                //character.CharacterPrefab.transform.DOScale(new Vector3(0, 0, 1), .2f);
                //await UniTask.Delay(200);
                Destroy(character.CharacterPrefab);
                _currentCharacters.Remove(character);
            }
            else if (character.Name != mainCharacter)
                _currentCharacters.Find(c => c.Name == character.Name).CharacterPrefab.transform.DOScale(new Vector3(1, 1, 1), .2f);

        }

        // Adiciona os novos personagens na tela
        foreach (var characterName in characters)
        {
            if (!_currentCharacters.Exists(c => c.Name == characterName))
            {
                var characterPrefab = _characters.Find(c => c.Name == characterName);
                GameObject character = Instantiate(characterPrefab.CharacterPrefab, _characterContainer);
                character.transform.DOScale(new Vector3(1, 1, 1), .2f);

                _currentCharacters.Add(new Character { Name = characterName, CharacterPrefab = character });
            }
        }

        _currentCharacters.Find(c => c.Name == mainCharacter).CharacterPrefab.transform.DOScale(new Vector3(1.2f, 1.2f, 1), .3f);

    }

    public void ShowOption(int index, string text)
    {
        // Cria e configura um botão de opção
        GameObject button = Instantiate(_optionButtonPrefab, _optionButtonContainer);
        button.GetComponentInChildren<TMP_Text>().text = $"{index}. {text}";
        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            OnOptionSelected(index);
        });

        _currentOptions.Add(button);
    }

    public async UniTask<int> GetPlayerChoiceAsync()
    {

        await UniTask.WaitUntil(() => selectedOption != -1);


        int optionIndex = selectedOption;
        selectedOption = -1;
        // Remove os botões da tela
        foreach (var button in _currentOptions)
        {
            Destroy(button);
        }
        _currentOptions.Clear();

        return optionIndex;
    }

    private void OnOptionSelected(int index)
    {
        Debug.Log($"Option {index} selected!");
        selectedOption = index;
        // Aqui você pode informar o índice selecionado para continuar o diálogo
    }

    public async Task ShowDialogText(string dialog, string characterName)
    {
        _characterName.text = characterName;
        _dialogText.text = "";
        _dialogScreen.SetActive(true);
        await SetDialogText(dialog);
    }

    public void HideDialogText()
    {
        _characterName.text = "";
        _dialogText.text = "";
        _dialogScreen.SetActive(false);
    }
    async Task SetDialogText(string fullText)
    {
        if (_isDialogOngoing)
        {
            _isDialogOngoing = false;
            _dialogText.text = fullText;
        }
        else
        {
            string currentText = "";
            _isDialogOngoing = true;
            for (int i = 0; i <= fullText.Length; i++)
            {
                if (!_isDialogOngoing)
                    return;
                if (UnityEngine.Random.Range(0, 3) == 1)
                    SoundManager.Instance.PlaySFX("Type", UnityEngine.Random.Range(0.8f, 1.3f));

                currentText = fullText.Substring(0, i);
                _dialogText.text = currentText;
                await UniTask.Delay(2);
            }
        }

        _isDialogOngoing = false;
    }



}
