using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSelector : MonoBehaviour
{
    public MeshRenderer _MeshRenderer;
    public Material _Material;






    // Start is called before the first frame update
    void Start()
    {
        _MeshRenderer.material = _Material;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
