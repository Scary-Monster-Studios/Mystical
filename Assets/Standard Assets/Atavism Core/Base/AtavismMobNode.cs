using UnityEngine;
using System.Collections;
using System;

namespace Atavism
{

    public delegate void DirectionChangeEventHandler(object sender, EventArgs args);

    public enum MovementState
    {
        NORMAL = 1,
        FLYING = 2,
        SWIMMING = 3
    }

    public class AtavismMobNode : AtavismObjectNode
    {

        protected long lastDirTimestamp = -1;
        protected Vector3 lastPosition;
        protected Vector3 lastDirection;
        protected float fallingSpeed = 0;
        protected bool isFalling = false;
        protected MovementState movementState = MovementState.NORMAL;
        // Used to make a mob keep facing a target
        protected long facing = -1;
        protected AtavismPathInterpolator pathInterpolator = null;

        /// <summary>
        ///   True if the system should call FMOD to notify about
        ///   position and velocity updates.  Velocity only matters
        ///   for doppler.
        /// </summary>
        protected bool positionalSoundEmitter = false;

        /// <summary>
        ///   Adjust the position gradually if it is less than MaxAdjustment, 
        ///   or just set it directly if it is more than MaxAdjustment
        /// </summary>
        const float MaxAdjustment = 3 * AtavismClient.OneMeter;
        /// <summary>
        ///   Converge on the destination at one meter per second
        /// </summary>
        const float AdjustVelocity = AtavismClient.OneMeter;

        public event DirectionChangeEventHandler DirectionChange;

        public AtavismMobNode(long oid, string name, AtavismWorldManager worldManager) :
                base(oid, name, ObjectNodeType.Npc, worldManager)
        {
            this.Targetable = true;
        }

        /// <summary>
        ///   Update the node
        /// </summary>
        /// <param name="timeSinceLastFrame">time since last frame (in seconds)</param>
        public override void Tick(float timeSinceLastFrame)
        {
            long now = AtavismTimeTool.CurrentTime;
         //   UnityEngine.Debug.LogError("Tick");

            // Set position based on direction vectors.
            //ComputePosition (now);
            //this.Position = ComputePosition (now);
            if (gameObject == null)
            {
                AtavismLogger.LogDebugMessage("MobNode.Tick gameObject is null: " + oid + ":" + OID.fromLong(oid).ToString());
                return;
            }
            if (parent != null)
            {
                this.Position = parent.transform.position;
            }
            else
            {
                this.Position = gameObject.transform.position;
            }


            base.Tick(timeSinceLastFrame);
#if DISABLED
            if (position != sceneNode.Position) {
                Vector3 posDelta = position - sceneNode.Position;
                float adjustDistance = timeSinceLastFrame * AdjustVelocity;
				if ((posDelta.Length > MaxAdjustment) ||
					(posDelta.Length <= adjustDistance))
					// If they are too far away, teleport them there
					// If they are close enough that we can adjust them there, do that
					sceneNode.Position = position;
				else {
					posDelta.Normalize();
					sceneNode.Position += adjustDistance * posDelta;
				}
			}
#endif
        }

        public override void SetDirLoc(long timestamp, Vector3 dir, Vector3 pos)
        {

            AtavismLogger.LogDebugMessage("SetDirLoc: dir for node Oid=" + this.Oid +" Name="+ this.Name+" dir= " + dir + " dl=" + dir.magnitude );
            if (timestamp <= lastDirTimestamp)
            {
                AtavismLogger.LogDebugMessage("Ignoring dirloc,since timestamps are too close " + timestamp + ", " + lastDirTimestamp + ".");
                return;
            }
            // timestamp is newer.
            lastDirTimestamp = timestamp;
            LastDirection = dir;
            // If the distance is greater than 5 metres, teleport the mob instantly, otherwise use sync offset to smooth out the syncing of positions
            float distance = Vector3.Distance(pos, Position);
            if (distance > 5 || distance < 0.1f || mobController == null)
            {
                SetLoc(timestamp, pos);
                //Position = pos;
            }
            else /*if (dir.magnitude < 0.5f)*/
            {
                //Debug.Log("Setting syncOffset with clients position: " + Position + " and given position: " + pos + " giving distance: " + distance);
                mobController.SyncOffset = pos - Position;
                mobController.SyncOffsetExpiration = Time.time + 1f;
            }
            lastPosition = pos;

           
        }

        public void StartJump()
        {
            mobController.LastJumpButtonTime = Time.time;
        }

        // Provide a way to return to the old behavior in which mobs
        // are supported by collision volumes.  Later, when we have
        // full mob pathing support, we can turn this off again.
        public static bool useMoveMobNodeForPathInterpolator = true;

       

        public void UpdateControllerDirection(Vector3 desiredDisplacement)
        {
            if (mobController != null)
                mobController.DesiredDisplacement = desiredDisplacement;
        }

        public void UpdateMobNodePosition(Vector3 position)
        {
            this.Position = position;
            lastPosition = position;
        }

        protected virtual void OnDirectionChange()
        {
            DirectionChangeEventHandler handler = DirectionChange;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public bool CanMove()
        {
            if (this.CheckBooleanProperty("world.nomove"))
                return false;
            return true;
        }

        public bool CanTurn()
        {
            if (this.CheckBooleanProperty("world.noturn"))
                return false;
            return true;
        }

        #region Properties

        /// <summary>
        /// This property is the movement direction vector last sent by the server.  It is used to interpolate
        /// the mob's position.
        /// 
        /// The Direction property on the Player class represents the direction as set by the player.
        /// </summary>
        public Vector3 LastDirection
        {
            get
            {
                return lastDirection;
            }
            set
            {
                if (value != lastDirection)
                {
                    lastDirection = value;
                    OnDirectionChange();
                }

                if (!(this is AtavismPlayer) && mobController != null)
                {
                    mobController.LastDirection = lastDirection;
                }
            }
        }

        /// <summary>
        /// This property represents the direction vector that comes from the player, and is used
        /// to interpolate the position of the player's avatar.
        /// 
        /// MobNode's LastDirection property is the last direction provided by the server.
        /// </summary>
        public virtual Vector3 Direction
        {
            get
            {
                return LastDirection;
            }
            set
            {
                Debug.Log("Set dir timestamp to : " + AtavismTimeTool.CurrentTime);
                lastDirTimestamp = AtavismTimeTool.CurrentTime;
                lastPosition = this.Position;
                LastDirection = value;
            }
        }

        public virtual bool IsFalling
        {
            get
            {
                return isFalling;
            }
            set
            {
                isFalling = value;
            }
        }

        public virtual float FallingSpeed
        {
            get
            {
                return fallingSpeed;
            }
            set
            {
                fallingSpeed = value;
            }
        }

        public virtual MovementState MovementState
        {
            get
            {
                return movementState;
            }
        }

        public AtavismPathInterpolator Interpolator
        {
            get
            {
                return pathInterpolator;
            }
            set
            {
                pathInterpolator = value;
                if (mobController != null)
                    mobController.PathInterpolator = value;
            }
        }

        public virtual bool PositionalSoundEmitter
        {
            get
            {
                return positionalSoundEmitter;
            }
            set
            {
                positionalSoundEmitter = value;
            }
        }

        public long Facing
        {
            get
            {
                return facing;
            }
            set
            {
                facing = value;
            }
        }

        #endregion
    }
}