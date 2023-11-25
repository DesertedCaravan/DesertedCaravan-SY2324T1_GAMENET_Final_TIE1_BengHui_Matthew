using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TileVanish : MonoBehaviour
{
    [SerializeField] private Material startingMaterial;
    [SerializeField] private Material lightWarningMaterial;
    [SerializeField] private Material warningMaterial;
    [SerializeField] private Material critWarningMaterial;

    public float countdown;
    public float threequarterpoint;
    public float halfpoint;
    public float quarterpoint;

    public float divide;
    public bool vanish;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("tvr"))
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("Slow"))
            {
                countdown = 5.0f;
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("Medium"))
            {
                countdown = 3.0f;
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("Fast"))
            {
                countdown = 1.0f;
            }
        }

        threequarterpoint = countdown * 0.75f;
        halfpoint = countdown / 2;
        quarterpoint = countdown / 4;

        divide = 0.5f;
        vanish = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (vanish == true)
        {
            countdown -= divide * Time.deltaTime;
        }

        if (countdown <= 0)
        {
            Destroy(this.gameObject); // Destroy(this) only destroys this component
        }
        else if (countdown <= quarterpoint)
        {
            this.GetComponent<MeshRenderer>().sharedMaterial = critWarningMaterial;
        }
        else if (countdown <= halfpoint)
        {
            this.GetComponent<MeshRenderer>().sharedMaterial = warningMaterial;
        }
        else if (countdown <= threequarterpoint)
        {
            this.GetComponent<MeshRenderer>().sharedMaterial = lightWarningMaterial;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // collision.gameObject.GetComponent<PlayerMovementController>() != null
        {
            if (collision.gameObject.GetComponent<TileActivation>().vulnerable == true)
            {
                vanish = true;
            }
        }
    }
}
