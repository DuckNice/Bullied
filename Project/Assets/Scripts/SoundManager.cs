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


    private void OnEnable()
    {
        Invoke("setTrack", 20);
    }


    void SetTrack()
    {
        SetMusicTrack(MusicTrack.Buildup);
    }


    public void SetMusicTrack(MusicTrack newTrack)
    {
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