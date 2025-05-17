using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SandboxGame
{
    public abstract class UIFieldFloat : UIField
    {
        float value;

        public TMP_Text nameText;
        public TMP_Text valueText;

        public override System.Object Value
        {
            get => value;
            set => this.value = (float)value;
        }

        public void Initialize(string Id, string label, string value)
        {
            this.Id = Id;
            nameText.text = label;
            valueText.text = value;
        }
    }
}
