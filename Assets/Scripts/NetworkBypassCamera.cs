using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBypassCamera : MonoBehaviour
{
	private Transform cacheTransform;
	
	void Update ()
	{
		var forward = transform.TransformDirection(Vector3.forward) * 10;
//        Debug.DrawRay(transform.position, forward);
        
		RaycastHit hit;
		if (Physics.Raycast(transform.position, forward, out hit, 100))
		{
			cacheTransform = hit.transform;
			
			hit.transform.gameObject.BroadcastMessage("OnCameraHover", true, 
				SendMessageOptions.DontRequireReceiver);
			if (Input.GetMouseButtonDown(0))
			{
				hit.transform.gameObject.BroadcastMessage("OnCameraClick", this, 
					SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			if (cacheTransform != null)
			{
				cacheTransform.gameObject.BroadcastMessage("OnCameraHover", false, 
					SendMessageOptions.DontRequireReceiver);
				cacheTransform = null;
			}
		}
	}
}
