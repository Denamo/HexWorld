
public class LogicVehicle : LogicWagon
{
    
    public LogicInput input = new LogicInput();

    public int speed = 0;
    public int speedMax = 50;
    public int speedMin = 0;
    public int steer = 0;
    public int steerMax = 10;
    public int steerMin = -10;

    public LogicVehicle()
	{
        tailPosLocal = new LogicVector2(0, -100);
    }

    public void SetInput(LogicInput input)
    {
        this.input = input;
    }

    public LogicInput GetInput()
    {
        return input;
    }
    
    public override void Update()
    {
        ApplyInput();
        ApplyOutput();

        base.Update();
    }

    private void ApplyInput()
    {
        if (input.y > 0)
        {
            speed += 2;
        }
        else if(input.y==0)
        {
            speed -= 1;
        }
        else
        {
            speed -= 4;
        }
        speed = LogicMath.clamp(speed, speedMin, speedMax);

        const int turnStep = 3;
        if (input.x == 0)
        {
            if (LogicMath.abs(steer) <= turnStep)
                steer = 0;
            else
                steer -= LogicMath.sign(steer) * turnStep;
        }
        else
        {
            steer += LogicMath.sign(input.x) * turnStep;
        }
        steer = LogicMath.clamp(steer, steerMin, steerMax);
    }

    private void ApplyOutput()
    {
        angle = LogicMath.normalizeAngle360(angle+steer);
        
        LogicVector2 speedVector = new LogicVector2((LogicMath.sin(angle) * speed) >> LogicMath.FIXED_SHIFT, (LogicMath.cos(angle) * speed) >> LogicMath.FIXED_SHIFT);

        velocity = speedVector;

        position.add(speedVector);

        
    }
}



