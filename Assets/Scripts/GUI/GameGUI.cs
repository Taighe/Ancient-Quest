using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Game UI singleton class used for updating and managing the games GUI.
/// </summary>
public class GameGUI : SingletonObject<GameGUI>
{
    public Image HitPointsBack;
    public Image HitPointsFront;
    public Image Fade;
    public TextMeshProUGUI LevelName;
    public TextMeshProUGUI Lives;
    private float _hpIconHeight;
    public bool IsTransitionDone 
    {
        get
        {
            return _counter >= _time;
        }
    }

    private float _counter;
    private float _time;

    public override void Awake()
    {
        base.Awake();
        _hpIconHeight = HitPointsBack.rectTransform.rect.height;
    }

    public void UpdateHitPoints(int hp, int hpMax)
    {
        var frontValue = _hpIconHeight * hp;
        var backValue = _hpIconHeight * hpMax;
        HitPointsBack.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, backValue);
        HitPointsFront.rectTransform.sizeDelta = new Vector2(HitPointsFront.rectTransform.sizeDelta.x, -Mathf.Abs(backValue - frontValue) );
    }

    public void UpdateLives(int lives)
    {
        Lives.text = $"x{lives}";
    }

    public void UpdateLevelName(string name)
    {
        LevelName.text = name;
    }

    public void TransitionFadeInByAmount(float amount)
    {
        if (Fade != null)
        {
            Color color = Fade.color;
            color.a = amount;
            Fade.color = color;
        }
    }

    public IEnumerator TransitionFade( float time, bool fadeOut = false)
    {
        _counter = 0;
        _time = time;
        float fade = 0;
        float f = fadeOut ? 1 : 0;

        while (fade < 1)
        {
            fade = _counter / _time;
            Color color = Fade.color;
            color.a = Mathf.Abs(f - fade);
            _counter = Mathf.Min(_counter + 1.0f * Time.unscaledDeltaTime, _time);
            Fade.color = color;
            yield return new WaitForEndOfFrame();
        }
    }
}
