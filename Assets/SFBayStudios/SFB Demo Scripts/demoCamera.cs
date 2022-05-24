using UnityEngine;
using System.Collections;

public class demoCamera : MonoBehaviour {

	public float speed = 10.0f;
	public Transform target;
	public float targetOffset = 1.0f;
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(target.position + new Vector3(0,1,0), Vector3.up, speed * Time.deltaTime);
		transform.LookAt(target.position + new Vector3(0,targetOffset,0));
	}

	public void SetSpeed(float newValue){
		speed = newValue;
	}
}