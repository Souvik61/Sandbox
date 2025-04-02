using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISimStatus : MonoBehaviour
{

    public TMP_Text statusText;

    
    // Update is called once per frame
    void Update()
    {
        statusText.text = PhysicsSimulatorManager.Instance.GetStatusText();
    }



}
