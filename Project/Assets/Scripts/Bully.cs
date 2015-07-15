using UnityEngine;

public class Bully : MonoBehaviour
{
    public static LevelSetup SceneManager;
    public int CircleNumber;

	// Use this for initialization
	void Start () {
	    SceneManager.OnRadiusChanged += Go;
	}

    public void Init()
    {
        transform.position = SceneManager.UnitCirclePosition(CircleNumber);
    }

    void Go()
    {
        Debug.Log("changed to " + SceneManager.CurrentRadius);
    }

	void Update () 
    {
        if (
            Vector3.Distance(SceneManager.UnitCirclePosition(CircleNumber), 
                            SceneManager.Player.transform.position) 
            < 
            SceneManager.CirclePiece + SceneManager.AttackTolerance)
        {
            var travel = (Vector3.Distance(SceneManager.UnitCirclePosition(CircleNumber), transform.position) +
                            (SceneManager.AttackSpeed * Time.deltaTime)) / Vector3.Distance(SceneManager.UnitCirclePosition(CircleNumber), SceneManager.Player.transform.position);

            transform.position = Vector3.Lerp(SceneManager.UnitCirclePosition(CircleNumber), SceneManager.Player.transform.position, travel);
        }
        else
        {
            if (Vector3.Distance(SceneManager.UnitCirclePosition(CircleNumber), transform.position) > SceneManager.Tolerance)
            {
                transform.position += (SceneManager.UnitCirclePosition(CircleNumber) - transform.position) *
                                                        SceneManager.AttackSpeed * Time.deltaTime;
            }
        }
	}
}
