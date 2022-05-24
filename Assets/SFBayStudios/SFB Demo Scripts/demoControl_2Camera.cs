using UnityEngine;
using System.Collections;

public class demoControl_2Camera : MonoBehaviour {

	public GameObject cameraObject;
	public GameObject cameraObject2;
	
	public void SetCameraHeight(float newValue){
		Debug.Log ("SetCameraHeight(" + newValue + ")");
		if (cameraObject) {
			Vector3 newPosition1 = new Vector3 (cameraObject.transform.position.x, newValue, cameraObject.transform.position.z);
			cameraObject.transform.position = newPosition1;
			Debug.Log ("Camera 1 Position: " + cameraObject.transform.position);
		}
		if (cameraObject2) {
			Vector3 newPosition2 = new Vector3 (cameraObject2.transform.position.x, newValue, cameraObject2.transform.position.z);
			cameraObject2.transform.position = newPosition2;
		}
	}

	public void SetTimescale(float newValue){
		Time.timeScale = newValue;
	}
}
