using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_CameraRotate : MonoBehaviour {

	public Transform target;									// Target for the rotation
	public float targetOffset = 1.0f;							// Vertical offset for the target, often 1.0 for the middle of a 2m tall character
	public float speed = 10.0f;									// Speed of the default rotation

	// Update is called once per frame
	void Update () {
		transform.RotateAround (target.position + new Vector3 (0, 1, 0), Vector3.up, speed * Time.deltaTime);	// Rotate around the point
		transform.LookAt (target.position + new Vector3 (0, targetOffset, 0));									// Look at the position we want
	}

	/// <summary>
	/// Sets the speed value
	/// </summary>
	/// <param name="newValue">New value.</param>
	public void SetSpeed(float newValue){
		speed = newValue;
	}

}