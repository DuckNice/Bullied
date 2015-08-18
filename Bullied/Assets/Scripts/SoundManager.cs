using UnityEngine;
using System.Collections;

public enum MusicTrack
{
    Intro,
    Buildup,
    Climax,
    Encore
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    public static SoundManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<SoundManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }

    public delegate void TrackChangeHandler();
    public static event TrackChangeHandler OnTrackChanged;

    public AudioSource IntroSource;
    public AudioSource BuildupSource;
    public AudioSource ClimaxSource;
    public AudioSource EncoreSource;
    public MusicTrack[] tracksInGame;
    public MusicTrack[] tracksCredits;

    public float FadeDuration = 2;

    private void OnEnable()
    {
        LevelSetup.OnStep += SetInGameMusicTrack;
        OnTrackChanged += () => { };
    }

    public void StopMusic()
    {
        StartCoroutine(FadeSounds());
    }

    void SetInGameMusicTrack(int currentStep)
    {
        MusicTrack newTrack = MusicTrack.Buildup;

        newTrack = tracksInGame[Mathf.Clamp(currentStep, 0, tracksInGame.Length - 1)];

        StartCoroutine(SwitchSound(newTrack));
    }

    public void SetCreditsMusicTrack(int currentStep)
    {
        MusicTrack newTrack = MusicTrack.Buildup;

        newTrack = tracksCredits[Mathf.Clamp(currentStep, 0, tracksCredits.Length - 1)];

        StartCoroutine(SwitchSound(newTrack));
    }

    IEnumerator SwitchSound(MusicTrack newTrack)
    {
        IntroSource.loop = false;
        IntroSource.volume = 1;
        BuildupSource.loop = false;
        BuildupSource.volume = 1;
        ClimaxSource.loop = false;
        ClimaxSource.volume = 1;
        EncoreSource.loop = false;
        EncoreSource.volume = 1;

        var playingSource = (IntroSource.isPlaying)
            ? IntroSource : (BuildupSource.isPlaying) 
            ? BuildupSource : (ClimaxSource.isPlaying) 
            ? ClimaxSource : EncoreSource;

        while (true)
        {
            if (playingSource == null || !playingSource.isPlaying)
            {
                switch (newTrack)
                {
                    case MusicTrack.Intro:
                        playingSource = IntroSource;
                        break;
                    case MusicTrack.Buildup:
                        playingSource = BuildupSource;
                        break;
                    case MusicTrack.Climax:
                        playingSource = ClimaxSource;
                        break;
                    case MusicTrack.Encore:
                        playingSource = EncoreSource;
                        break;
                }

                playingSource.Play();
                playingSource.loop = true;

                OnTrackChanged();

                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator FadeSounds()
    {
        IntroSource.loop = false;
        BuildupSource.loop = false;
        ClimaxSource.loop = false;
        EncoreSource.loop = false;

        var playingSource = (IntroSource.isPlaying)
            ? IntroSource : (BuildupSource.isPlaying)
            ? BuildupSource : (ClimaxSource.isPlaying)
            ? ClimaxSource : EncoreSource;

        float StartTime = Time.time;

        while (StartTime + FadeDuration > Time.time)
        {
            float volume = 1 - Mathf.Clamp01((Time.time - StartTime) / FadeDuration);
            IntroSource.volume = volume;
            BuildupSource.volume = volume;
            ClimaxSource.volume = volume;
            EncoreSource.volume = volume;

            yield return new WaitForFixedUpdate();
        }

        IntroSource.Stop();
        BuildupSource.Stop();
        ClimaxSource.Stop();
        EncoreSource.Stop();

        OnTrackChanged();

    }
}