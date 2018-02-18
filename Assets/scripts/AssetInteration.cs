using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class AssetInteration : MonoBehaviour, IPointerDownHandler {

    private bool isGrabbing = false;

    private RigidbodyConstraints originalConstraints;
    private Rigidbody rigidBody;

    private float lastTouchPosition;

    // speed of depth change
    [SerializeField] float depthSpeed = 1f;
    [SerializeField] float rotateSpeed = 1f;


	// Use this for initialization
	void Start () {
        rigidBody = this.gameObject.GetComponent<Rigidbody>();
        originalConstraints = rigidBody.constraints;
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 touchPos = MiraController.TouchPos;
        bool triggerHold = MiraController.TriggerButton;
        bool touchpadButtonHold = MiraController.TouchpadButton;

        float depthChange = 0f;
        if (triggerHold && touchpadButtonHold == true) {
            Debug.Log(touchPos);
            depthChange = touchPos.y * depthSpeed * Time.deltaTime;
        }
        float newZ = transform.localPosition.z + depthChange;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZ);


        if (isGrabbing == true) {
            // freeze the position of the physics simulation temporarily so the object doesn't
            // spiral out of control while its being interacted with
            // you could freeze the rotation as well if you wanted
            rigidBody.constraints = RigidbodyConstraints.FreezePosition;

            float touchInfluence = 0.0f;
            float thisTouch = 0.0f;

            // MiraController.Touchpos.Y goes from 1 to 0 , near to far
            // we want to change this so the touchpad closer to the user returns negative values
            // and the upper half returns positive values
            thisTouch = MiraController.TouchPos.y;
            //Debug.Log(thisTouch);
            // now its 0.5 to -0.5
            //thisTouch -= 0.5f;
            // now its -0.5 to 0.5
            Debug.Log(thisTouch);
            //thisTouch *= -1.0f;
            // scale it down so it's not too strong
            thisTouch *= 0.05f;

            //touchInfluence = thisTouch * depthSpeed * Time.deltaTime;

            // get the distance from this object to the controller
            float currentDistance = (MiraController.Position - transform.position).magnitude;
            // the new distance of the grabbed object is the current distance,
            // adjusted by the users touch, in the direction it was from the controller
            Vector3 newLength = MiraController.Direction.normalized * (currentDistance + touchInfluence);
            //Debug.Log(newLength);
            Vector3 newPosition = MiraController.Position + newLength;
            //Debug.Log(newPosition);
            transform.position = newPosition;
            //Debug.Log(transform.position); 

            if (MiraController.TouchpadButton == true) {
                transform.Rotate(0, rotateSpeed, 0);
            }
        }
	}

    // these OnPointer functions are automatically called when
    // the pointer interacts with a game object that this script is attached to
    public void OnPointerDown(PointerEventData pointerData) {
        // onPointerDown is called every frame the pointer is held down on the object
        // we only want to grab objects if the click button was just pressed
        // this prevents multiple objects from unintentionally getting grabbed
        switch (isGrabbing) {
            // if grabbing, release
            case true:
                if (MiraController.ClickButtonPressed) {
                    isGrabbing = false;
                    Debug.Log("object released");
                }
                break;
            // if not grabbing, grab
            case false:
                if (MiraController.ClickButtonPressed) {
                    isGrabbing = true;
                    Debug.Log("holding object");
                }
                break;
        }
    }




}
