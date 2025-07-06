using TMPro;
using UnityEngine;

namespace SandboxGame
{
    public class PNL_ViewScale : MonoBehaviour
    {

        [SerializeField] private TMP_Text _cameraZoomText;

        private EditController controller;
        /// <summary>
        /// Implement later
        /// </summary>
        public void Init(EditController editController)
        {
            controller = editController;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (controller)
            {
                _cameraZoomText.text = "x" + controller.CameraZoomMultiplier.ToString("F2");
            }
        }
    }
}
