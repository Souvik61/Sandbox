using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SandboxGame
{
    public class UIFieldFloat : UIField
    {
        float value;

        public TMP_Text nameText;
        public TMP_Text valueText;

        public override System.Object Value
        {
            get => value;
            set
            {
                this.value = (float)value;
                valueText.text = value.ToString();
            }
        }

        public void Initialize(string Id, string label, float value)
        {
            this.Id = Id;
            nameText.text = label;
            valueText.text = value.ToString();
            this.value = value;
        }
    }
}
