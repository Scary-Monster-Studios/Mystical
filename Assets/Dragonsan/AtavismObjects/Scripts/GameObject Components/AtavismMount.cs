using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace Atavism
{
    public class AtavismMount : MonoBehaviour
    {

        public Transform mountSocket;
        public string characterSocket;
        public string riderAnimation = "Mount";
        long riderOid;

        public AnimationClip idleAnimation;
        public AnimationClip walkAnimation;
        public AnimationClip runAnimation;
        public AnimationClip jumpPoseAnimation;
        public AnimationClip swimIdleAnimation;
        public AnimationClip swimAnimation;
        public float walkMaxAnimationSpeed = 0.75f;
        public float trotMaxAnimationSpeed = 1.0f;
        public float runMaxAnimationSpeed = 1.0f;
        public float jumpAnimationSpeed = 1.15f;
        public float landAnimationSpeed = 1.0f;
        private bool useAnimator = false;
        public float runThreshold = 2.5f;
        bool jumping = false;
        bool jumpingReachedApex = false;
        int movementState = 0;
        public float speed = 0F;
        private GameObject mountCoordEffect;
        private GameObject dismountCoordEffect;
        float destroyTime = -1;
        private Animator _animator;
        private Animation _animation;
        public  MobController3D mc3d = null;
        public LayerMask groundMask;

        private CharacterController controller;
        // Use this for initialization
        void Start()
        {
            _animation  = (Animation)GetComponent("Animation");
            if (!_animation)
            {
                useAnimator = true;
            }
            _animator = (Animator)GetComponentInChildren(typeof(Animator));
            
            foreach (AtavismMobNode mNode in ClientAPI.WorldManager.GetMobNodes())
            {
                if (mNode.GameObject != null && mNode.GameObject.GetComponent<Collider>() != null && GetComponent<Collider>() != mNode.GameObject.GetComponent<Collider>())
                    Physics.IgnoreCollision(GetComponent<Collider>(), mNode.GameObject.GetComponent<Collider>());
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (destroyTime != -1 && destroyTime < Time.time)
            {
                ClientAPI.GetObjectNode(riderOid).GameObject.transform.parent = null;
                ClientAPI.GetObjectNode(riderOid).Parent = null;
                DestroyImmediate(gameObject);
            }
            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
            }

            // ANIMATION sector
            if (!useAnimator)
            {
                 _animation = GetComponentInChildren<Animation>();
                //Debug.Log("Using animation for mob: " + name);
                if (jumping)
                {
                    if (!jumpingReachedApex)
                    {
                        _animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
                        _animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
                        _animation.CrossFade(jumpPoseAnimation.name);
                    }
                    else
                    {
                        _animation[jumpPoseAnimation.name].speed = -landAnimationSpeed;
                        _animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
                        _animation.CrossFade(jumpPoseAnimation.name);
                    }
                }
                else if (movementState == 2)
                {
                    if (controller.velocity.sqrMagnitude > 0.1)
                    {
                        _animation[swimAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
                        _animation.CrossFade(swimAnimation.name);
                    }
                    else
                    {
                        _animation.CrossFade(swimIdleAnimation.name);
                    }
                }
                else
                {
                    if (controller.velocity.sqrMagnitude > 0.1)
                    {
                        if (controller.velocity.magnitude > runThreshold)
                        {
                            _animation[runAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
                            _animation.CrossFade(runAnimation.name);
                        }
                        else
                        {
                            _animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
                            _animation.CrossFade(walkAnimation.name);
                        }
                    }
                    else
                    {
                        _animation.CrossFade(idleAnimation.name);
                    }
                }
            }
            else if (useAnimator)
            {
                if (_animator == null)
                {
                    _animator = (Animator)GetComponentInChildren(typeof(Animator));
                }
                // Debug.Log("Using animator for mob: " + name);
                if (_animator)
                {
                    _animator.SetInteger("MovementState", movementState);
                    speed = controller.velocity.magnitude;
                    _animator.SetFloat("Speed", speed);
                    Profiler.BeginSample("AMMC3D Update Diraction");
                    
                         if (controller.velocity.magnitude > 0.1f && mc3d.RotatingDirection == 0)
                         {
                             float dot = Vector3.Dot(transform.forward, mc3d.Movement.normalized);
                             Vector3 cross = Vector3.Cross(transform.forward, mc3d.Movement.normalized);
                             if (dot > 0.8 && cross.y < 0.28 && cross.y > -0.28)
                             {//forward
                                 _animator.SetFloat("Direction", 0);
                             }
                             else if (dot > 0.3 && (cross.y > 0.28 || cross.y < -0.28))
                             {//forward Right|Left
                                 _animator.SetFloat("Direction", cross.y < 0 ? 45 : -45);
                             }
                             else if (dot < 0.3 && dot > -0.3 && (cross.y > 0.28 || cross.y < -0.28))
                             {//Right|Left
                                 _animator.SetFloat("Direction", cross.y < 0 ? 90 : -90);
                             }
                             else if (dot < -0.3 && (cross.y > 0.28 || cross.y < -0.28))
                             {//Backwards Right|Left
                                 _animator.SetFloat("Direction", cross.y < 0 ? 135 : -135);
                             }
                             else if (dot < -0.3 && cross.y < 0.28 && cross.y > -0.28)
                             {//Backwards
                                 _animator.SetFloat("Direction", 180);
                             }
                         }
                         else
                         {
                             float dot = Vector3.Dot(transform.forward, mc3d.Movement.normalized);
                             _animator.SetFloat("Direction", dot < -0.4 ? 180 : 0);
                             //_animator.SetFloat ("Direction", 0);
                          //   _animator.SetFloat("RotatingDirection", mc3d.RotatingDirection);
                         }
                    Profiler.EndSample();
                    if (jumping)
                    {
                        _animator.SetBool("Jump", true);
                    }
                    else
                    {
                        _animator.SetBool("Jump", false);
                    }
                    
                    RaycastHit hit;

                    if (Physics.Raycast(transform.position + new Vector3(0.0f, 0.5f, 0.0f), Vector3.down, out hit, 1.5f, groundMask))
                    {
                        Quaternion newRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                        _animator.transform.rotation = Quaternion.Slerp(_animator.transform.rotation, newRot, Time.deltaTime * 15f);
                    
                    }
                }
            }
        }

        public void SetRider(long rider)
        {
            this.riderOid = rider;
            StartMount();
        }

        public bool IsGrounded(CollisionFlags collisionFlags)
        {
            return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
        }

        public void SetJumping(bool jumping)
        {
            this.jumping = jumping;
        }

        public void SetJumpingReachedApex(bool jumpingReachedApex)
        {
            this.jumpingReachedApex = jumpingReachedApex;
        }

        public void StartMount()
        {
            if (mountCoordEffect != null)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("sourceOID", OID.fromLong(riderOid));
                CoordinatedEffectSystem.ExecuteCoordinatedEffect(mountCoordEffect.name, props);
            }
        }

        public bool StartDismount()
        {
            if (dismountCoordEffect != null)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("sourceOID", OID.fromLong(riderOid));
                CoordinatedEffectSystem.ExecuteCoordinatedEffect(dismountCoordEffect.name, props);
                destroyTime = Time.time + 1.0f;
                return true;
            }
            return false;
        }

        public void despawn()
        {
            
        }
    }
}