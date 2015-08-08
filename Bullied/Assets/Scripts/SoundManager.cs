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
    public delegate void TrackChangeHandler();
    public static event TrackChangeHandler OnTrackChanged;

    public AudioSource IntroSource;
    public AudioSource BuildupSource;
    public AudioSource ClimaxSource;
    public AudioSource EncoreSource;
    public MusicTrack[] tracks;

    private void OnEnable()
    {
        LevelSetup.OnStep += SetInGameMusicTrack;
        OnTrackChanged += () => { };
    }

    public void StopMusic()
    {
        IntroSource.Stop();
        BuildupSource.Stop();
        ClimaxSource.Stop();
        EncoreSource.Stop();
    }

    void SetInGameMusicTrack(int currentStep)
    {
        MusicTrack newTrack = MusicTrack.Buildup;

        newTrack = tracks[Mathf.Clamp(currentStep, 0, tracks.Length - 1)];

        StartCoroutine(SwitchSound(newTrack));
    }


    IEnumerator SwitchSound(MusicTrack newTrack)
    {
        IntroSource.loop = false;
        BuildupSource.loop = false;
        ClimaxSource.loop = false;
        EncoreSource.loop = false;

        var playingSource = (IntroSource.isPlaying)
            ? IntroSource : (BuildupSource.isPlaying) 
            ? BuildupSource : (ClimaxSource.isPlaying) 
            ? ClimaxSource : EncoreSource;

        while (true)
        {
            if (!playingSource.isPlaying && LevelSetup.GameOn)
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
}