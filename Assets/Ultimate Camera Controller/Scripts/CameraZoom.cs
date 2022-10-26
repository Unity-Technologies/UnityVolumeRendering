//Ultimate Camera Controller - Camera Zoom
//This script is responsible for handling the zoom functionality of the package
//To add zoom functionality to a camera you should just attach this script to the GameObject that contains the camera

using UnityEngine;

namespace UltimateCameraController.Cameras.Controllers
{
	 //We use Require Component attribute to ensure that there is always a camera attached to the game object
	 //so that we can effectively avoid null reference exceptions
	 [RequireComponent(typeof(Camera))]
	 [AddComponentMenu("Ultimate Camera Controller/Camera Zoom")]
	 public class CameraZoom : MonoBehaviour
	 {
		 //We cache the target camera so that we won't need to use 
		 //GetComponent every frame and thus we can boost performance
		 private Camera targetCamera;
	 
		 [Header("Zoom Configuration")] 
		 [Space(10)] 
		 
		 [SerializeField] 
		 [Tooltip("Maximum Zoom Value")]
		 private float zoomMax; //Maximum Zoom Value
		 
		 [SerializeField] 
		 [Tooltip("Minimum Zoom Value")]
		 private float zoomMin; //Minimum Zoom Value
	 
		 [Space(10)]
		 
		 [SerializeField] [Tooltip("Scroll Bar Sensitivity")] [Range(1, 100)]
		 private float sensitivity; // Sensitivity Variable
	 
		 private void Start()
		 {
			 //Here we cache the target Camera
			 targetCamera = GetComponent<Camera>();
		 }
	 
		 private void Update()
		 {
			 //We cache camera's field of view in a temporary variable
			 float fieldOfView = targetCamera.fieldOfView;
	 
			 //Simple calculation to perform the zoom functionality
			 fieldOfView += Input.GetAxis("Mouse ScrollWheel") * sensitivity * -1;
	 
			 //We ensure that the camera's field of view is always within
			 //the specified limits
			 fieldOfView = Mathf.Clamp(fieldOfView, zoomMin, zoomMax);
	 
			 //We assign the temp field of view to the actual FOV in the camera
			 targetCamera.fieldOfView = fieldOfView;
		 }
	 }
}