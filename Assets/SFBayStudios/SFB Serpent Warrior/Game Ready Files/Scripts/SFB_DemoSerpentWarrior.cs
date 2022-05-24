using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DemoSerpentWarrior : MonoBehaviour {

	private Animator animator;
	public Transform leftHandSpawnPos;
	public Transform rightHandSpawnPos;
	public Transform dustSpot;
	public GameObject rightHandParticle;
	public GameObject leftHandParticle;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	public void Locomotion(float newValue){
		animator.SetFloat ("locomotion", newValue);
	}

	public void RightHandCast(){
		GameObject newParticle = Instantiate(rightHandParticle, dustSpot.position, Quaternion.identity);
		newParticle.transform.rotation = dustSpot.rotation;
		newParticle.transform.position = dustSpot.position;
		Destroy(newParticle, 5.0f);
	}

	public void LeftHandCast(){
		GameObject newParticle = Instantiate(leftHandParticle, leftHandSpawnPos.position, Quaternion.identity);
		newParticle.transform.rotation = leftHandSpawnPos.rotation;
		newParticle.transform.position = leftHandSpawnPos.position;
		Destroy(newParticle, 5.0f);
	}
}
