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

        public TMP_Text nameText;
        public Button colorButton;
        public Image displayColorImage;

        private Action<float> onValueChanged;

        public override object Value
        {
            get => value;
            set
            {
                this.value = (Color)value;
                displayColorImage.color = (Color)value;
            }
        }

        public void Initialize(string Id, string label, Color value, Action callback)
        {
            this.Id = Id;
            nameText.text = label;
            displayColorImage.color = value;
            this.value = value;
            colorButton.onClick.AddListener(() => { callback(); });
        }

    }
}
