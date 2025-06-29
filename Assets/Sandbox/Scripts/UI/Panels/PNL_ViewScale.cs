using TMPro;
using UnityEngine;

namespace SandboxGame
{
    public class PNL_ViewScale : MonoBehaviour
    {

        [SerializeField] private TMP_Text _cameraZoomText;

        /// <summary>
        /// Implement later
        /// </summary>
        public void Init()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (EditController.InstanceValid)
            {
                _cameraZoomText.text = "x" + EditController.Instance.CameraZoomMultiplier.ToString("F2");
            }
        }
    }
}
