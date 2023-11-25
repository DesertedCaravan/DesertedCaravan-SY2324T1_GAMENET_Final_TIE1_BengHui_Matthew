using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject playerNameText;
    public GameObject enemyMarker;

    // Start is called before the first frame update
    void Start()
    {
        // Make name visible to other players
        playerNameText.GetComponent<TextMeshProUGUI>().text = photonView.Owner.NickName;
        this.playerNameText.SetActive(true); // Must be visible for all

        this.enemyMarker.SetActive(!photonView.IsMine);

        // GetComponent<PlayerMovementController>().enabled = photonView.IsMine; // enable Player Movement only if it's the client's Player Movement
        GetComponent<CountdownManager>().enabled = true;
        GetComponent<TileActivation>().enabled = true;
    }
}