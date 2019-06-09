using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;

	// Use this for initialization
	void Start () {
		/*if(target){
			lastLookAt = target.position;
		}*/
	}

	//Vector3 lastLookAt;

	// Update is called once per frame
	void LateUpdate () {
		if(target){
			Vector3 offset = new Vector3(0f,20f,-10f);
			Vector3 offset2 = target.TransformPoint(offset);
			offset2.y = target.position.y + offset.y;

			//Vector3 lerped = Vector3.Lerp(transform.position,offset2,0.1f);

            Vector3 lerped = Vector3.Lerp(transform.position, offset2, 1f);

            transform.position = lerped; //target.position + offset;

            transform.position = offset2; //target.position + offset;

            //transform.position.z = target.position.z = target.z + 20f;

            transform.LookAt(target.position+Vector3.up*7f);
            //+target.TransformPoint(Vector3.forward) * 10f
            //Vector3
            /*Vector3 lookAt = Vector3.Lerp(lastLookAt,target.position,0.1f);
			transform.LookAt(lookAt);
			lastLookAt = lookAt;*/

        }
	}
}
