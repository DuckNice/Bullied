using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float CenterTolerance;
    private Vector3 _direction = new Vector3(0, 0, 0);
    public float FallSpeed;
    public int HurtDivisor = 2;
    private GameObject _lastBullyHit;
    private bool _playerControl = true;
    public float PlayerSpeed;
    public LevelSetup LvlRef;

    public bool HitBully(Vector2 bullyPosition, bool sameBully)
    {
        if (LevelSetup.GameOn)
        {
            if (_playerControl || !sameBully)
            {
                _playerControl = false;

                _direction = Vector3.zero - transform.position;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().velocity = (_direction*FallSpeed);

                return true;
            }
        }

        return false;
    }

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
    
    private void Update()
    {
        if (LevelSetup.GameOn)
        {
            if (_playerControl)
            {
                var x = Input.GetAxis("Horizontal");
                var y = Input.GetAxis("Vertical");
                float drag = 0;

                if(LvlRef.CurrentStep >= _slowStartStep && _slowStartHitsAmount.x < _slowHitCounter)
                {
                    float relHit = Mathf.Clamp01((_slowHitCounter - _slowStartHitsAmount.x) / _slowStartHitsAmount.y);
                    drag = relHit * _slowStartHitsAmount.z;
                }

                Vector3 offset = new Vector3(x, y, 0) * (PlayerSpeed - drag) * Time.deltaTime;

                transform.position += offset;

                if(!_shakeInProgress && LvlRef.CurrentStep >= _shakeStartStep && _shakeHitCounter >= _shakeStartHitsAmount.x && 
                    _lastShake + _shakeInterval <= Time.time)
                {
                    StartCoroutine("Shake");
                }
            }
            else
            {
                if (!(GetComponent<Rigidbody>().velocity.magnitude < 0.02f) && _waitStart < 0.0f)
                {
                    _waitStart = Time.time;
                }
                else if (_timeToWait + _waitStart < Time.time)
                {
                    _playerControl = true;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    _waitStart = -1;
                }
            }
        }

        if(!_hitBullyOnUpdate)
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
                bully.gameObject == _lastBullyHit))
            {
                _lastBullyHit = bully.gameObject;
                if (LvlRef.CurrentStep >= _slowStartStep && !_isHittingBully)
                {
                    _slowHitCounter++;
                }
                if (LvlRef.CurrentStep >= _shakeStartStep && !_isHittingBully)
                {
                    _shakeHitCounter++;
                }

                _hitBullyOnUpdate = true;
                _isHittingBully = true;
            }

            if (LvlRef.CurrentStep >= LvlRef.TotalSteps)
            {
                LvlRef.EndGame();
            }
        }
    }
}