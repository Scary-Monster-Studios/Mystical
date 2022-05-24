using UnityEngine;
using System.Collections;

public class SFBDemo_StayWithTarget : MonoBehaviour {

	public GameObject target;													// Target object
	public float verticalOffset = 0.0f;											// Vertical offset

	void LateUpdate () {														// Changes made in LateUpdate
		Vector3 newPosition = new Vector3();									// Create new vector3
		newPosition = target.transform.position;								// Set position to target position
		newPosition.y += verticalOffset;										// Apply offset
		transform.position = newPosition;										// Set Vector3 Value
	}
}