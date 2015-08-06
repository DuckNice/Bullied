using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticlePacer : MonoBehaviour {
    public ParticleSystem emitter;

    [SerializeField] private AnimationCurve _emitanceProgress;

    public LevelSetup GameManager;

    private float _duration;

    List<Particle> particles = new List<Particle>();

    protected void Start()
    {
        _duration = GameManager.TotalSteps * GameManager.TimeBetweenSteps;
    }

    protected void Update()
    {
     /*   for(int i = particles.Count - 1; i >= 0 ; i++)
        {
            if(particles[i].)
            {
                particles.RemoveAt(i);
            }
        }*/
    }
}
