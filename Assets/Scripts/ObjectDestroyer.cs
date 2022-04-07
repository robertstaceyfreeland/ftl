using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            Destroy(GameObject.Find("GameHandler"));
        }

        catch
        {
            Debug.Log("GameHandler NOT Destroyed!");
        }
        
    }
}
