using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PauseMenuPopupAnimation : PopupAnimation
{
    [SerializeField] private Image _blackFog;
    [SerializeField] private Image _transparentBackground;
    [SerializeField] private GameObject _buttons;

    public override void PlayOpenAnimation() => AsyncPlayOpenAnimation();
    public override void PlayCloseAnimation() => AsyncPlayCloseAnimation();

    public async void AsyncPlayOpenAnimation()
    {
        _buttons.SetActive(false);
        _blackFog.color = new Color(_blackFog.color.r, _blackFog.color.g, _blackFog.color.b, 0);
        _transparentBackground.color = new Color(_transparentBackground.color.r, _transparentBackground.color.g, _transparentBackground.color.b, 0);
        await _transparentBackground.DOFade(0.7f, 0.1f).AsyncWaitForCompletion();
        await _blackFog.DOFade(1, 0.3f).AsyncWaitForCompletion();
        _buttons.SetActive(true);
    }

    public async void AsyncPlayCloseAnimation()
    {
        _buttons.SetActive(false);
        await _blackFog.DOFade(0, 0.3f).AsyncWaitForCompletion();
        await _transparentBackground.DOFade(0, 0.1f).AsyncWaitForCompletion();
        Destroy(this.gameObject);
    }
}
