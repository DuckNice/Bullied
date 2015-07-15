using UnityEngine;

public class Bully : MonoBehaviour
{
    public static LevelSetup SceneManager;
    public int CircleNumber;

    public void Init()
    {
        SceneManager.OnRadiusChanged += CalculateNewPosition;
        transform.position = SceneManager.UnitCirclePosition(CircleNumber);
        transform.Rotate(new Vector3(0, 0, (Mathf.Rad2Deg * SceneManager.CalculateRadians(CircleNumber))));
    }

    void CalculateNewPosition()
    {
        SceneManager.UnitCirclePosition(CircleNumber);
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
	    }
    }
}