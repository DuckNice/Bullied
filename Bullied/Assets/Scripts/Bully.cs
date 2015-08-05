using UnityEngine;

public class Bully : MonoBehaviour
{
    public static LevelSetup SceneManager;
    public int CircleNumber;

    public void Init()
    {
        LevelSetup.OnRadiusChanged += CalculateNewPosition;
        transform.position = SceneManager.UnitCirclePosition(CircleNumber);
        CalculateNewRotation();
    }

    void CalculateNewPosition(int CurrentStep)
    {
        SceneManager.UnitCirclePosition(CircleNumber);
    }

    void CalculateNewRotation()
    {
        gameObject.transform.rotation = Quaternion.identity;

        Vector2 u = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        u.Normalize();
        Vector2 v = Vector2.right;

        float dot = (v.x * u.x) + (v.y * u.y);
        float det = (v.x * u.y) - (v.y * u.x);

        float angle = Mathf.Atan2(det, dot);

        gameObject.transform.Rotate(Vector3.forward, Mathf.Rad2Deg * angle);
    }

	void Update ()
    {
	    if (LevelSetup.GameOn)
	    {
	        if (
	            Vector3.Distance(SceneManager.UnitCirclePosition(CircleNumber),
	                SceneManager.Player.transform.position)
	            <
	            SceneManager.CirclePiece + SceneManager.AttackTolerance)
	        {
	            var travel = (Vector3.Distance(SceneManager.UnitCirclePosition(CircleNumber), transform.position) +
	                          (SceneManager.AttackSpeed*Time.deltaTime))/
	                         Vector3.Distance(SceneManager.UnitCirclePosition(CircleNumber),
	                             SceneManager.Player.transform.position);

	            transform.position = Vector3.Lerp(SceneManager.UnitCirclePosition(CircleNumber),
	                SceneManager.Player.transform.position, travel);
	        }
	        else
	        {
	            if (Vector3.Distance(SceneManager.UnitCirclePosition(CircleNumber), transform.position) >
	                SceneManager.Tolerance)
	            {
	                transform.position += (SceneManager.UnitCirclePosition(CircleNumber) - transform.position)*
	                                      SceneManager.AttackSpeed*Time.deltaTime;
	            }
	        }

            CalculateNewRotation();
	    }
    }
}