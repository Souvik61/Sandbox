using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    public class PNL_Settings : MonoBehaviour
    {
        public UIFieldBool showDetailsField;

        private void Start()
        {
            var settings = GameManager.Instance.gameSettings;

            showDetailsField.Initialize("showDetails", "Show more details in inspector", settings.ShowDetails, (val) => 
            {
                settings.ShowDetails = val;
            });
        }
    }
}
