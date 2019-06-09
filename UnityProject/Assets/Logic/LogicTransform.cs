
public class LogicTransform
{
    public LogicVector2 position = new LogicVector2();
    public int angle = 0;

    public LogicTransform()
	{
	}

    public LogicTransform(LogicTransform transform)
    {
        Set(transform);
    }

    public void Set(LogicTransform transform)
    {
        position = new LogicVector2(transform.position);
        angle = transform.angle;
    }

    public LogicTransform Clone()
    {
        return new LogicTransform(this);
    }


    public LogicVector2 localToWorld(LogicVector2 pos)
    {
        LogicVector2 clone = pos.clone();
        clone.rotate(-angle);
        clone.add(position);
        return clone;
    }

}



