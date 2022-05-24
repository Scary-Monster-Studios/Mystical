using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_LightRandomHeight : MonoBehaviour {

	public Vector2 heightRange = new Vector2(0.2f, 0.4f);
	public Light lightComp;
	public float lightMaxIntensity = 5.0f;

	void OnEnable(){
		if (lightComp.enabled) {
			lightComp.intensity = 0;
		}
	}

	void Update(){
		Vector3 newPos = new Vector3 (transform.position.x, Random.Range (heightRange.x, heightRange.y), transform.position.z);
		transform.position = newPos;
		lightComp.intensity = Mathf.MoveTowards (lightComp.intensity, lightMaxIntensity, Time.deltaTime);
	}
}
