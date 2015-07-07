using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private bool playerControl = true;
	private Vector3 direction = new Vector3(0, 0, 0);
	private Vector3 hitPosition = new Vector3(0, 0, 0);
	private GameObject lastBullyHit;
	public float playerSpeed;
	private float currentSpeed;
	public float fallSpeed;
	public float centerTolerance;
	public LevelSetup refe;
	public int hurtDivisor = 2;

	public bool HitBully(Vector2 bullyPosition, bool sameBully){
		if(playerControl || !sameBully){
			playerControl = false;

			direction = Vector3.zero - transform.position;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().velocity = (direction * fallSpeed);
			hitPosition = bullyPosition;

			return true;
		}

		return false;
	}

	void Start(){
		currentSpeed = playerSpeed;
	}

	// Update is called once per frame
	void Update () {
		if (playerControl) {
			float x = Input.GetAxis ("Horizontal");
			float y = Input.GetAxis ("Vertical");
			Vector3 offset = (new Vector3(x, y, 0) * playerSpeed / ((refe.step + hurtDivisor) / hurtDivisor) * Time.deltaTime);

			transform.position += offset;
		}
		else {

			if(/*Vector3.Distance(rigidbody.position, Vector3.zero) < centerTolerance || */GetComponent<Rigidbody>().velocity.magnitude < 0.5f){
				playerControl = true;
				GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
	}

	void OnTriggerStay(Collider bully)
	{
		if(HitBully (new Vector2(bully.transform.position.x, bully.transform.position.y), bully.gameObject == lastBullyHit)){
			lastBullyHit = bully.gameObject;
		}
	}
}