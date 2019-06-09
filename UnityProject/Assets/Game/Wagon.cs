using UnityEngine;
using System.Collections;

public class Wagon : MyGameObject {

	public LogicWagon logicWagon { get { return (LogicWagon)logic; } }
    
    void Start () {

        LogicWagon logicWagon = new LogicWagon();
        
        logic = logicWagon;
    }

    

}
