﻿// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
// 
// Downloading and/or using this MIRA SDK is under license from MIRA, 
// and subject to all terms and conditions of the Mira Software License,
// found here: www.mirareality.com/sdk-license/
// 
// By downloading this SDK, you agree to the Mira Software License.
//
// This SDK may only be used in connection with the development of
// applications that are exclusively created for, and exclusively available
// for use with, MIRA hardware devices. This SDK may only be commercialized
// in the U.S. and Canada, subject to the terms of the License.
// 
// The MIRA SDK includes software under license from The Apache Software Foundation.

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// An example of how to use MiraController and the event system to grab 3D objects with Unity's physics simulation system
/// </summary>
public class GrabObject : MonoBehaviour, IPointerDownHandler {
    private bool isGrabbing = false;

    private RigidbodyConstraints originalConstraints;
    private Rigidbody rigidBody;

    private float lastTouchPosition;

    private float depthSpeed = 4f;

    // these OnPointer functions are automatically called when
    // the pointer interacts with a game object that this script is attached to
    public void OnPointerDown(PointerEventData pointerData) {
        // onPointerDown is called every frame the pointer is held down on the object
        // we only want to grab objects if the click button was just pressed
        // this prevents multiple objects from unintentionally getting grabbed
        switch (isGrabbing){
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

    // Use this for initialization
    private void Start() {
        rigidBody = this.gameObject.GetComponent<Rigidbody>();
        originalConstraints = rigidBody.constraints;
    }

    // Update is called once per frame
    private void Update() {
        // stop grabbing if the user isn't clicking
        //if (isGrabbing == true && MiraController.ClickButton == false) {
          //  rigidBody.constraints = originalConstraints;
          //  isGrabbing = false;
        //}

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
            thisTouch -= 0.5f;
            // now its -0.5 to 0.5
            //thisTouch *= -1.0f;
            // scale it down so it's not too strong
            thisTouch *= 0.05f;
            //Debug.Log(thisTouch);
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
         

        }
    }
}