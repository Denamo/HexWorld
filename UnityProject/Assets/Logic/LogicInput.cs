
public class LogicInput
{

    public int x = 0;
    public int y = 0;
    public bool a = false;
    public bool b = false;
    
    public LogicInput()
	{
		
	}

    public void Set(int x, int y, bool a, bool b)
    {
        this.x = LogicMath.clamp(x, -1, 1);
        this.y = LogicMath.clamp(y, -1, 1);
        this.a = a;
        this.b = b;
    }
    
}



