using System.Collections.Generic;
using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    public delegate void RadiusChangeHandler(int CurrentStep);
    public static event RadiusChangeHandler OnRadiusChanged;

    public delegate void StepHandler(int CurrentStep);
    public static event StepHandler OnStep;

    [SerializeField] private float _radiusScreenFactor = 1;
    private float _lengthOfStep;
    private float _nextStep;
    private float _radiansEachUnit;
    private float _radius;
    public float AttackSpeed;
    public float AttackTolerance;
    public GameObject BullyPrefab;
    public GameObject Player;
    public float StepSpeed;
    public float TimeBetweenSteps;
    public float Tolerance = 0.02f;
    private bool stepable = true;
    public int TotalSteps = 4;
    [SerializeField]
    private float _innerRadiusScreenFactor = 0.3f;
    public List<float> UnitsScale = new List<float>();
    public float CirclePiece { get; private set; }
    public static bool GameOn { get; private set; }
    public int CurrentStep { get; private set; }
    public List<Vector2> BullyPositions { get; private set; }

    private void OnEnable()
    {
        BullyPositions = new List<Vector2>();

        for (var i = UnitsScale.Count - 1; i >= 0; i--)
        {
            BullyPositions.Add(UnitCirclePosition(i));
        }

        _nextStep = TimeBetweenSteps;
    }

    private void Start()
    {
        GameOn = true;

        SoundManager.OnTrackChanged += Step;

        var dimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        _radius = (dimensions.x < dimensions.y) ? dimensions.x*_radiusScreenFactor : dimensions.y*_radiusScreenFactor;
        var _innerRadius = (dimensions.x < dimensions.y) ? dimensions.x * _innerRadiusScreenFactor : dimensions.y * _innerRadiusScreenFactor;

        _lengthOfStep = (_radius - _innerRadius)/TotalSteps;

        _radiansEachUnit = (2*Mathf.PI)/UnitsScale.Count;

        UpdateCirclePiece();

        Bully.SceneManager = this;

        for (var i = 0; i < UnitsScale.Count; i++)
        {
            var unit = Instantiate(BullyPrefab);
            var bullyScript = unit.GetComponent<Bully>();
            bullyScript.CircleNumber = i;
            bullyScript.Init();
        }
    }

    public float CalculateRadians(int i)
    {
        return _radiansEachUnit * i;
    }

    private void Update()
    {
        if (GameOn)
        {
            if (stepable && Time.time > _nextStep)
            {
                CurrentStep++;
                OnStep(CurrentStep);
                stepable = false;

            }
        }
    }

    public void EndGame()
    {
        GameOn = false;
    }

    public void Step()
    {
        _nextStep += TimeBetweenSteps;
        _radius -= _lengthOfStep;

        OnRadiusChanged(CurrentStep);
        UpdateCirclePiece();
        stepable = true;
    }

    public Vector3 UnitCirclePosition(int index)
    {
        var radians = _radiansEachUnit*index;

        var x = Mathf.Cos(radians)*_radius;
        var y = Mathf.Sin(radians)*_radius;

        return new Vector3(x, y, 0);
    }

    private void UpdateCirclePiece()
    {
        CirclePiece = ((_radius*2)*Mathf.PI)/(360/(Mathf.Rad2Deg*_radiansEachUnit));
    }
}