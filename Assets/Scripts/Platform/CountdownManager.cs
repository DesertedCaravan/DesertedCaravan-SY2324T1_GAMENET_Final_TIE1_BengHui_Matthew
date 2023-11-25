using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText; // No [SerializeField] because it's part of a prefab

    public float timeToStartRace = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.timerText = PlatformGameManager.instance.timerText;  
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (PhotonNetwork.IsMasterClient)
        {
        }
        */

        if (timeToStartRace > 0)
        {
            timeToStartRace -= Time.deltaTime;

            if (photonView.IsMine)
            {
                SetTime(timeToStartRace);
            }
        }
        else if (timeToStartRace < 0)
        {
            StartRace();
        }
    }

    public void SetTime(float time)
    {
        if (time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else
        {
            timerText.text = "";
        }
    }

    public void StartRace()
    {
        GetComponent<PlayerMovementController>().isControlEnabled = photonView.IsMine;
        GetComponent<TileActivation>().vulnerable = true;
        this.enabled = false;
    }
}