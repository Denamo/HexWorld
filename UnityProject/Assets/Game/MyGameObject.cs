using UnityEngine;
using System.Collections;

public class MyGameObject : MonoBehaviour {

	public LogicGameObject logic;
    
    void Start () {
        logic = new LogicVehicle();
    }

    private LogicTransform lastLogicTransform = new LogicTransform();

    public void FixedUpdate () {
        LogicUpdate();
    }

    public virtual void LogicUpdate()
    {
        lastLogicTransform.Set(logic);
        logic.Update();
    }

    void Update() {
        float subStep = (Time.time - Time.fixedTime) / (Time.fixedDeltaTime);
        GameUpdate(subStep);
    }

    static Vector3 center = new Vector3(0, 0, 64);
    public Vector3 logicToGame(LogicVector2 pos)
    {
        return center + new Vector3(pos.x, 0, pos.y) * 0.01f;
    }

    public Quaternion logicToGame(float angle)
    {
        return Quaternion.Euler(0, angle, 0);
    }
    
    bool lerping = true;
    public virtual void GameUpdate(float subStep)
    {
        
        Vector3 pos = logicToGame(logic.position);
        Vector3 lastPos = logicToGame(lastLogicTransform.position);
        Vector3 lerpedPos = Vector3.Lerp(lastPos, pos, subStep);
        
        float lerpedAngle = Mathf.LerpAngle(lastLogicTransform.angle, logic.angle, subStep);
        
        if (Input.GetKeyDown("l"))
        {
            lerping = !lerping;
            Debug.Log("Lerping " + lerping);
        }
        
        if (lerping)
            transform.SetPositionAndRotation(lerpedPos, logicToGame(lerpedAngle));
        else
            transform.SetPositionAndRotation(logicToGame(logic.position), logicToGame(logic.angle));
        
    }

    /*
    Vector3 getWheelSuspensionPosition(WheelCollider collider)
	{
		Vector3 center = collider.transform.TransformPoint(collider.center);
		RaycastHit hit;

		if ( Physics.Raycast(center, -collider.transform.up, out hit, collider.suspensionDistance + collider.radius) ) {
			return hit.point + (collider.transform.up * collider.radius);
		} else {
			return center - (collider.transform.up * collider.suspensionDistance);
		}

	}
    */


}
