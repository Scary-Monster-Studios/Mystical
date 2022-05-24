using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DemoImp : MonoBehaviour {

	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	public void Locomotion(float newValue){
		animator.SetFloat("locomotion", newValue);
	}

	public void Turning(float newValue){
		animator.SetFloat("turning", newValue);
	}
}
