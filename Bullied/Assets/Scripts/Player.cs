using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    #region debug
    [SerializeField]
    private bool _moveWhilePushed = true;
    #endregion

    public float CenterTolerance;
    private Vector3 _direction = new Vector3(0, 0, 0);
    public float FallSpeed = 7;
    public float EndFallSpeed = 5;
    public AnimationCurve FallSpeedCurve;

    public int HurtDivisor = 2;
    private bool _playerControl = true;
    public float PlayerSpeed;
    private Rigidbody _playerRig;
    

    #region player movement disability
        [SerializeField] private Vector3 _shakeStartHitsAmount;
        [SerializeField] private Vector3 _slowStartHitsAmount;
        [SerializeField] private float _shakeInterval;
        [SerializeField] private int _shakeStartStep;
        [SerializeField] private int _slowStartStep;
        private int _slowHitCounter = 0;
        private int _shakeHitCounter = 0;
        private float _lastShake;
        [SerializeField] private float _shakeLength;
        private bool _shakeInProgress = false;
        [SerializeField] private float _shakeSpeed;
    #endregion

        [SerializeField]
        private float bullyHitSaveDur = 3;

    public void Awake()
    {
        _playerRig = GetComponent<Rigidbody>();
    }

    public bool HitBully(Vector2 bullyPosition, GameObject bully)
    {
        if (LevelSetup.GameOn)
        {
            if (_playerControl || !LevelSetup.instance.bulliesHitWithinDuration.Exists(x => x.bully == bully))
            {
                _playerControl = false;
                _direction = Vector3.zero - transform.position;

                float fallSpeed = FallSpeedCurve.Evaluate((float)LevelSetup.instance.CurrentStep / LevelSetup.instance.TotalSteps);

                _playerRig.velocity = (_direction * ((EndFallSpeed * fallSpeed) + ((1-fallSpeed) * FallSpeed)));

                LevelSetup.instance.bulliesHitWithinDuration.Add(new LevelSetup.BullyHitContainer(bully, Time.time));
                return true;
            }
        }

        

        return false;
    }

    private void Update()
    {
        if (LevelSetup.GameOn)
        {
            if (_playerControl)
            {
                var x = Input.GetAxis("Horizontal");
                var y = Input.GetAxis("Vertical");
                float drag = 0;

                if (LevelSetup.instance.CurrentStep >= _slowStartStep && _slowStartHitsAmount.x < _slowHitCounter)
                {
                    float relHit = Mathf.Clamp01((_slowHitCounter - _slowStartHitsAmount.x) / _slowStartHitsAmount.y);
                    drag = relHit * _slowStartHitsAmount.z;
                }

                Vector3 offset = new Vector3(x, y, 0) * (PlayerSpeed - drag) * Time.deltaTime;

                transform.position += offset;

                if (!_shakeInProgress && LevelSetup.instance.CurrentStep >= _shakeStartStep && _shakeHitCounter >= _shakeStartHitsAmount.x && 
                    _lastShake + _shakeInterval <= Time.time)
                {
                    StartCoroutine("Shake");
                }
            }
            else
            {
                if (!(_playerRig.velocity.magnitude < 0.02f) && _waitStart < 0.0f)
                {
                    _waitStart = Time.time;
                }
                else if (_timeToWait + _waitStart < Time.time)
                {
                    _playerControl = true;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    _waitStart = -1;
                }

                if(_moveWhilePushed)
                {
                    
                }
            }
        }
        else
        {
            _playerRig.velocity = Vector3.zero;
        }

        for (int i = LevelSetup.instance.bulliesHitWithinDuration.Count - 1; i >= 0; i-- )
        {
            if (LevelSetup.instance.bulliesHitWithinDuration[i].time + bullyHitSaveDur <= Time.time)
            {
                LevelSetup.instance.bulliesHitWithinDuration.RemoveAt(i);
            }
        }

        if (!_hitBullyOnUpdate)
        {
            _isHittingBully = false;
        }
        
        _hitBullyOnUpdate = false;
    }

    bool _hitBullyOnUpdate = false;
    bool _isHittingBully = false;

    private float _waitStart = -1;
    [SerializeField] private float _timeToWait = 2;

    private IEnumerator Shake()
    {
        _shakeInProgress = true;
        _lastShake = Time.time;

        while(true)
        {
            float relAmount = Mathf.Clamp01((_shakeHitCounter - _shakeStartHitsAmount.x) / _shakeStartHitsAmount.y);
            float shakeAmount = relAmount * _shakeStartHitsAmount.z;

            transform.position += new Vector3(Mathf.Sin(Time.time * _shakeSpeed) * shakeAmount, 0, 0);
            
            if(_lastShake + _shakeLength < Time.time){
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        _lastShake = Time.time;
        _shakeInProgress = false;
    }
    
    private void OnTriggerStay(Collider bully)
    {
        if (LevelSetup.GameOn)
        {            
            if (HitBully(new Vector2(bully.transform.position.x, bully.transform.position.y),
                bully.gameObject))
            {
                if (LevelSetup.instance.CurrentStep >= _slowStartStep && !_isHittingBully)
                {
                    _slowHitCounter++;
                }
                if (LevelSetup.instance.CurrentStep >= _shakeStartStep && !_isHittingBully)
                {
                    _shakeHitCounter++;
                }

                _hitBullyOnUpdate = true;
                _isHittingBully = true;
            }

            if (LevelSetup.instance.CurrentStep >= LevelSetup.instance.TotalSteps)
            {
                LevelSetup.instance.EndGame();
            }
        }
    }
}