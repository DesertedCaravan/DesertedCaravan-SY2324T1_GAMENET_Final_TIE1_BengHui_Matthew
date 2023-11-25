using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlatformGameManager : MonoBehaviourPunCallbacks
{
    [Header("Player Spawning")]
    [SerializeField] public TextMeshProUGUI timerText;
    public GameObject[] playerPrefabs;
    public Transform[] startingPositions;

    [Header("WinText")]
    public TextMeshProUGUI winText;

    [Header("Quit Game")]
    public GameObject quitGameButton;

    // Convert to Singleton
    public static PlatformGameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        // DontDestroyOnLoad(gameObject); // Makes it a persistent GameObject
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                // Debug.Log((int)playerSelectionNumber);

                // get Actor Number of Local Player (is unique across all players)
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;
                Quaternion instantiateRotation = startingPositions[actorNumber - 1].rotation;

                PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, instantiateRotation);
            }
        }

        quitGameButton.SetActive(false);
    }

    public void DisplayQuitButton()
    {
        quitGameButton.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void LeaveRoom()
    {
        quitGameButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }
}
