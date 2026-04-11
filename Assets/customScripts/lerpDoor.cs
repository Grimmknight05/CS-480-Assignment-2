using System;
using UnityEngine;

namespace StealthGame
{
    /*
    //- CUSTOM LERP DOOR -//
    There are three modes this door can function in this is defined by an Enum DoorMode and selected in the inpector for each door.
    I adapted the origonal door code which simply deleted the door on colision. I changed this interaction to a trigger and added a 
    seperate trigger box for more range. I also added door state enum to track the doors state to check to see if the door is already open

    //SLIDE
        In slide mode, I lerp between startPos targetPos in acordance to the set offset.
        This lerp is controlled by a clamped and curved delta time, to acieve a smooth transition between the two points. The exponential easing 
        function used is, 1 - Mathf.Exp(-5 * t). Speed is controlled by the variable speed and set independantly on each door.
    //ROTATE
        The Rotate mode is similar to the slide mode we just Quaternion.lerp between startRot targetRot defined at start using rotationAmount
        as the rotation offset. 
    //AXIS_ROTATION
        The axis rotation mode uses a defined game object position as a pivot for the door. We calculate the current angle and then the target
        rotation angle using a lerp according to tEased. We find the difference between these two angles and pass it into RotateAround
        which uses that angle to rotate around the defind rotation pivot, and rotationAxis.
     */
    public class lerpDoor : MonoBehaviour
    {
        [SerializeField] private string KeyName = "key1";
        [SerializeField] private float speed = 5f;
        private float t = 0f;
        private bool opening = false;
        //SLIDE
        private Vector3 startPos;
        private Vector3 targetPos;
        [SerializeField] private Vector3 offset;
        //ROTATION
        private Quaternion startRot;
        private Quaternion targetRot;
        [SerializeField] private float rotationAmount = 90.0f; // degrees
        //AXIS_ROTATION
        [SerializeField] private Transform pivot;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        private float currentAngle = 0f;
        //
        [SerializeField] private Transform doorMesh;
        
        public enum DoorMode //Door Type
        {
            Slide,
            Rotate,
            AxisRotation
        }
        [SerializeField] DoorMode mode;
        public enum DoorState //Door State
        {
            Closed,
            Opening,
            Open
        }

        public DoorState state = DoorState.Closed;

        private void OnTriggerEnter(Collider other)
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();//Get ref to player on trigger

            if (player == null)
                return;

            if (player.OwnKey(KeyName) && state == DoorState.Closed)//If player has key in inventory, and door is closed
            {
                Debug.Log("Door triggered");
                //Destroy(gameObject);
                state = DoorState.Opening;
                t = 0f;
                //AXIS_ROTATION
                currentAngle = 0.0f;

            }
        }
        private void Start()
        {
            //Slide
            startPos = doorMesh.position;
            targetPos = startPos + offset;
            //Rotation
            startRot = doorMesh.rotation;//Get current Rotation
            targetRot = Quaternion.Euler(0, 90, 0) * startRot;
        }
        private void Update()
        {
            if (state == DoorState.Opening)//Opening state
            {
                //Time Calculation
                t += Time.deltaTime * speed;//Delta Time
                t = Mathf.Clamp01(t);//Clamps t between 0 and 1
                //Debug.Log("Opening... t=" + t);
                float tEased = 1 - Mathf.Exp(-5 * t);// Ease time slow at start slow at end

                //Modes
                switch (mode)//Mode switch
                {
                    //SLIDE
                    case DoorMode.Slide:
                        doorMesh.position = Vector3.Lerp(startPos, targetPos, tEased);
                        break;
                    //ROTATE
                    case DoorMode.Rotate:
                        doorMesh.rotation = Quaternion.Lerp(startRot, targetRot, tEased);
                        break;
                    //AXIS_ROTATION
                    case DoorMode.AxisRotation:
                        float targetAngle = Mathf.Lerp(0f, rotationAmount, tEased); //Find the lerped angle acording to tEased
                        float delta = targetAngle - currentAngle; //Calculate distance between angles
                        transform.RotateAround(pivot.position, rotationAxis, delta);//Actual Rotation
                        currentAngle = targetAngle;//set new angle as current
                        break;
                }
                if (t >= 1.0f)//when t = 1.0
                {
                    state = DoorState.Open;//Door fully open, break loop
                }
            }
        }
    }
}