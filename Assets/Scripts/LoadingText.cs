using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingText : MonoBehaviour
{
    private TextMeshProUGUI _Text;
    private float _NextTick;

    // Start is called before the first frame update
    void Start()
    {
        _Text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_NextTick < Time.time)
        {
            ChangeText();

            _NextTick = Time.time + .2f;
        }
    }

    private void ChangeText()
    {
        switch (_Text.text)
        {
            case "":
                _Text.text = ".";
                break;
            case ".":
                _Text.text = "..";
                break;
            case "..":
                _Text.text = "...";
                break;
            case "...":
                _Text.text = "";
                break;
            default:
                _Text.text = "...";
                break;
        }
        
    }
}
