using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerListManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI PlayerNameText;
    public Button PlayerReadyButton;
    public Image PlayerReadyImage;

    private bool isPlayerReady = false;

    public void Initialize(int playerId, string playerName)
    {
        PlayerNameText.text = playerName;

        if (PhotonNetwork.LocalPlayer.ActorNumber != playerId)
        {
            PlayerReadyButton.gameObject.SetActive(false);
        }
        else
        {
            // sets custom property for each player "isPlayerReady"
            ExitGames.Client.Photon.Hashtable initializeProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_READY, isPlayerReady } }; //Constant was previous called "isPlayerReady"
            PhotonNetwork.LocalPlayer.SetCustomProperties(initializeProperties);

            PlayerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);

                ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_READY, isPlayerReady } }; //Constant was previous called "isPlayerReady"
                PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
            });
        }
    }

    public void SetPlayerReady(bool playerReady)
    {
        PlayerReadyImage.enabled = playerReady;

        if (playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "READY!";
        }
        else
        {
            PlayerReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready?";
        }
    }
}
