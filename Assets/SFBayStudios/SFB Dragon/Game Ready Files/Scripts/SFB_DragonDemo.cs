using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DragonDemo : MonoBehaviour {

	Animator animator;

	void Start(){
		animator = GetComponent<Animator> ();
	}

	public void UpdateLocomotion(float value){
		animator.SetFloat ("locomotion", value);
	}
}
