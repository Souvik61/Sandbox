using Redapple;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    /// <summary>
    /// Manages gameobjects colors
    /// </summary>
    public class ColorManager : MonoBehaviour
    {
        public Color LastUsedColor { get => _lastUsedColor; }
        private Color _lastUsedColor;

        GameManager _gameManagerInstance;

        public void Init(GameManager gameManager)
        {
            _gameManagerInstance = gameManager;
            _lastUsedColor = Color.black;
        }

        public Color GetRandomColor()
        {
            Color col = _gameManagerInstance.ConfigData.colorList[Random.Range(0, _gameManagerInstance.ConfigData.colorList.Length)];

            while (_lastUsedColor == col)
            {
                col = _gameManagerInstance.ConfigData.colorList[Random.Range(0, _gameManagerInstance.ConfigData.colorList.Length)];
            }

            _lastUsedColor = col;
            return col;

        }
    }
}