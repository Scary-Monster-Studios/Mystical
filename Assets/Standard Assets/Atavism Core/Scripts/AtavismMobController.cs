using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{

    public abstract class AtavismMobController : MonoBehaviour
    {

        #region Fields
        protected long oid;
        public bool isPlayer = false;

        protected float lastLocTimestamp = -1;
        protected float lastDirTimestamp = -1;
        protected Vector3 lastPosition;
        protected Vector3 lastDirection;
        protected Vector3 syncOffset = Vector3.zero;
        protected float syncOffsetExpiration = -1;

        protected Vector3 desiredDisplacement = Vector3.zero;

        protected float runSpeed = 6.0f;
        // Last time the jump button was clicked down
        protected float lastJumpButtonTime = -10.0f;

        protected long target = -1;
        protected AtavismPathInterpolator pathInterpolator = null;

        // Are we jumping? (Initiated with jump button and not grounded yet)
        protected bool jumping = false;
        protected bool jumpingReachedApex = false;
        protected bool followTerrain = true;
        public LayerMask groundLayers = 0;

        protected float rotatingDirection = 0;

        protected List<int> waterRegions = new List<int>();

        protected int movementState = 1;

        public float nameHeight = 2f;

        protected GameObject mount;

        #endregion Fields

        // Use this for initialization
        void Start()
        {
        }

        void ObjectNodeReady()
        {
            GetComponent<AtavismNode>().SetMobController(this);
        }

        // Update is called once per frame
        void Update()
        {
        }

        public abstract Vector3 MoveMob();

        public abstract Vector3 MovePlayer();

        public abstract void PlayAnimation(string animationName, float length);
        public abstract void PlayAnimationFloat(string animationName, float value, float length, float valueAfter);
        public abstract void PlayAnimationTrigger(string animationName);
        public abstract void PlayAnimationInt(string animationName, int value, float length, int valueAfter);
        public abstract void PlayMeleeAttackAnimation(string attackType, string result);
        public abstract void PlayMeleeRecoilAnimation(string result);

        public abstract void WaterRegionEntered(int id);
        public abstract void WaterRegionLeft(int id);

        #region Properties
        public long Oid
        {
            get
            {
                return oid;
            }
            set
            {
                oid = value;
            }
        }

        public bool IsPlayer
        {
            get
            {
                return isPlayer;
            }
            set
            {
                isPlayer = value;
                if (isPlayer)
                {
                    gameObject.layer = AtavismClient.Instance.playerLayer;
                    gameObject.tag = AtavismClient.Instance.playerTag;
                }
            }
        }

        public Vector3 LastDirection
        {
            get
            {
                return lastDirection;
            }
            set
            {
                lastDirection = value;
            }
        }

        public Vector3 SyncOffset
        {
            get
            {
                return syncOffset;
            }
            set
            {
                syncOffset = value;
            }
        }

        public float SyncOffsetExpiration
        {
            get
            {
                return syncOffsetExpiration;
            }
            set
            {
                syncOffsetExpiration = value;
            }
        }

        public Vector3 DesiredDisplacement
        {
            get
            {
                return desiredDisplacement;
            }
            set
            {
                desiredDisplacement = value;
            }
        }

        public float RunSpeed
        {
            get
            {
                return runSpeed;
            }
            set
            {
                runSpeed = value;
            }
        }

        public long Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }

        public AtavismPathInterpolator PathInterpolator
        {
            set
            {
                pathInterpolator = value;
            }
        }

        public float LastJumpButtonTime
        {
            set
            {
                lastJumpButtonTime = value;
                jumping = true;
            }
        }

        public bool Jumping
        {
            set
            {
                jumping = value;
            }
        }

        public float RotatingDirection
        {
            set
            {
                rotatingDirection = value;
            }
            get
            {
                return rotatingDirection;
            }
        }

        public bool FollowTerrain
        {
            set
            {
                followTerrain = value;
            }
            get
            {
                return followTerrain;
            }
        }

        public int MovementState
        {
            get
            {
                return movementState;
            }
        }

        public GameObject Mount
        {
            get
            {
                return mount;
            }
        }
        #endregion Properties
    }
}