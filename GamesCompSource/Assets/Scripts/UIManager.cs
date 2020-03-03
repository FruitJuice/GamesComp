using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.NUIGalway.CompGame
{
    public class UIManager : MonoBehaviour
    {

        public CanvasGroup PlayerInformationPanel;
        public CanvasGroup EscapePanel;
        public CanvasGroup UserScorePanel;

        public GameObject scoreTable;

        #region Monobehaviour Callbacks
        private void Start()
        {
            SetActivePanel(PlayerInformationPanel.name);
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (UserScorePanel.alpha != 0)
                {
                    SetActivePanel(PlayerInformationPanel.name);
                }
                else
                {
                    SetActivePanel(UserScorePanel.name);
                    FillTable();
                }
            }

            if (Input.GetButtonDown("Cancel"))
            {
                if (EscapePanel.alpha != 0)
                {
                    SetActivePanel(PlayerInformationPanel.name);
                }
                else
                {
                    SetActivePanel(EscapePanel.name);
                }
            }
        }

        #endregion

        #region Private Methods

        void FillTable()
        {
            foreach (Transform child in UserScorePanel.transform)
            {
                Destroy(child.gameObject);
            }


            foreach (Player p in PhotonNetwork.PlayerList)
            {
                float score = (float)p.CustomProperties[ClipperGate.PLAYER_SCORE];

                GameObject entry = Instantiate(scoreTable);
                entry.transform.SetParent(UserScorePanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<ScoreTableEntry>().Initialize(p.GetPlayerNumber(), p.NickName, score.ToString());
            }
        }

        void SetActivePanel(string activePanel)
        {
            PlayerInformationPanel.alpha = System.Convert.ToSingle((activePanel.Equals(PlayerInformationPanel.name)));
            EscapePanel.alpha = System.Convert.ToSingle((activePanel.Equals(EscapePanel.name)));
            EscapePanel.interactable = activePanel.Equals(EscapePanel.name);
            UserScorePanel.alpha = System.Convert.ToSingle((activePanel.Equals(UserScorePanel.name)));
        }

        #endregion

    }
}
