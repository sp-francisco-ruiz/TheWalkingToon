using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{

    [SerializeField] Transform Followed;
    [SerializeField] float FollowSpeed;

    Vector3 _offset;

	// Use this for initialization
	void Awake () 
    {
        _offset = transform.position -  Followed.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    transform.position = Vector3.Lerp(transform.position, Followed.position + _offset, FollowSpeed * Time.deltaTime);
	}
}
