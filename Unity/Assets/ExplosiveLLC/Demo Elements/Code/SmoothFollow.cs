using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
	private const float SMOOTH_TIME = 0.3f;
	public bool LockX;
	public float offSetX;
	public float offsetY;
	public bool LockY;
	public bool LockZ;
	public bool useSmoothing;
	public Transform target;
	public GameObject hudElements;
	private Transform thisTransform;
	private Vector3 velocity;
	bool hudActive = true;
	
	private void Awake()
	{
		thisTransform = transform;
		velocity = new Vector3(0.5f, 0.5f, 0.5f);
		target = GameObject.Find("Crafter").transform;
	}

	void Update()
	{
		if(hudActive)
		{
			if(Input.GetKeyDown(KeyCode.H))
			{
				hudElements.SetActive (false);
				hudActive = false;
			}
		}
		else
		{
			if(Input.GetKeyDown(KeyCode.H))
			{
				hudElements.SetActive (true);
				hudActive = true;
			}
		}
	}

	private void FixedUpdate()
	{
		var newPos = Vector3.zero;
		if(useSmoothing)
		{
			newPos.x = Mathf.SmoothDamp(thisTransform.position.x, target.position.x + offSetX, ref velocity.x, SMOOTH_TIME);
			newPos.y = Mathf.SmoothDamp(thisTransform.position.y, target.position.y + offsetY, ref velocity.y, SMOOTH_TIME);
			newPos.z = Mathf.SmoothDamp(thisTransform.position.z, target.position.z, ref velocity.z, SMOOTH_TIME);
		}
		else
		{
			newPos.x = target.position.x;
			newPos.y = target.position.y;
			newPos.z = target.position.z;
		}

		if(LockX) { newPos.x = thisTransform.position.x; }
		if(LockY) { newPos.y = thisTransform.position.y; }
		if(LockZ) { newPos.z = thisTransform.position.z; }

		transform.position = Vector3.Slerp(transform.position, newPos, Time.time);
	}
}