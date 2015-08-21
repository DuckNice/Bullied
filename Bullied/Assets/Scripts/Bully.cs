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
                float speed = LevelSetup.instance.AttackSpeed * Time.deltaTime;
                float distx = transform.position.x - LevelSetup.instance.Player.transform.position.x;
                float disty = transform.position.y - LevelSetup.instance.Player.transform.position.y;

                Vector3 travel = new Vector3(distx, disty, 0);

                transform.position -= travel.normalized * speed;
                LevelSetup.instance.BullyStepped[CircleNumber] = true;
	        }
	        else
	        {
                if (Vector3.Distance(LevelSetup.instance.UnitCirclePosition(CircleNumber), transform.position) >
                    LevelSetup.instance.Tolerance)
	            {
                    if(LevelSetup.instance.BullyStepped[CircleNumber]){
                        transform.position += (LevelSetup.instance.UnitCirclePosition(CircleNumber) - transform.position) *
                                              LevelSetup.instance.WalkSpeed * Time.deltaTime;
	                }
                    else
                    {
                        transform.position += (LevelSetup.instance.UnitCirclePosition(CircleNumber) - transform.position) *
                                              LevelSetup.instance.StepSpeed * Time.deltaTime;
                        if (Vector3.Distance(LevelSetup.instance.UnitCirclePosition(CircleNumber), transform.position) <
                            LevelSetup.instance.StepTolerance)
                        {
                            LevelSetup.instance.BullyStepped[CircleNumber] = true;
                        }
                    }
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