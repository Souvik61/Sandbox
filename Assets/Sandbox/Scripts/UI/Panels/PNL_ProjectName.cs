using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{
    public class PNL_ProjectName : MonoBehaviour
    {
        [Header("References")]
        public GameObject btnOk;
        public TMP_InputField inputField;

        public CanvasGroup panel;

        public Action OnOk;

        public void Init()
        {
            btnOk.GetComponent<Button>().onClick.AddListener(() => { OnOk?.Invoke(); });
        }

        public void Show()
        {
            panel.alpha = 0;
            panel.DOFade(1, 0.1f);
        }

        public void Hide()
        {
            //panel.DOFade(0, 0.1f);
        }
    }
}
