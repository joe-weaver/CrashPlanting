using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// Our interface with player movement
	public CharacterController controller;

	// A reference to a transform for the camera so we can calulate our move direction
	public Transform camera;

	// Player movement speed
	public float speed = 6f;
	public float turnSmoothTime = 0.1f;
	float currentSmoothVelocity;

	// Gravity
	public float gravity = 9.8f;
	public float jumpSpeed = 10f;
	float vertical = 0.0f;

	// Ground Check
	public Transform groundCheck;
	public LayerMask groundLayer;

    // Update is called once per frame
    void Update()
    {
		float forward = Input.GetAxisRaw("Horizontal");
		float strafe = Input.GetAxisRaw("Vertical");

		bool pressedJump = Input.GetButtonDown("Jump");
		bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundLayer);

		// If we aren't grounded, move downwards from gravity
		if(!isGrounded) {
			vertical -= gravity * Time.deltaTime;
		}

		if(pressedJump && isGrounded) {
			// Pressed jump and isGrounded, so jump
			vertical = 1f * jumpSpeed;
		}

		// Player ground movement
		Vector3 groundMovement = Vector3.zero;

		Vector3 direction = new Vector3(forward, 0, strafe).normalized;

		if(direction.magnitude >= 0.1f) {
			// Get the angle our movement & camera is pointing in
			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;

			// Get the smoothed angle based on smoothing between our current angle and the desired angle
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentSmoothVelocity, turnSmoothTime);

			// Rotate around the y-axis
			transform.rotation = Quaternion.Euler(0f, angle, 0f);

			// rotate our current forward direction by the target angle
			Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

			groundMovement = moveDir.normalized;			
		}

		// Add vertical movement and ground movement
		Vector3 movement = groundMovement * speed + Vector3.up * vertical;

		// Move the player
		controller.Move(movement * Time.deltaTime);
	}

	private void OnTriggerEnter(Collider other) {
		if(other.tag.Equals("Vine")) {
			print("Colliding with vine");
		}
	}
}
