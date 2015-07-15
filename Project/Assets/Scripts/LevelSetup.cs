using System.Collections.Generic;
using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    public delegate void RadiusChangeHandler();

    [SerializeField] private float _radiusScreenFactor = 1;

    private float _lengthOfStep;
    private float _nextStep;
    private float _radiansEachUnit;
    private float _radius;
    public float AttackSpeed;
    public float AttackTolerance;
    public GameObject BullyPrefab;
    public GameObject Player;
    public int TotalSteps = 4;
    public float StepSpeed;
    public float TimeBetweenSteps;
    public float Tolerance = 0.02f;
    public List<float> UnitsScale = new List<float>();
    public float CirclePiece { get; private set; }
    public float CurrentRadius { get; private set; }
    public bool GameOn { get; private set; }
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

        var dimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        _radius = (dimensions.x < dimensions.y) ? dimensions.x*_radiusScreenFactor : dimensions.y*_radiusScreenFactor;

        _lengthOfStep = _radius/TotalSteps;

        CurrentRadius = _radius;

        _radiansEachUnit = (2*Mathf.PI)/UnitsScale.Count;

        UpdateCirclePiece();

        Bully.SceneManager = this;

        float radians = 0;
        for (var i = 0; i < UnitsScale.Count; i++)
        {
            var unit = Instantiate(BullyPrefab);
            var bullyScript = unit.GetComponent<Bully>();
            bullyScript.CircleNumber = i;
            bullyScript.Init();

            //TODO: Funktion til at udregne bully's rotation. Flyt til bully script.
            unit.transform.Rotate(new Vector3(0, 0, (Mathf.Rad2Deg*radians)));

            radians += _radiansEachUnit;
        }
    }

    public event RadiusChangeHandler OnRadiusChanged;

    private void Update()
    {
        if (GameOn)
        {
            if (Time.time > _nextStep)
            {
                CurrentStep++;
                _nextStep += TimeBetweenSteps;
                CurrentRadius -= _lengthOfStep;

                OnRadiusChanged();
                UpdateCirclePiece();
            }
        }
    }

    public void EndGame()
    {
        GameOn = false;

/*#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();*/
    }

    public Vector3 UnitCirclePosition(int index)
    {
        var radians = _radiansEachUnit*index;

        var x = Mathf.Cos(radians)*CurrentRadius;
        var y = Mathf.Sin(radians)*CurrentRadius;

        return new Vector3(x, y, 0);
    }

    private void UpdateCirclePiece()
    {
        CirclePiece = ((CurrentRadius*2)*Mathf.PI)/(360/(Mathf.Rad2Deg*_radiansEachUnit));
    }
}