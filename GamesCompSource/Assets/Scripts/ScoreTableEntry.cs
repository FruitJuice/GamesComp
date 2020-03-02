using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.NUIGalway.CompGame
{
    public class ScoreTableEntry : MonoBehaviour
    {
        public Image PlayerColorImage;
        public Text PlayerNameText;
        public Text PlayerScoreText;

        #region Public Methods
        public void Initialize(int playerId, string playerName, string playerScore)
        {
            PlayerColorImage.color = ClipperGate.GetColor(playerId);
            PlayerNameText.text = playerName;
            PlayerScoreText.text = playerScore;
        }
        #endregion

    }
}
