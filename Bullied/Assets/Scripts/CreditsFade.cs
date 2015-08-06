using UnityEngine;
using System.Collections;

public class CreditsFade : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        LevelSetup.OnGameEnded += new LevelSetup.GameEndedHandler(delegate() { fadeStart = Time.time; });
        mat.SetColor("_TintColor", new Color(0, 0, 0, 0));
	}

    [SerializeField] private float _fadeDuration = 2000;
    private float fadeStart;
    [SerializeField] private float fadeDelay;
    [SerializeField] private Material mat;
    
	// Update is called once per frame
	void Update () {
	    if(!LevelSetup.GameOn)
        {
            mat.SetColor("_TintColor", new Color(0, 0, 0, 1 * Mathf.Clamp01((Time.time - (fadeStart + fadeDelay)) / _fadeDuration)));
        }
	}
}
