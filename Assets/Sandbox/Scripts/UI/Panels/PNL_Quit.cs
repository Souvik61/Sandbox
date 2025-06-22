using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{
    public class PNL_Quit : MonoBehaviour
    {
        [Header("References")]
        public GameObject btnYes;
        public GameObject btnNo;

        public CanvasGroup panel;

        public Action OnYes;
        public Action OnNo;

        public void Init()
        {
            btnYes.GetComponent<Button>().onClick.AddListener(() => { OnYes?.Invoke(); });
            btnNo.GetComponent<Button>().onClick.AddListener(() => { OnNo?.Invoke(); });
        }

        public void Show()
        {
            panel.alpha = 0;
            panel.DOFade(1, 0.1f);
        }

        public void Hide()
        {
            panel.DOFade(0, 0.1f);
        }

    }
}
