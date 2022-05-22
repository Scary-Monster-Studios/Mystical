using UnityEngine;
using System.Collections;
namespace Atavism
{

    public abstract class AtavismInputController : MonoBehaviour
    {

        // Is this InputController active?
        protected bool inputControllerActive = false;
        // The target of the input controller
        protected Transform target = null;
        // is the camera grabbed by a script
        protected bool cameraGrabbed = false;
        protected bool mouseWheelDisabled = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public abstract Vector3 GetPlayerMovement();

        public abstract void RunCameraUpdate();

        public bool InputControllerActive
        {
            get
            {
                return inputControllerActive;
            }
            set
            {
                inputControllerActive = value;
            }
        }

        public Transform Target
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

        public bool MouseWheelDisabled
        {
            get
            {
                return mouseWheelDisabled;
            }
            set
            {
                mouseWheelDisabled = value;
            }
        }

        public bool CameraGrabbed
        {
            get
            {
                return cameraGrabbed;
            }
            set
            {
                cameraGrabbed = value;
            }
        }
    }
}