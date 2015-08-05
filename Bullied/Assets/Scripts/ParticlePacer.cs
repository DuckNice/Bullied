using UnityEngine;
using System.Collections;

public class ParticlePacer : MonoBehaviour {
    public ParticleSystem emitter;

    [SerializeField] private Color _startColorL;
    [SerializeField] private Color _startColorD;
    [SerializeField] private Color _endColorL;
    [SerializeField] private Color _endColorD;
    [SerializeField] private AnimationCurve _colorProgress;

    [SerializeField] private AnimationCurve _emitanceProgress;

    public LevelSetup GameManager;

    private float _duration;

    protected void Start()
    {
        _duration = GameManager.TotalSteps * GameManager.TimeBetweenSteps;
    }

    protected void Update()
    {
        emitter.emissionRate = _emitanceProgress.Evaluate(Time.time / _duration);
    }
}
