using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreditsFade : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        LevelSetup.OnGameEnded += new LevelSetup.GameEndedHandler(delegate() { _fadeStart = Time.time; });
        _mat.SetColor("_TintColor", new Color(0, 0, 0, 0));
        _credits.rectTransform.anchoredPosition.Set(_credits.rectTransform.anchoredPosition.x, _creditsStartEndY.x);
        _credits.enabled = false;
	}

    [SerializeField] private float _fadeDuration = 2000;
    private float _fadeStart;
    [SerializeField] private float _fadeDelay;
    [SerializeField] private Material _mat;
    private float _bgTint = 0;
    [SerializeField] private Text _credits;
    [SerializeField] private Vector2 _creditsStartEndY;
    [SerializeField] private float _speed = 2.0f;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private float creditsFadeShowDelay;
    private float sessionStart;
    [SerializeField] private float creditsMusicEndDelayBeforeTurnDown;
    private int curStep = 0;

    private int step = 0;
    

	// Update is called once per frame
	void Update () {
	    if(!LevelSetup.GameOn)
        {
            switch(step)
            {
                case 0:
                    if (_bgTint < 1)
                    {
                        _bgTint = 1 * Mathf.Clamp01((Time.time - (_fadeStart + _fadeDelay)) / _fadeDuration);
                        _mat.SetColor("_TintColor", new Color(0, 0, 0, _bgTint));
                        sessionStart = Time.time;
                    }
                    else
                    {
                        step++;
                    }
                    break;

                case 1:
                    if (sessionStart + creditsFadeShowDelay < Time.time)
                    {
                        step++;
                        sessionStart = Time.time;
                        _credits.enabled = true;
                        soundManager.SetCreditsMusicTrack(curStep);
                    }
                    break;
                case 2:
                    if (curStep < soundManager.tracksCredits.Length)
                    {
                        soundManager.SetCreditsMusicTrack(++curStep);
                    }
                    else
                    {
                        step++;
                        sessionStart = Time.time;
                    }
                    break;
                case 3:
                    if (sessionStart + creditsMusicEndDelayBeforeTurnDown < Time.time)
                    {
                        Application.Quit();
                    }
                    break;
            }

        }
	}
}
