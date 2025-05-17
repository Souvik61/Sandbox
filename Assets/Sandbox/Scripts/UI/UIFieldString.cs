using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SandboxGame
{
    public abstract class UIFieldString : UIField
    {
        string value;

        public TMP_Text nameText;
        public TMP_Text valueText;

        public override object Value
        {
            get => value;
            set => this.value = (string)value;
        }

        public void Initialize(string Id, string label, string value)
        {
            this.Id = Id;
            nameText.text = label;
            valueText.text = value;
        }

    }
}
