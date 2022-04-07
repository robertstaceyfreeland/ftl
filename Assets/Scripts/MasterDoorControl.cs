using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MasterDoorControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenAllDoors()
    {
        var _DoorControls = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDoorControl>();

        foreach (IDoorControl i in _DoorControls)
        {
            i.LockOpenAllDoors();
        }
    }

    public void CloseAllDoors()
    {
        var _DoorControls = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDoorControl>();

        foreach (IDoorControl i in _DoorControls)
        {
            i.CloseAllDoors();
        }
    }
}
