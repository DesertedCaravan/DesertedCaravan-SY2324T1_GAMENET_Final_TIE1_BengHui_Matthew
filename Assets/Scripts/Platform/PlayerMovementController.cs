using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    // Reference: https://gamedevbeginner.com/how-to-jump-in-unity-with-or-without-physics/

    public float speed = 7.5f;
    public float rotationSpeed = 100f;
    public float currentSpeed = 0f;

    public float jumpAmount = 10f;
    public float gravityScale = 5f;
    
    public bool isControlEnabled;
    public bool airTime;
    public float maxDescent = 60f;
    public float descent;

    private void Start()
    {
        isControlEnabled = false;
        airTime = false;
        descent = maxDescent;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isControlEnabled)
        {
            float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

            transform.Translate(0, 0, translation);
            currentSpeed = translation;

            transform.Rotate(0, rotation, 0);

            if (Input.GetKeyDown(KeyCode.Space) && airTime == false && this.GetComponent<Rigidbody>().velocity.y == 0)
            {
                airTime = true;
            }
            else if (airTime == false && this.GetComponent<Rigidbody>().velocity.y > 0)
            { 
                this.GetComponent<Rigidbody>().AddForce(Physics.gravity * (gravityScale - 1) * this.GetComponent<Rigidbody>().mass);
            }

            if (airTime == true)
            {
                this.GetComponent<Rigidbody>().AddForce(transform.up * jumpAmount);

                descent--;

                if (descent <= 0)
                {
                    descent = maxDescent;
                    airTime = false;
                }
            }
        }
    }
}
