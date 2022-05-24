using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DragonHeight : MonoBehaviour {

	public bool isFlying = false;
	public float flyingHeight = 50.0f;
	public float groundHeight = 0.0f;
	public float minFlyHeight = 10.0f;
	Animator animator;
	public int animatorHash1;
	public int animatorHash2;
	public int animatorHash3;
	public bool isDying = false;
	public int currentHash;
	public float normalizedTime;
	public float deathGroundTime = 0.8f;

	void Start () {
		animator = GetComponent<Animator> ();
		animatorHash1	= Animator.StringToHash("Base Layer.Air.FlyDeath01");
		animatorHash2	= Animator.StringToHash("Base Layer.Air.FlyDeath02");
		animatorHash3	= Animator.StringToHash("Base Layer.Air.FlyDeath03");
	}
	
	void Update () {
		AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo (0);
		normalizedTime = animatorStateInfo.normalizedTime;
		currentHash = animatorStateInfo.fullPathHash;

		if (currentHash != animatorHash1 && currentHash != animatorHash2 && currentHash != animatorHash3) {
			isDying = false;
			if (isFlying) {
				if (transform.position.y < minFlyHeight) {
					Vector3 newHeight = new Vector3 (transform.position.x, flyingHeight, transform.position.z);
					transform.position = newHeight;
				}
			}
		} else {
			isDying = true;
			if (currentHash == animatorHash1 || currentHash == animatorHash2)
			{
				if (transform.position.y < minFlyHeight)
					animator.SetTrigger("flyDeathEnd");
			}
		}

		if (isDying && currentHash == animatorHash3 && normalizedTime >= deathGroundTime) {
			Vector3 newHeight = new Vector3 (transform.position.x, 0, transform.position.z);
			transform.position = newHeight;
			isDying = false;
		}
	}

	public void StartFlying(){
		isFlying				= true;
		Vector3 newHeight = new Vector3 (0, flyingHeight, 0);
		transform.position = newHeight;
	}

	public void StartGround(){
		isFlying				= false;
		Vector3 newHeight = new Vector3 (0, groundHeight, 0);
		transform.position = newHeight;
	}
}
