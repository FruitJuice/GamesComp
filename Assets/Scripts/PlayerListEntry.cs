using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace Com.NUIGalway.CompGame
{
    public class PlayerListEntry : MonoBehaviour
    {
        [Header("UI References")]
        public Text PlayerNameText;

        public Image PlayerColorImage;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;
        public Dropdown PlayerCharacterSelect;

        private int ownerId;
        private bool isPlayerReady;

        #region UNITY

        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

        public void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
            {
                PlayerReadyButton.gameObject.SetActive(false);
                PlayerCharacterSelect.gameObject.SetActive(false);
            }
            else
            {
                Hashtable initialProps = new Hashtable() { { ClipperGate.PLAYER_READY, isPlayerReady }, { ClipperGate.PLAYER_HEALTH, ClipperGate.PLAYER_MAX_HEALTH }, { ClipperGate.CHOSEN_CHARACTER, ClipperGate.GetCharacter(0) } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
                PhotonNetwork.LocalPlayer.SetScore(0);

                PlayerReadyButton.onClick.AddListener(() =>
                {
                    isPlayerReady = !isPlayerReady;
                    SetPlayerReady(isPlayerReady);

                    Hashtable props = new Hashtable() { { ClipperGate.PLAYER_READY, isPlayerReady } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        FindObjectOfType<Lobby>().LocalPlayerPropertiesUpdated();
                    }
                });

                PlayerCharacterSelect.onValueChanged.AddListener(delegate {
                    SelectedCharacter(PlayerCharacterSelect);
                });

            }
        }

        public void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        #endregion

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
        }

        private void OnPlayerNumberingChanged()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId)
                {
                    PlayerColorImage.color = ClipperGate.GetColor(p.GetPlayerNumber());
                }
            }
        }

        public void SelectedCharacter(Dropdown choice)
        {
            Hashtable props = new Hashtable() { { ClipperGate.CHOSEN_CHARACTER, ClipperGate.GetCharacter(choice.value) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            print(ClipperGate.GetCharacter(choice.value));

            if (PhotonNetwork.IsMasterClient)
            {
                FindObjectOfType<Lobby>().LocalPlayerPropertiesUpdated();
            }
        }

        public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
            PlayerReadyImage.enabled = playerReady;
        }
    }
}