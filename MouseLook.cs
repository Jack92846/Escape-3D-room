using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles
{
    public class MouseLook : MonoBehaviour
    {
        [Header("Sensitivity setting")]
        public float mouseXSensitivity = 100f;
        public float mouseYSensitivity = 100f;

        [Header("Vision limit")]
        public float minY = -90f;
        public float maxY = 90f;

        public Transform playerBody;

        [Header("Statu")]
        public bool canLook = true;

        private float xRotation = 0f;
        private bool isCursorLocked = true;

        void Start()
        {
            LockCursor();
        }

        void Update()
        {
            if (!canLook) return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCursorLock();
            }

            if (isCursorLocked)
            {
                HandleMouseLook();
            }
        }

        void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseXSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseYSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minY, maxY);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }

        void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isCursorLocked = true;
        }

        void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isCursorLocked = false;
        }

        void ToggleCursorLock()
        {
            if (isCursorLocked)
                UnlockCursor();
            else
                LockCursor();
        }

        public void SetCursorLock(bool locked)
        {
            if (locked)
                LockCursor();
            else
                UnlockCursor();
        }

        public void DisableLook()
        {
            canLook = false;
            UnlockCursor();
        }

        public void EnableLook()
        {
            canLook = true;
            LockCursor();
        }
    }
}