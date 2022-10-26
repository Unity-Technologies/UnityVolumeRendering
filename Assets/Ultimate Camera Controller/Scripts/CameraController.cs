//Ultimate Camera Controller - Camera Controller
//This script is responsible for following a target object and adding orbit functionality
//to the object that it is attached to
//To make a camera follow or orbit around a target you just need to attach this script to 
//the object that contains the camera or one of its parents

using UnityEngine;

namespace UltimateCameraController.Cameras.Controllers
{
	[AddComponentMenu("Ultimate Camera Controller/Camera Controller")]
	public class CameraController : MonoBehaviour
	{
		[Header("Follow Settings")]
		[Space(10)]
		
		[Tooltip("Should the camera follow the target?")]
		public bool followTargetPosition = true; //Do we want our camera to follow the target object
		
		[Tooltip("The target object our camera should follow or orbit around")]
		public Transform targetObject; //The object that our camera should follow

		[Tooltip("The smooth factor when the camera follows a target object")]
		[Range(0.2f, 1f)]
		public float cameraFollowSmoothness; //The smooth factor when the camera follows a target object

		[Header("Orbit Settings")] 
		[Space(10)] 
		
		[Tooltip("Should the player be able to orbit around the target object?")]
		public bool orbitAroundTarget = true; //Do we want to add orbit functionality to the camera
		
		[Tooltip("The speed by which the camera rotates when orbiting")]
		[Range(2f, 15f)]
		public float rotationSpeed; //The speed by which the camera rotates when orbiting

		[Tooltip("The mouse button that the player must hold in order to orbit the camera")]
		public MouseButtons mouseButton; //The mouse button that the player must hold in order to orbit the camera
		
		private Vector3 _cameraOffset; //How far away is the camera from the target
		
		private void Start()
		{
			//We calculate the distance between the camera and the target
			_cameraOffset = transform.position - targetObject.position;
		}

		//We use late update so that player movement is completed before we move the camera
		//This way we can avoid glitches
		private void LateUpdate()
		{
			//We do an error check
			if (targetObject == null)
			{
				Debug.LogError("Target Object is not assigned. Please assign a target object in the inspector.");
				return;
			}
			
			//If we want the camera to follow the target
			if (followTargetPosition)
			{
				//We set the position the camera should move to, to the sum of the offset and the target's position
				var newPosition = targetObject.position + _cameraOffset;
				//We are moving slowly to the new position. The smooth factors determines how fast 
				//the camera will move to its new postion
				transform.position = Vector3.Slerp(transform.position, newPosition, cameraFollowSmoothness);
			}

			//If we want to make the player able to orbit around the target
			if (orbitAroundTarget)
			{
				//We call the function to orbit the camera
				OrbitCamera();
			}
		}

		//Method to handle Orbit of the Camera
		private void OrbitCamera()
		{
			//If the player holds the selected mouse button
			if (Input.GetMouseButton((int)mouseButton))
			{
				//We cache the mouse rotation values multiplied by the rotation speed
				float y_rotate = Input.GetAxis("Mouse X") * rotationSpeed;
				float x_rotate = Input.GetAxis("Mouse Y") * rotationSpeed;
				
				//We calculate the rotation angles based on the cached values and a specific axes
				Quaternion xAngle = Quaternion.AngleAxis(y_rotate, Vector3.up);
				Quaternion yAngle = Quaternion.AngleAxis(x_rotate, Vector3.left);

				//We multiply the rotation angle by the camera offset 
				_cameraOffset = xAngle * _cameraOffset;
				_cameraOffset = yAngle * _cameraOffset;

				//We make our transform to "look" at the target		
				transform.LookAt(targetObject);
			}
		}
	}

	//Custom enumerator that represents the mouse buttons
	public enum MouseButtons
	{
		LeftButton = 0,
		RightButton = 1,
		ScrollButton = 2
	};
}