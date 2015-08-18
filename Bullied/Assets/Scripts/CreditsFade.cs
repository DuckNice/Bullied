using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreditsFade : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        LevelSetup.OnGameEnded += new LevelSetup.GameEndedHandler(delegate() { _fadeStart = Time.time; });
        _mat.SetColor("_TintColor", new Color(0, 0, 0, 0));
        _credits.enabled = false;
	}

    [SerializeField] private float _fadeDuration = 2000;
    private float _fadeStart;
    [SerializeField] private float _fadeDelay;
    [SerializeField] private Material _mat;
    private float _bgTint = 0;
    [SerializeField] private Text _credits;
    [SerializeField] private float _creditsFadeShowDelay;
    private float sessionStart;
    [SerializeField] private float _creditsMusicEndDelayBeforeTurnDown;
    private int _curStep = 0;
    [SerializeField]
    private float _trackDuration;

    private int _step = 0;
    private bool _changing = false;
 

	// Update is called once per frame
	void Update () {
	    if(!LevelSetup.GameOn)
        {
            switch(_step)
            {
                case 0:
                    if (_bgTint < 0.5f)
                    {
                        _bgTint = 0.5f * Mathf.Clamp01((Time.time - (_fadeStart + _fadeDelay)) / _fadeDuration);
                        _mat.SetColor("_TintColor", new Color(0, 0, 0, _bgTint));
                    }
                    else
                    {
                        sessionStart = Time.time;
                        _step++;
                    }
                    break;

                case 1:
                    if (sessionStart + _creditsFadeShowDelay < Time.time)
                    {
                        _step++;
                        sessionStart = Time.time;
                        _credits.enabled = true;
                        SoundManager.instance.SetCreditsMusicTrack(_curStep);
                        SoundManager.OnTrackChanged += () => { _changing = false; sessionStart = Time.time;};
                    }
                    break;
                case 2:
                    if (_curStep < SoundManager.instance.tracksCredits.Length && sessionStart + _trackDuration > Time.time && !_changing)
                    {
                        _changing  = true;
                        SoundManager.instance.SetCreditsMusicTrack(++_curStep);
                    }
                    else if (SoundManager.instance.tracksCredits.Length <= _curStep)
                    {
                        _step++;
                        sessionStart = Time.time;
                        _changing = false;

                    }
                    break;
                case 3:
                    if (sessionStart + _creditsMusicEndDelayBeforeTurnDown < Time.time && !_changing)
                    {
                        SoundManager.OnTrackChanged += () =>
                        {
                            Application.Quit();
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#endif
                        };

                        SoundManager.instance.SetCreditsMusicTrack(++_curStep);
                        _changing = true;

                    }
                    break;
            }
        }
	}
}
