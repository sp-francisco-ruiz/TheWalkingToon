using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{

    [SerializeField] Transform Followed;
    [SerializeField] float FollowSpeed;
    [SerializeField] float RotationSpeed;

	// Use this for initialization
	void Awake () 
    {
	}
	
	// Update is called once per frame
	void Update () 
    {
        var targetPos = Followed.position;
        targetPos.y = 0f;
        transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Followed.rotation, RotationSpeed * Time.deltaTime);
	}
}
