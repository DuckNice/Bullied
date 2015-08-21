using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class LevelSetup : MonoBehaviour
{
    private static LevelSetup _instance;

    public static LevelSetup instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LevelSetup>();

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


    public delegate void RadiusChangeHandler(int CurrentStep);
    public static event RadiusChangeHandler OnRadiusChanged;

    public delegate void StepHandler(int CurrentStep);
    public static event StepHandler OnStep;

    public delegate void GameEndedHandler();
    public static event GameEndedHandler OnGameEnded;

    [SerializeField] private bool _doStep = false;

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
    public float WalkSpeed;
    private float _startSize;
    public float TimeBetweenSteps;
    [SerializeField]
    private float _playerGrowFactor = 3;
    public float Tolerance = 0;
    public float StepTolerance = 0.02f;
    private bool stepable = true;
    public int TotalSteps = 4;
    [SerializeField] private float _bullyEndSize = 4;
    [SerializeField] private AnimationCurve _bullySizeScale;
    [SerializeField] private float _bullyGrowSpeed;
    [SerializeField] private float _innerRadiusScreenFactor = 0.3f;
    public List<float> UnitsScale = new List<float>();
    public float CirclePiece { get; private set; }
    public static bool GameOn { get; private set; }
    public int CurrentStep { get; private set; }
    public List<Vector2> BullyPositions { get; private set; }
    [HideInInspector] public List<bool> BullyStepped = new List<bool>();

    private void OnEnable()
    {
        BullyPositions = new List<Vector2>();

        for (var i = UnitsScale.Count - 1; i >= 0; i--)
        {
            BullyPositions.Add(UnitCirclePosition(i));
            BullyStepped.Add(true);
        }

        _nextStep = TimeBetweenSteps;

        _startSize = Camera.main.orthographicSize;
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
            if ((stepable && Time.time > _nextStep) || _doStep)
            {
                if (CurrentStep < TotalSteps - 1)
                {
                    CurrentStep++;
                    OnStep(CurrentStep);
                    StartCoroutine(GrowBullies());
                    stepable = false;
                    _doStep = false;
                }
                else
                {
                    CurrentStep++;
                    
                    SoundManager.instance.StopMusic();
                    UpdateCirclePiece();
                    stepable = false;
                    _doStep = false;

                }
            }
        }
    }

    public void EndGame()
    {
        GameOn = false;
        OnGameEnded();
        SoundManager.instance.StopMusic();
    }

    public void Step()
    {
        _nextStep = Time.time + TimeBetweenSteps;
        _radius -= _lengthOfStep;

        OnRadiusChanged(CurrentStep);
        UpdateCirclePiece();
        stepable = true;
        for (int i = BullyStepped.Count - 1; i >= 0; i-- )
        {
            BullyStepped[i] = false;
        }
    }

    public IEnumerator GrowBullies()
    {
        float sizePoint = _bullySizeScale.Evaluate(((float)CurrentStep / TotalSteps)) * _bullyEndSize;

        float newSize = _startSize - sizePoint;
        
        while(true)
        {
            float mbox = _bullyGrowSpeed * Time.fixedDeltaTime;
            Camera.main.orthographicSize -= mbox;
            Player.transform.localScale -= new Vector3(mbox / _playerGrowFactor, mbox / _playerGrowFactor, mbox / _playerGrowFactor);

            if (newSize >= Camera.main.orthographicSize)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }
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