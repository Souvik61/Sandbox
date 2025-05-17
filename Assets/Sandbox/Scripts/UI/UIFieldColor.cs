using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SandboxGame
{
    public class UIFieldColor: UIField
    {
        Color value;

        public Text nameText;
        public Button colorButton;

        private Action<float> onValueChanged;

        public override object Value
        {
            get => value;
            set => this.value = (Color)value;
        }

        public void Initialize(string Id, string label, Color value, Action<float> callback)
        {
            this.Id = Id;
            nameText.text = label;
            //onValueChanged = callback;

            //inputField.onEndEdit.AddListener(OnValueChanged);
        }

    }
}
