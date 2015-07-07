using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSetup : MonoBehaviour{
	public GameObject player;
	public List<float> unitsScale = new List<float>();
	private List<GameObject> unitPrefabs = new List<GameObject> ();
	public GameObject unitPrefab;
	public int steps = 4;
	public float radius = 6.0f;
	private float currentRadius;
	[HideInInspector]
	public int step = 0;
	private float lengthOfStep;
	public float stepSpeed;
	private bool stepping = false;
	private float nextStep;
	public float timeBetweenSteps;
	private float radiansEachUnit;
	public float attackSpeed;
	private float circlePiece;
	public float attackTolerance;
	public float tolerance = 0.02f;

	void Start (){
		lengthOfStep = radius / steps;
		nextStep = timeBetweenSteps;
		currentRadius = radius;
		float radians = 0;
		radiansEachUnit = (2 * Mathf.PI) / unitsScale.Count;
		calculateCirclePiece ();

		for(int i = 0; i < unitsScale.Count; i++){
			GameObject unit = Instantiate(unitPrefab) as GameObject;

			unit.transform.position = unitPosition(i);
			unit.transform.Rotate(new Vector3(0, 0, (Mathf.Rad2Deg * radians)));

			unitPrefabs.Add (unit);

			radians += radiansEachUnit;
		}
	}


		// Update is called once per frame
	void Update (){
		if(Time.time > nextStep){
			stepping = true;
			step++;
			nextStep += timeBetweenSteps;
		}

		if (stepping){
			currentRadius -= (stepSpeed * Time.deltaTime);
			calculateCirclePiece ();
		}

		for (int i = 0; i < unitPrefabs.Count; i++){
			if(Vector3.Distance(unitPosition(i), player.transform.position) < circlePiece + attackTolerance){
				float travel = (Vector3.Distance(unitPosition(i), unitPrefabs[i].transform.position) + (attackSpeed * Time.deltaTime)) / Vector3.Distance(unitPosition(i), player.transform.position);

				unitPrefabs[i].transform.position = Vector3.Lerp(unitPosition(i), player.transform.position, travel);
			}
			else{
				if (stepping){
					unitPrefabs [i].transform.position = unitPosition (i);
				}
				else if(Vector3.Distance(unitPosition(i), unitPrefabs[i].transform.position) > tolerance){
					unitPrefabs[i].transform.position += (unitPosition(i) - unitPrefabs[i].transform.position) * attackSpeed * Time.deltaTime;
				}
			}
		}

		if (currentRadius <= radius - (lengthOfStep * step)){
			stepping = false;
		}

		if(step >= steps){
			Invoke("EndGame", 0.1f);
		}
	}

	void EndGame()
	{
		Application.Quit();
		
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}


	Vector3 unitPosition(int index){
		float radians = radiansEachUnit * index;

		float x = Mathf.Cos(radians) * currentRadius;
		float y = Mathf.Sin(radians) * currentRadius;
		
		return new Vector3 (x, y, 0);
	}

	void calculateCirclePiece(){
		circlePiece = ((currentRadius * 2) * Mathf.PI) / (360 / (Mathf.Rad2Deg * radiansEachUnit));
	}
}