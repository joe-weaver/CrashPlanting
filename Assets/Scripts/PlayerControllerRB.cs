using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerRB : MonoBehaviour
{
	// The rigidbody for our player
	private Rigidbody body;

	// A reference to a transform for the camera so we can calulate our move direction
	public Transform cameraTransform;

	// A target for the camera that we can smooth over recent player positions to avoid jittering
	public Transform cameraTarget;
	private Vector3[] prevPositions = new Vector3[20];

	// Player movement direction
	Vector3 movementDirection;

	// Player lookAt direction (really just a copy of the last non-zero movement direction)
	Vector3 lookAt;

	// Player movement speed
	public float speed = 6f;
	public float climbSpeed = 2f;
	public float turnSmoothTime = 0.1f;
	float currentSmoothVelocity;

	// Gravity
	public float jumpHeight = 1f;

	// Ground Check
	public Transform groundCheck;
	public LayerMask groundLayer;
	bool wasGrounded = true;
	bool isGrounded = false;
	public LayerMask plantLayer;

	public LayerMask waterLayer;

	// Animations
	Animator animator;
	const float animationSmoothTime = 0.1f;

	// Sounds
	public float timeBetweenSteps = 0.5f;
	private float timeSinceLastStep = 0f;
	public float landSpeedSoundCutoff = 1f;

	/* ---------- PLANT INTERACTIONS ---------- */
	// Inventory
	public InventoryManager inventory;

	// Planting
	public Transform plantGroundCheck;
	public GhostPlant[] ghostPlants;
	bool isPlanting = false;
	bool canPlant = false;
	private GhostPlant ghostPlant = null;
	private Vector3 plantGroundPoint;
	public Material invalidMat;
	public Material validMat;

	// Vine
	bool collidingWithVine = false;
	bool climbingVine = false;
	float climbDirection = 0f;

	// Pumpkin (and other plants you can hold?)
	bool holdingPlant = false;
	PickupPlant heldPlant;
	public Transform pickupChecker;
	public float pickupRadius = 0.5f;
	public LayerMask pickupLayer;
	public float throwSpeed = 5f;
	
	// Mushroom
	public LayerMask mushroomLayer;

	// Dragon's Breath
	Burnable[] burnables;
	/* ---------------------------------------- */

	void Start() {
		body = GetComponent<Rigidbody>();
		burnables = FindObjectsOfType<Burnable>();

		foreach(Burnable burnable in burnables) {
			burnable.SetBurnables(burnables);
		}

		//print(burnables.Length);

		// Get the animator - it is on the model, which is a child of this component
		animator = GetComponentInChildren<Animator>();

		//Set static materials for the ghost plant
		GhostPlant.invalidMat = invalidMat;
		GhostPlant.validMat = validMat;

		print("GROUND: " + groundLayer.value);
		print("PLANT: " + plantLayer.value);
	}

	void Update() {
		isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(0.4f, 0.2f, 0.4f), Quaternion.identity, groundLayer.value);

		GetMovmentDirection();

		// Get the updated planting state and current plant
		HandleChangePlantingState();

		// 
		HandlePlantRecall();

		// Handle the player actions depending on the state
		if(isPlanting) {
			HandlePlacePlant();
		} else {
			if(!holdingPlant) {
				HandlePickUpPlant();
			} else {
				HandleDropPlant();
			}
		}

		// Before checking for a jump, reset the jump trigger of the animator (if we're grounded)
		if(isGrounded) {
			animator.ResetTrigger("jump");
		}

		// Handle Climbing Vine
		if((Input.GetButton("Jump") || Input.GetKey(KeyCode.LeftShift)) && collidingWithVine) {
			climbingVine = true;

			// If jump key is pressed, we're climbing up. Otherwise, we're climbing down
			climbDirection = Input.GetButton("Jump") ? 1f : -1f;
		} else {
			climbingVine = false;

			// Handle Jump
			if(Input.GetButtonDown("Jump") && isGrounded) {
				body.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);

				// Tell the animator we jumped
				animator.SetTrigger("jump");
			}
		}

		// Do animations
		// Set the speed percent based on whether we're moving or not (we don't have run vs walk)
		animator.SetFloat("speedPercent", movementDirection.x != 0f ? 1f : 0f, animationSmoothTime, Time.deltaTime);
		// Set the holding status of the player
		if(holdingPlant && heldPlant is ElephantEar) {
			// We are holding the plant up
			animator.SetBool("holdingUp", true);
		} else if(holdingPlant) {
			animator.SetBool("holding", true);
		} else {
			// We are not holding a plant
			animator.SetBool("holding", false);
			animator.SetBool("holdingUp", false);
		}
		// Set the grounded status of the player
		if(!isGrounded) {
			animator.SetBool("falling", true);
		} else {
			animator.SetBool("falling", false);
		}

		// Do sounds
		timeSinceLastStep += Time.deltaTime;
		if(isGrounded && (Mathf.Abs(movementDirection.x) > 0.001 || Mathf.Abs(movementDirection.z) > 0.001) && timeSinceLastStep > timeBetweenSteps) {
			// Play the step sound
			FindObjectOfType<AudioManager>().Play("Step");
			timeSinceLastStep = 0f;
		}

		if(isGrounded && !wasGrounded) {
			wasGrounded = true;
			print("Landed");
			// Play a sound if our velocity was high enough
			if(Mathf.Abs(GetComponent<Bounceable>().getFallSpeed()) > landSpeedSoundCutoff) {
				FindObjectOfType<AudioManager>().Play("Land");
			}
		} else if(!isGrounded) {
			wasGrounded = false;
		}

	}

	void FixedUpdate() {
		// Move the player.
		Vector3 vel = movementDirection.normalized * speed + (climbingVine ? Vector3.up * climbSpeed * climbDirection : Vector3.zero);
		body.MovePosition(body.position + vel * Time.fixedDeltaTime);
/*
        // Handle landing on mushroom
        bool landedOnMushroom = Physics.CheckBox(groundCheck.position, new Vector3(0.4f, 0.2f, 0.4f), Quaternion.identity, mushroomLayer);
        if (landedOnMushroom && body.velocity.y < 0)
        {
            // If on mushroom, bounce
            body.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -8f * Physics.gravity.y), ForceMode.VelocityChange);
        }
*/
        // Apply float if holding elephant ear
        if (holdingPlant && heldPlant != null && heldPlant is ElephantEar) {
			if(body.velocity.y < 0) {
				body.AddForce(new Vector3(0f, 8f, 0f));
			}
		}

		// Handle moving the plantGroundCheck up or down depending on the terrain
		HandlePlantGroundPoint();

		// Update the list of positions
		int index = prevPositions.Length - 2;

		while(index > -1) {
			prevPositions[index + 1] = prevPositions[index];
			index -= 1;
		}

		prevPositions[0] = transform.position;

		// Update the position of the camera target
		index = 0;
		Vector3 sum = Vector3.zero;

		while(index < prevPositions.Length && prevPositions[index] != null) {
			sum += prevPositions[index];
			index += 1;
		}

		cameraTarget.position = sum / index;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag.Equals("Vine"))
		{
			collidingWithVine = true;

			// While climbing a vine, gravity does not apply to us.
			body.useGravity = false;
			body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
		}
		if(other.tag.Equals("Seed")) {
			int index = other.GetComponent<SeedPacket>().plantType;
			if(inventory.isUnlocked(index)) {
				// If already unlocked, just add one
				inventory.addMaxCount(index);
			} else {
				inventory.unlockPlant(index);
			}

			// Hide the seed packet
			other.gameObject.SetActive(false);

			// Play a sound
			FindObjectOfType<AudioManager>().Play("Collect");
		}
	}

	void OnTriggerExit(Collider other) {
		if(other.tag.Equals("Vine")) {
			// No longer climbing vine. Gravity applies again.
			collidingWithVine = false;
			body.useGravity = true;
		}
	}

	// Change the selected plant and the ghost plant if planting
	void ChangeSelectedPlant() {
		if(isPlanting) {
			Destroy(ghostPlant.gameObject);
			CreateGhostPlant();
		}
    }

	// Toggle value of isPlanting and create the ghost plant if planting
	void TogglePlanting() {
		print("Toggle planting" + canPlant);

		// If we're holding a plant, we can't plant
		if(holdingPlant) {
			return;
		}

		if(isPlanting) {
			isPlanting = false;
			Destroy(ghostPlant.gameObject);
         } else {
			isPlanting = true;
			CreateGhostPlant();
		}
    }

	void CreateGhostPlant() {
		if(isPlanting) {
			GhostPlant plant = ghostPlants[inventory.getSelection()];
			ghostPlant = Instantiate(plant, plantGroundCheck.position, transform.rotation, transform);
			ghostPlant.setNotInitiallyOnPlantableGround(!canPlant);
			ghostPlant.inventory = inventory;
		}
	}

	void GetMovmentDirection() {
		movementDirection = Vector3.zero;
		movementDirection.x = Input.GetAxisRaw("Horizontal");
		movementDirection.z = Input.GetAxisRaw("Vertical");

		// Handle Rotation and extract movement for FixedUpdated
		if(movementDirection != Vector3.zero) {
			// Get the angle our movement & camera is pointing in
			float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

			// Get the smoothed angle based on smoothing between our current angle and the desired angle
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentSmoothVelocity, turnSmoothTime);

			// Rotate around the y-axis
			transform.rotation = Quaternion.Euler(0f, angle, 0f);

			// Update the movement direction by rotating our current forward vector
			movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

			lookAt = transform.rotation * Vector3.forward;
		}
	}

	void HandleChangePlantingState() {
		// Handle Entering Planting Mode - Only allow if we've picked up a plant
		if(Input.GetKeyDown(KeyCode.Tab) && inventory.canPlant()) {
			TogglePlanting();
		}

		int prevSelectedPlant = inventory.getSelection();

		int index = -1;

		//KeyCode[] keyCodesToCheck = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8 };
		KeyCode[] keyCodesToCheck = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };
		// Extract the keycode as an index 0 - 8
		foreach(KeyCode keyCode in keyCodesToCheck) {
			if(Input.GetKeyDown(keyCode)) {
				// Key is down, set the key code
				index = (int)keyCode - (int)KeyCode.Alpha1;
				break;
			}
		}

		if(index > -1) {
			// If we found an index, check if it's unlocked, then set it as selected
			if(inventory.isUnlocked(index)) {
				inventory.setSelection(index);
			}
		}

		// Handle scroll events
		float scroll = Input.mouseScrollDelta.y;

		if(scroll != 0) {
			// We scrolled, so handle that
			inventory.scroll(Mathf.Sign(scroll));
		}

		// Check if the selected plant changed
		if(prevSelectedPlant != inventory.getSelection()) {
			ChangeSelectedPlant();
		}
	}

	// Handle moving the plantGroundCheck up or down depending on the terrain
	void HandlePlantGroundPoint() {
		bool prevCanPlant = canPlant;

		if (isPlanting) {
			RaycastHit ground;

			// Manual check for lilypad
			LayerMask layer = inventory.getSelection() == 5 ? waterLayer : groundLayer;

			// Try to raycast to the ground infront of player
			if (Physics.Raycast(plantGroundCheck.position, Vector3.down, out ground, 2f, layer)) {
				// If raycast hits, move plant point to ground
				if(inventory.getSelection() == 5 && ground.transform.gameObject.layer == LayerMask.NameToLayer("Water")) {
					canPlant = isGrounded;
				} else if(ground.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
					canPlant = isGrounded;
				} else {
					canPlant = false;
				}

				plantGroundPoint = ground.point;

			} else {
				//If unsuccessful, disable planting and move the ghost plant back to infront of the player
				plantGroundPoint = new Vector3(plantGroundCheck.position.x, transform.position.y - .75f, plantGroundCheck.position.z);
				canPlant = false;
			}

			// If we were successful, but have no plants left, it is also invalid
			if(!inventory.canPlantSelected()) {
				canPlant = false;
			}

			//Update position of ghost plant
			ghostPlant.transform.position = plantGroundPoint;

			//Change ghost plant color if necessary
			if (canPlant && !prevCanPlant)
				ghostPlant.SetValidMaterial(false);
			else if(!canPlant && prevCanPlant)
				ghostPlant.SetInvalidMaterial(false);
		}

		
	}

	void HandlePlacePlant() {
		// Handle Planting
		if(Input.GetKeyDown(KeyCode.E) && canPlant && isPlanting) {
			//If planting is successful, toggle planting off
			if(ghostPlant.Plant(transform, plantGroundPoint, burnables)) {
				TogglePlanting();

				// Update the inventory
				inventory.placeSelected();

				// Play a sound
				FindObjectOfType<AudioManager>().Play("Plant");
			}
		}
	}

	void HandlePickUpPlant() {
		// Handle picking up plant
		if(Input.GetKeyDown(KeyCode.E)) {
			// Check if a viable plant is in front of the player
			float dist = Vector3.Distance(transform.position, pickupChecker.position);

			RaycastHit hitInfo;
			Physics.SphereCast(transform.position, 0.3f, pickupChecker.position - transform.position, out hitInfo, dist, pickupLayer.value);

			if(hitInfo.rigidbody != null && hitInfo.rigidbody.gameObject.TryGetComponent(out PickupPlant plant)) {
				// We got a hit
				heldPlant = plant;
				heldPlant.Lift(transform);
				holdingPlant = true;
			}
		}
	}

	void HandlePlantRecall() {
		//
    }

	void HandleDropPlant() {
		if(Input.GetKeyDown(KeyCode.E)) {
			holdingPlant = !heldPlant.Drop(transform, movementDirection, body.velocity, isGrounded, throwSpeed);
		}
	}

	public bool HoldingPumpkin() {
		return holdingPlant && heldPlant is Pumpkin;
	}
}
