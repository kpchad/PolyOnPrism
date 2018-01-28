using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetInteration : MonoBehaviour {

    // speed of depth change
    [Tooltip("In ms^-1")] [SerializeField] float depthSpeed = 1f;

	// Use this for initialization
	void Start () {
		
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
	}
}
