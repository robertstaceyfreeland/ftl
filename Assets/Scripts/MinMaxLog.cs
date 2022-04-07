using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMaxLog : MonoBehaviour
{

    [SerializeField] private RectTransform _CombatLog;

    private bool _IsTop = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleMinMax()
    {
        


        if (_IsTop)
        {
            _CombatLog.sizeDelta = new Vector2(590, 110);
            _IsTop = false;

            Vector3 _NewPosition = new Vector3(transform.position.x, transform.position.y-200,0);
            transform.position = _NewPosition;
            transform.rotation = Quaternion.Euler(0, 0, 90);



        }
        else
        {
            _CombatLog.sizeDelta = new Vector2(590, 275);
            _IsTop = true;

            Vector3 _NewPosition = new Vector3(transform.position.x, transform.position.y+200, 0);
            transform.position = _NewPosition;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        

        
    }

}
