using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : SingletonObject<GameUI>
{
    public Image HitPointsBack;
    public Image HitPointsFront;
    public Image Fade;
    public bool IsTransitionDone 
    {
        get
        {
            return _counter >= _time;
        }
    }

    private float _counter;
    private float _time;

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
