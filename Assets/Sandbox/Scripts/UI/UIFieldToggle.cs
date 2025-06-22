using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{
    public class UIFieldToggle: UIField
    {
        int value;

        public TMP_Text nameText;
        public ToggleGroup toggleGroup;

        [SerializeField] private Toggle toggle1;
        [SerializeField] private Toggle toggle2;
        [SerializeField] private Toggle toggle3;
        [SerializeField] private Toggle toggle4;

        public override object Value
        {
            get => value;
            set
            {
                this.value = (int)value;

                var toggles = toggleGroup.GetComponentsInChildren<Toggle>();

                for (int i = 0; i < toggles.Length; i++)
                {
                    toggles[i].isOn = (i == (int)value) ? true : false;
                }
            }
        }

        public void Initialize(string Id, string label, int value, Action<int> callback)
        {
            this.Id = Id;
            nameText.text = label;
            this.value = value;

            var toggles = toggleGroup.GetComponentsInChildren<Toggle>();

            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = (i == value) ? true : false;
                var t = toggles[i];

                toggles[i].onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        OnToggleChanged(t, callback);
                    }
                });
            }
        }

        private void OnToggleChanged(Toggle changedToggle, Action<int> callback)
        {

            if (changedToggle == toggle1)
            {
                callback(0);
            }
            else if (changedToggle == toggle2)
            {
                callback(1);
            }
            else if (changedToggle == toggle3)
            {
                callback(2);
            }
            else if (changedToggle == toggle4)
            { 
                callback(3);
            }
        }

    }
}
