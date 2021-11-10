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
    public Animator MessageBoxConfirm;
    private float _hpIconHeight;
    private int _pageIndex;

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
        Message.text = "";
        MessageBoxConfirm.SetBool("isLastPage", false);
        MessageBoxConfirm.SetBool("isReading", true);
        StartCoroutine(MessageBoxDisplayAsync(0.3f));

        while(!IsTransitionDone)
            yield return new WaitForEndOfFrame();

        while (index < pages.Length)
        {
            if(index == pages.Length-1)
                MessageBoxConfirm.SetBool("isLastPage", true);

            StartCoroutine(DisplayMessageTextAsync(pages[index], rate, delay));

            while (!IsTransitionDone)
                yield return new WaitForEndOfFrame();

            index++;
            _pageIndex = index;
            StopCoroutine("DisplayMessageTextAsync");
        }

        StartCoroutine(MessageBoxDisplayAsync(0.3f, false));

        while (!IsTransitionDone)
            yield return new WaitForEndOfFrame();

        _isMessageRunning = false;
    }

    private IEnumerator DisplayMessageTextAsync(string message, float rate, float delay)
    {
        _counter = 0;
        MessageBoxConfirm.SetBool("isReading", true);
        bool endByButton = false;
        if (delay == 0)
        {
            _time = 1;
            endByButton = true;
        }
        else
            _time = delay;

        Message.text = "";
        int index = 0;
        int length = message.Length;
        while (index <= length)
        {
            yield return new WaitForSecondsRealtime(rate);
            Message.text = message.Substring(0, index);
            index++;
        }

        MessageBoxConfirm.SetBool("isReading", false);

        while (_counter < _time)
        {
            yield return new WaitForEndOfFrame();

            if (endByButton && ControllerMaster.Input.GetUseButton())
            {
                _counter = 1;
            }

            if (!endByButton)
            {
                _counter = Mathf.Min(_counter + 1.0f * Time.unscaledDeltaTime, _time);
            }
        }

        _counter = _time;
    }

    private IEnumerator MessageBoxDisplayAsync(float time, bool display = true)
    {
        _counter = 0;
        _time = time;
        Vector3 toScale = display ? new Vector3(1, 1, 1) : Vector3.zero;
        Vector3 currentScale = MessageBox.rectTransform.localScale;

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
