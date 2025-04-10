using TMPro;

namespace Project
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class ScoreView : MonoBehaviour
    {
        [SerializeField] TMP_Text _scoreText; // スコアを測る間隔（秒）

        public void Set(float score)
        {
            _scoreText.text = $"{score:F0}";
        }
    }
}