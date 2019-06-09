
public class LogicWagon : LogicGameObject
{
    public LogicWagon jointParent;
    public LogicVector2 jointPosLocal = new LogicVector2();
    public LogicVector2 velocity = new LogicVector2();
    public int angularVelocity = 0;
    public LogicVector2 tailPosLocal = new LogicVector2();


    public LogicWagon()
	{
        jointPosLocal = new LogicVector2(0,100);
        tailPosLocal = new LogicVector2(0,-100);
    }

    public void Join(LogicWagon parent)
    {
        jointParent = parent;
        SnapToJoint();
    }

    public override void Update()
    {
        LogicVector2 oldPos = position.clone();
        int oldAngle = angle;

        ResolveInertia();
        ResolveJoint();
    
        oldPos.subtract(position);
        velocity = oldPos;
        angularVelocity = LogicMath.normalizeAngle180(angle - oldAngle);

    }

    public void SnapToJoint()
    {
        if (jointParent == null)
            return;

        LogicVector2 jointPosParentWorld = jointParent.localToWorld(jointParent.tailPosLocal);

        LogicVector2 jointPosWorld = localToWorld(jointPosLocal);
        jointPosWorld.subtract(jointPosParentWorld);

        position.subtract(jointPosWorld);
    }

    public void ResolveInertia()
    {
        LogicVector2 sub = velocity.clone();
        int length = sub.getLength();
        
        int frac = (length * 50) / 100;
        sub.normalize(frac);
        position.add(sub);


    }


    public void ResolveJoint()
    {
        
        if (jointParent == null)
            return;

        LogicVector2 jointPosParentWorld = jointParent.localToWorld(jointParent.tailPosLocal);

        LogicVector2 deltaToParentJoint = jointPosParentWorld.clone();
        deltaToParentJoint.subtract(position);
        int angleToParentJoint = LogicMath.getAngle(deltaToParentJoint.y, deltaToParentJoint.x);

        angle = angleToParentJoint + (angularVelocity*65)/100;

        LogicVector2 jointPosWorld = localToWorld(jointPosLocal);

        LogicVector2 delta = jointPosWorld.clone();
        delta.subtract(jointPosParentWorld);
        
        position.subtract(delta);

        
    }

}



