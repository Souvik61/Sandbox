using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{
    public class UIFieldBool : UIField
    {
        bool value;

        public TMP_Text nameText;
        public TMP_Text valueText;
        public Toggle valueToggle;

        public override object Value
        {
            get => value;
            set
            {
                this.value = (bool)value;
                valueText.text = value.ToString();
            }
        }

        public void Initialize(string Id, string label, bool value, Action<bool> callback)
        {
            this.Id = Id;
            nameText.text = label;
            valueText.text = value.ToString();
            this.value = value;
            valueToggle.isOn = value;
            valueToggle.onValueChanged.AddListener((val) => { callback(val); });
        }

    }
}
