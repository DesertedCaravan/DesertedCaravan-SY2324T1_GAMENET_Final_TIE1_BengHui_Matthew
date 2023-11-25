using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class TileActivation : MonoBehaviourPunCallbacks
{
    [Header("Status")]
    public bool vulnerable;
    public TextMeshProUGUI winText;
    public int survivors;
    public bool alive;
    public bool winner;
    // Start is called before the first frame update
    void Start()
    {
        vulnerable = false;

        survivors = PhotonNetwork.CurrentRoom.PlayerCount;

        ExitGames.Client.Photon.Hashtable initializeProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.SURVIVORS_LEFT, survivors } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(initializeProperties);

        alive = true;

        winner = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (alive == true && winner == false)
        {
            object survivorsLeft;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.SURVIVORS_LEFT, out survivorsLeft))
            {
                survivors = (int)survivorsLeft;

                if ((survivors <= 1 && photonView.IsMine) || (PhotonNetwork.CurrentRoom.PlayerCount <= 1 && photonView.IsMine))
                {
                    string winner = PhotonNetwork.LocalPlayer.NickName;

                    photonView.RPC("WinState", RpcTarget.AllBuffered, winner);
                }
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (vulnerable == true && alive == true)
        {
            if (this.transform.position.y <= -20)
            {
                Debug.Log("player has been eliminated!");

                photonView.RPC("PlayerEliminated", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void PlayerEliminated()
    {
        alive = false;

        survivors--;

        ExitGames.Client.Photon.Hashtable updateProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.SURVIVORS_LEFT, survivors } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(updateProperties);

        this.GetComponent<PlayerMovementController>().enabled = false;
    }

    [PunRPC]
    public void WinState(string winningPlayer)
    {
        winner = true;

        this.winText = PlatformGameManager.instance.winText;
        winText.GetComponent<TextMeshProUGUI>().text = winningPlayer + " is the winner!";

        PlatformGameManager.instance.DisplayQuitButton();

        // photonView.RPC("DisplayQuitButton", RpcTarget.AllBuffered);
        // StartCoroutine(ClearWinText());
    }

    [PunRPC]
    public void DisplayQuitButton()
    {
        PlatformGameManager.instance.DisplayQuitButton();
    }

    /*
    IEnumerator ClearWinText()
    {
        yield return new WaitForSeconds(3.0f);

        PlatformGameManager.instance.LeaveRoom();
    }
    */
}
