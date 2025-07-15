using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    public class FCPTest : MonoBehaviour
    {
        public FlexibleColorPicker colorPicker;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                colorPicker.SetColor(Color.black);
            }
        }

        public void OnColorChange()
        {
            Debug.Log("Color change!");
            Debug.Log(colorPicker.color);
        }
    }
}
