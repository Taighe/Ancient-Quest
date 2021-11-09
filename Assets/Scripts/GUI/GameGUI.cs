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
    public Image MessageBox;
    public TextMeshProUGUI Message;
    private float _hpIconHeight;
    public bool IsTransitionDone 
    {
        get
        {
            return _counter == -1 || _counter >= _time;
        }
    }

    private float _counter = -1;
    private float _time;
    private bool _isMessageRunning;

    public override void Awake()
    {
        base.Awake();
        _hpIconHeight = HitPointsBack.rectTransform.rect.height;
        MessageBox.rectTransform.localScale = Vector3.zero;
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

    public void StartMessage(string[] pages, float rate, float delay = 3)
    {
        if(!_isMessageRunning)
        {
            StartCoroutine(StartMessageAsync(pages, rate, delay));
            _isMessageRunning = true;
        }
    }

    private IEnumerator StartMessageAsync(string[] pages, float rate, float delay)
    {
        int index = 0;
        StartCoroutine(MessageBoxDisplayAsync(0.3f));

        while(!IsTransitionDone)
            yield return new WaitForEndOfFrame();

        while (index < pages.Length)
        {
            StartCoroutine(DisplayMessageTextAsync(pages[index], rate, delay));

            while (!IsTransitionDone)
                yield return new WaitForEndOfFrame();

            index++;
        }

        StartCoroutine(MessageBoxDisplayAsync(0.3f, false));

        while (!IsTransitionDone)
            yield return new WaitForEndOfFrame();

        _isMessageRunning = false;
    }

    private IEnumerator DisplayMessageTextAsync(string message, float rate, float delay)
    {
        Message.text = "";
        var m = message;
        _counter = 0;
        _time = m.Length;
        while (_counter < _time)
        {
            yield return new WaitForSecondsRealtime(rate);
            Message.text = m.Substring(0, (int)_counter);
            _counter++;
        }

        _counter = 0;
        _time = delay;

        while (_counter < _time)
        {
            yield return new WaitForEndOfFrame();
            _counter = Mathf.Min(_counter + 1.0f * Time.unscaledDeltaTime, _time);
        }

        _counter = _time;
    }

    private IEnumerator MessageBoxDisplayAsync(float time, bool display = true)
    {
        _counter = 0;
        _time = time;
        Vector3 toScale = display ? new Vector3(1, 1, 1) : Vector3.zero;
        Vector3 currentScale = MessageBox.rectTransform.localScale;
        Debug.Log("Displayed once");
        while (_counter < _time)
        {
            yield return new WaitForEndOfFrame();
            var t = _counter / _time;
            MessageBox.rectTransform.localScale = Vector3.Lerp(currentScale, toScale , t);
            _counter = Mathf.Min(_counter + 1.0f * Time.unscaledDeltaTime, _time);
        }

        MessageBox.rectTransform.localScale = toScale; 
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
