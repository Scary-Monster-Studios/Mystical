using UnityEngine;
using System.Collections;
namespace Atavism
{

    public class AtavismPlayer : AtavismMobNode
    {

        public DirUpdate dirUpdate;
        public OrientationUpdate orientUpdate;
        public float lastDirSent;
        public float lastOrientSent;
        public float lastUpdateSent;
        public float lastUpdateMovmentSent;

        public long updateCount = 0;
        // Our version of direction
        Vector3 direction;

        // Underwater state
        //private bool underwater = false;

        // The input controller component on the players character object
        private AtavismInputController inputController;

        public struct DirUpdate
        {
            public bool dirty;
            public float timestamp; // in local time
            public Vector3 direction;
            public Vector3 position;
        }

        public struct OrientationUpdate
        {
            public bool dirty;
            public Quaternion orientation;
        }

        public AtavismPlayer(long oid, string name, AtavismWorldManager worldManager) :
                base(oid, name, worldManager)
        {
            // force object type to User for the player
            objType = ObjectNodeType.User;

            orientUpdate.orientation = Quaternion.identity; //gameObject.transform.rotation;
            dirUpdate.dirty = false;
            orientUpdate.dirty = false;
            this.Targetable = false;
            lastUpdateSent = Time.realtimeSinceStartup;
            AtavismLogger.LogInfoMessage("Created player object");
        }

        public void SetDirection(Vector3 dir, Vector3 pos, float now)
        {
            if (dir != Direction)
            {
                //UnityEngine.Debug.Log ("In Player.SetDirection, marking direction dirty: dir " + dir + " lastdir: " + Direction);
                dirUpdate.timestamp = now;
                dirUpdate.direction = dir;
                dirUpdate.position = pos;
                dirUpdate.dirty = true;
            }
            OnDirectionChange();
            direction = dir;
        }

       

        public void UpdatePlayerPosition(Vector3 position)
        {
            //this.Position = position;
            //lastPosition = position;
        }

        public override Quaternion Orientation
        {
            get
            {
                return base.Orientation;
            }
            set
            {
                if (value != orientUpdate.orientation)
                {
                    orientUpdate.orientation = value;
                    orientUpdate.dirty = true;
                   // UnityEngine.Debug.Log("Setting orientUpdate to dirty");
                }
                base.Orientation = value;
            }
        }

        /// <summary>
        /// This property represents the direction vector that comes from the player, and is used
        /// to interpolate the position of the player's avatar.
        /// 
        /// MobNode's LastDirection property is the last direction provided by the server.
        /// </summary>
        public override Vector3 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                if (value != direction)
                {
                    SetDirection(value, this.Position, AtavismTimeTool.CurrentTime);
                }
            }
        }

        /// <summary>
        ///   Invoke the base SetDirLoc, and then the dirUpdate fields
        ///   to be consistent with the new position and direction
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="dir"></param>
        /// <param name="pos"></param>
        public override void SetDirLoc(long timestamp, Vector3 dir, Vector3 pos)
        {
            base.SetDirLoc(timestamp, dir, pos);
            dirUpdate.position = pos;
            dirUpdate.direction = dir;
            dirUpdate.dirty = false;
        }

        public override GameObject GameObject
        {
            get
            {
                return gameObject;
            }
            set
            {
                if (gameObject != null && !gameObject.Equals(value))
                {
                    UnityEngine.GameObject.Destroy(gameObject);
                    gameObject = null;
                }
                gameObject = value;
                if (worldManager.ActiveInputController != null)
                {
                    worldManager.ActiveInputController.Target = GetControllingTransform();
                }
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                if (gameObject.GetComponent<AtavismNode>() != null)
                    GameObject.DestroyImmediate(gameObject.GetComponent<AtavismNode>());
                gameObject.AddComponent<AtavismNode>();
                gameObject.GetComponent<AtavismNode>().SetNodeData(this);
                if (PropertyExists("movement_speed"))
                {
                    mobController.RunSpeed = (int)GetProperty("movement_speed");
                }
            }
        }

        public override GameObject Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if (value != null)
                {
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.identity;
                }
                else if (parent != null)
                {
                    gameObject.transform.localPosition = parent.transform.position;
                    gameObject.transform.localRotation = parent.transform.rotation;
                }
                parent = value;
                if (worldManager.ActiveInputController != null)
                {
                    worldManager.ActiveInputController.Target = GetControllingTransform();
                }
                //UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }
        }

        public AtavismInputController InputController
        {
            get
            {
                return inputController;
            }
            set
            {
                inputController = value;
            }
        }
    }
}