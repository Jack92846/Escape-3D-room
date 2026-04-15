using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles
{
    public class PlayerMovement : MonoBehaviour
    {
        public CharacterController controller;

        [Header("Movement setting")]
        public float walkSpeed = 5f;
        public float runSpeed = 8f;
        public float gravity = -15f;

        [Header("statu")]
        public bool canMove = true;
        public bool isRunning = false;

        private Vector3 velocity;
        private bool isGrounded;
        private float currentSpeed;

        void Update()
        {
            if (!canMove) return;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            // Running
            isRunning = Input.GetKey(KeyCode.LeftShift) && (x != 0 || z != 0);
            currentSpeed = isRunning ? runSpeed : walkSpeed;

            controller.Move(move * currentSpeed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

        }

        public void DisableMovement()
        {
            canMove = false;
        }

        public void EnableMovement()
        {
            canMove = true;
        }
    }
}