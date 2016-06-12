using UnityEngine;
using System.Collections;

public class FacingCameraBillboard : MonoBehaviour {

	[SerializeField] Camera TargetCamera;

	void Update () 
    {
	    transform.LookAt(transform.position + TargetCamera.transform.rotation * Vector3.forward, TargetCamera.transform.rotation * Vector3.up);
	}
}
