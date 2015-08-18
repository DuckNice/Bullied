using UnityEngine;

public class Bully : MonoBehaviour
{
    public int CircleNumber;

    public void Init()
    {
        LevelSetup.OnRadiusChanged += CalculateNewPosition;
        transform.position = LevelSetup.instance.UnitCirclePosition(CircleNumber);
        CalculateNewRotation();
    }

    void CalculateNewPosition(int CurrentStep)
    {
        LevelSetup.instance.UnitCirclePosition(CircleNumber);
    }

    
	void Update ()
    {
	    if (LevelSetup.GameOn)
	    {
	        if (
                Vector3.Distance(LevelSetup.instance.UnitCirclePosition(CircleNumber),
                    LevelSetup.instance.Player.transform.position)
	            <
                LevelSetup.instance.CirclePiece + LevelSetup.instance.AttackTolerance)
	        {
                var travel = (Vector3.Distance(LevelSetup.instance.UnitCirclePosition(CircleNumber), transform.position) +
                              (LevelSetup.instance.AttackSpeed * Time.deltaTime)) /
                             Vector3.Distance(LevelSetup.instance.UnitCirclePosition(CircleNumber),
                                 LevelSetup.instance.Player.transform.position);

                transform.position = Vector3.Lerp(LevelSetup.instance.UnitCirclePosition(CircleNumber),
                    LevelSetup.instance.Player.transform.position, travel);
	        }
	        else
	        {
                if (Vector3.Distance(LevelSetup.instance.UnitCirclePosition(CircleNumber), transform.position) >
                    LevelSetup.instance.Tolerance)
	            {
                    transform.position += (LevelSetup.instance.UnitCirclePosition(CircleNumber) - transform.position) *
                                          LevelSetup.instance.StepSpeed * Time.deltaTime;
	            }
	        }

            CalculateNewRotation();
	    }
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

}