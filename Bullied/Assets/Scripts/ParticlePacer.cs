using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticlePacer : MonoBehaviour {
    public Material mat;
    [SerializeField] private Vector2 _particlesFadeStartDur;
    [SerializeField] private Vector2 _particlesOverlapStartDur;

    [SerializeField] private Vector2 _particleSystemStartStopZ = new Vector2(6.4f, -0.7f);
    [SerializeField] private AnimationCurve _particleFadeIn;
    [SerializeField] private AnimationCurve _particleOverlap;
    [SerializeField] private int startStep = 2;
    private float timeStart = -1;
  
    List<Particle> particles = new List<Particle>();

    protected void Start()
    {
        mat.SetColor("_TintColor", new Color(0.49f, 0.49f, 0.49f, 1));
    }

    protected void Update()
    {
        if(startStep <= LevelSetup.instance.CurrentStep)
        {
            if(timeStart < 0)
            {
                timeStart = Time.time;
            }

            float alpha = Mathf.Clamp01(_particleFadeIn.Evaluate((Time.time - timeStart - _particlesFadeStartDur.x) / _particlesFadeStartDur.y));

            mat.SetColor("_TintColor", new Color(0.49f, 0.49f, 0.49f, alpha));

            if(alpha == 0)
            {
                float pos = Mathf.Clamp01(_particleOverlap.Evaluate((Time.time - timeStart - _particlesFadeStartDur.x - _particlesFadeStartDur.y - _particlesOverlapStartDur.x) / _particlesOverlapStartDur.y));

                float worldPos = (_particleSystemStartStopZ.y * pos) + ((1 - pos) * _particleSystemStartStopZ.x);
                
                transform.position = new Vector3(transform.position.x, transform.position.y, worldPos);
            }
        }
    }
}
