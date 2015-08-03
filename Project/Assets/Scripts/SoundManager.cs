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
    [SerializeField]
    public AudioSource IntroSource;
    public AudioSource BuildupSource;
    public AudioSource ClimaxSource;
    public AudioSource EncoreSource;
    public MusicTrack[] tracks;

    private void OnEnable()
    {
        StartCoroutine(SwitchSound(MusicTrack.Buildup));
        LevelSetup.OnRadiusChanged += SetMusicTrack;
    }



    void SetMusicTrack(int CurrentStep)
    {
        MusicTrack newTrack = MusicTrack.Buildup;

        

        StartCoroutine(SwitchSound(newTrack));
    }


    IEnumerator SwitchSound(MusicTrack newTrack)
    {
        IntroSource.loop = false;
        BuildupSource.loop = false;
        ClimaxSource.loop = false;

        var playingSource = (IntroSource.isPlaying)
            ? IntroSource : (BuildupSource.isPlaying) 
            ? BuildupSource : (ClimaxSource.isPlaying) 
            ? ClimaxSource : EncoreSource;

        while (true)
        {
            if (!playingSource.isPlaying)
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

                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}