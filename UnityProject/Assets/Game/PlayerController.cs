using UnityEngine;
using System.Collections;

public class PlayerController : MyGameObject {

    //public Transform mount;
    public MyGameObject wagon;
    public MyGameObject wagon2;
    public MyGameObject wagon3;
    public MyGameObject wagon4;

    //public LogicVehicle logicVehicle;
    public LogicInput logicInput;
    public LogicVehicle logicVehicle { get { return (LogicVehicle)logic; } }


    public int speed;
    public int steer;

    void Start () {

        LogicVehicle logicVehicle = new LogicVehicle();
        logicInput = new LogicInput();
        logicVehicle.SetInput(logicInput);
        logic = logicVehicle;
        logic.angle = 90;

        MyGameObject tail = this;
        
        tail = JoinWagon(tail, wagon);
        tail = JoinWagon(tail, wagon2);
        tail = JoinWagon(tail, wagon3);
        tail = JoinWagon(tail, wagon4);


    }

    public MyGameObject JoinWagon(MyGameObject tail, MyGameObject wagon)
    {
        if (wagon && (wagon.logic is LogicWagon) && tail && (tail.logic is LogicWagon))
        {
            LogicWagon logicWagon = (LogicWagon)wagon.logic;
            LogicWagon logicTail = (LogicWagon)tail.logic;
            
            logicWagon.Join(logicTail);
            return wagon;
        }

        return tail;
    }
    
    public override void LogicUpdate () {
        int x = (int)(Input.GetAxisRaw("Horizontal") * 100f);
        int y = (int)(Input.GetAxisRaw("Vertical") * 100f);
        logicInput.Set(x, y, false, false);

        base.LogicUpdate();
    }

    public override void GameUpdate(float subStep) {
        speed = logicVehicle.speed;
        steer = logicVehicle.steer;

        //speed = ((LogicWagon)wagon.logic).velocity.getLength();

        base.GameUpdate(subStep);
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
