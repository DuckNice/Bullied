using UnityEngine;

public class Player : MonoBehaviour
{
    public float CenterTolerance;
    private Vector3 _direction = new Vector3(0, 0, 0);
    public float FallSpeed;
    public int HurtDivisor = 2;
    private GameObject _lastBullyHit;
    private bool _playerControl = true;
    public float PlayerSpeed;
    public LevelSetup Refe;

    public bool HitBully(Vector2 bullyPosition, bool sameBully)
    {
        if (_playerControl || !sameBully)
        {
            _playerControl = false;

            _direction = Vector3.zero - transform.position;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().velocity = (_direction*FallSpeed);

            return true;
        }

        return false;
    }


    // Update is called once per frame
    private void Update()
    {
        if (_playerControl)
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");
            var offset = (new Vector3(x, y, 0)*PlayerSpeed/((Refe.Step + HurtDivisor)/HurtDivisor)*Time.deltaTime);

            transform.position += offset;
        }
        else
        {
            if ( /*Vector3.Distance(rigidbody.position, Vector3.zero) < centerTolerance || */
                GetComponent<Rigidbody>().velocity.magnitude < 0.5f)
            {
                _playerControl = true;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    private void OnTriggerStay(Collider bully)
    {
        if (HitBully(new Vector2(bully.transform.position.x, bully.transform.position.y),
            bully.gameObject == _lastBullyHit))
        {
            _lastBullyHit = bully.gameObject;
        }
    }
}