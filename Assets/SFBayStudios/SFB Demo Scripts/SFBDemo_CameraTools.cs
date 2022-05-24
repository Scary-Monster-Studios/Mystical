using UnityEngine;
using System.Collections;

public class SFBDemo_CameraTools : MonoBehaviour {

	public GameObject rotateTarget;											// Target object
	public GameObject[] rotateTargets;										// Array of targets to choose from
	public float rotateTargetOffset = 1.0f;									// Offset for camera direction
	public float rotateSpeed = 10.0f;										// Speed of rotation
	
	void Update () {
		if (!rotateTarget.activeSelf) {											// If the current target isn't active
			rotateTarget = FindNewTarget();										// Get a new target
		}
		// Rotate around the target, with the fixed y position
		transform.RotateAround (rotateTarget.transform.position + new Vector3 (0, 1, 0), Vector3.up, rotateSpeed * Time.deltaTime);
		// Look at the target, with the fixed y Position
		transform.LookAt (rotateTarget.transform.position + new Vector3 (0, rotateTargetOffset, 0));
	}

	/// <summary>
	/// Will return the first active object in the targets array
	/// </summary>
	/// <returns>The new target.</returns>
	GameObject FindNewTarget(){
		for (int i = 0; i < rotateTargets.Length; i++){							// For each targets object
			if (rotateTargets [i].activeSelf) {									// If it is active
				return rotateTargets [i];										// Return the game object
			}
		}
		return null;															// Return null
	}

	/// <summary>
	/// Sets the speed value
	/// </summary>
	/// <param name="newSpeed">New speed.</param>
	public void SetRotationSpeed(float newSpeed){
		rotateSpeed = newSpeed;													// Set value
	}

	/// <summary>
	/// Sets the camera y position
	/// </summary>
	/// <param name="newValue">New value.</param>
	public void SetCameraHeight(float newValue){
		Vector3 newPosition1 = new Vector3 (transform.position.x, newValue, transform.position.z);	// Create a new Vector3
		transform.position = newPosition1;															// Set position
	}
}