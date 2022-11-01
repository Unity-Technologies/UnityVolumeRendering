//Ultimate Camera Controller - Player Movement
//This script adds some basic movement functionality to the object that it is attached to
//so that you can test the orbit and zoom functionality while the target object is moving
//We have not included comments in this script as it was made only for demonstration purposes

using UnityEngine;

namespace UltimateCameraController.Cameras.Samples.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] 
        private float moveSpeed = 10f;

        private void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }
}