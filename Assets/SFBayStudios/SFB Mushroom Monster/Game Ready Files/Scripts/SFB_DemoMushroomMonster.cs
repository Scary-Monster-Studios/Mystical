using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DemoMushroomMonster : MonoBehaviour {

	public Transform castSpot;
	public GameObject castPrefab;
	public Animator animator;

	public void CastMagic(){
		GameObject newSpell = Instantiate (castPrefab, castSpot.position, castSpot.rotation);
		Destroy (newSpell, 5.0f);
	}

	public void UpdateLocomotion(float newValue){
		animator.SetFloat ("locomotion", newValue);
	}
}
	