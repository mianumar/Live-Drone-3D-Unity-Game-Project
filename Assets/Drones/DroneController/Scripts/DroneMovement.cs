using DroneController.Physics;
using UnityEngine;

public class DroneMovement : DroneMovementScript {

    public override void Awake()
    {
        base.Awake(); //I would suggest you to put code below this line or in a Start() method
    }

    void FixedUpdate()
    {

        GetVelocity();
        ClampingSpeedValues();
		SettingControllerToInputSettings(); //sensitivity settings for joystick,keyboard,mobile (depending on which is turned on)
		if (FlightRecorderOverride == false)
		{
			MovementUpDown();
			MovementLeftRight();
			Rotation();
			MovementForward();
			BasicDroneHoverAndRotation(); //this method applies all the forces and rotations to the drone.
		}
	}

    void Update () {
        RotationUpdateLoop_TrickRotation(); //applies rotation to the drone it self when doing the barrel roll trick, does NOT trigger the animation
        Animations(); //part where animations are triggered
        DroneSound(); //sound producing stuff
        CameraCorrectPickAndTranslatingInputToWSAD(); //setting input for keys, translating joystick, mobile inputs as WSAD (depending on which is turned on)
    }

    public void SetMovement(float moveX, float moveY, float moveZ, float rotationY)
    {
        // Movement Left/Right (X-axis)
        if (moveX != 0)
        {
            Debug.Log("I'm here");
            Horizontal_A = moveX < 0 ? 1 : 0; // A key (left)
            Horizontal_D = moveX > 0 ? 1 : 0; // D key (right)

            //Intaj
            if (Horizontal_A > 0)
                A = true;
            //Intaj
            if (Horizontal_D>0)
                D = true;

        }

        // Movement Forward/Backward (Z-axis)
        if (moveY != 0)
        {
            Vertical_W = moveY > 0 ? 1 : 0; // W key (forward)
            Vertical_S = moveY < 0 ? 1 : 0; // S key (backward)
        }

        // Movement Up/Down (Y-axis) (using trigger input for Z axis)
        if (moveZ != 0)
        {
            Vertical_I = moveZ > 0 ? 1 : 0; // Upward movement (I key)
            Vertical_K = moveZ < 0 ? 1 : 0; // Downward movement (K key)
        }

        // Rotation (Yaw: Left/Right)
        if (rotationY != 0)
        {
            Horizontal_J = rotationY < 0 ? 1 : 0; // Rotate left (J key)
            Horizontal_L = rotationY > 0 ? 1 : 0; // Rotate right (L key)
        }

        // You can call existing methods to apply changes to the drone's movement
        MovementLeftRight(); // Handles left-right movement logic
        MovementForward();   // Handles forward-backward movement logic
        MovementUpDown();    // Handles up-down movement logic
        Rotation();          // Applies rotation to the drone
    }

/*    public void SetJoystickCase()
    {
        inputEditorSelection = 2;
    }*/

    /*public void SetMovement(float moveX, float moveY)
    {
        // Set movement for Left/Right (X-axis)
        if (moveX > 0)
        {
            Horizontal_A = 0;
            Horizontal_D = 1; // Move right (D)
        }
        else if (moveX < 0)
        {
            Horizontal_A = 1; // Move left (A)
            Horizontal_D = 0;
        }
        else
        {
            Horizontal_A = 0;
            Horizontal_D = 0;
        }

        // Set movement for Forward/Backward (Y-axis)
        if (moveY > 0)
        {
            Vertical_W = 1; // Move forward (W)
            Vertical_S = 0;
        }
        else if (moveY < 0)
        {
            Vertical_S = 1; // Move backward (S)
            Vertical_W = 0;
        }
        else
        {
            Vertical_W = 0;
            Vertical_S = 0;
        }

        // Apply movement logic (you can call your movement methods here)
        MovementLeftRight(); // Applies left/right movement logic
        MovementForward();   // Applies forward/backward movement logic
    }*/

}
