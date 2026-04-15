using UnityEngine;
using TMPro;

namespace SojaExiles
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interact setting")]
        public float interactRange = 3f;
        public LayerMask interactLayer;
        public KeyCode interactKey = KeyCode.E;

        [Header("UI Hint")]
        public GameObject interactPrompt;
        public TextMeshProUGUI promptText;

        private Camera playerCamera;
        private Inventory inventory;
        private GameObject currentInteractable;

        void Start()
        {
            playerCamera = GetComponentInChildren<Camera>();
            inventory = GetComponent<Inventory>();

            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }

        void Update()
        {
            CheckForInteractable();

            if (Input.GetKeyDown(interactKey))
            {
                TryInteract();
            }
        }

        void CheckForInteractable()
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
            {
                GameObject hitObject = hit.collider.gameObject;

                Interactive interactive = hitObject.GetComponent<Interactive>();
                CollectableItem collectable = hitObject.GetComponent<CollectableItem>();

                if (interactive != null || collectable != null)
                {
                    if (currentInteractable != hitObject)
                    {
                        currentInteractable = hitObject;
                        ShowInteractionPrompt(hitObject);
                    }
                    return;
                }
            }

            if (currentInteractable != null)
            {
                currentInteractable = null;
                HideInteractionPrompt();
            }
        }

        void ShowInteractionPrompt(GameObject obj)
        {
            if (interactPrompt == null) return;

            CollectableItem collectable = obj.GetComponent<CollectableItem>();
            Interactive interactive = obj.GetComponent<Interactive>();

            string message = "";

            if (collectable != null && collectable.itemData != null)
            {
                message = $"Press E Collect {collectable.itemData.itemName}";
            }
            else if (interactive != null)
            {
                if (interactive.requireItem != ItemName.Note)
                    message = "Press E using item";
                else
                    message = "Press E Interact";
            }

            interactPrompt.SetActive(true);
            if (promptText != null)
                promptText.text = message;
        }

        void HideInteractionPrompt()
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }

        void TryInteract()
        {
            if (currentInteractable == null) return;

            CollectableItem collectable = currentInteractable.GetComponent<CollectableItem>();
            if (collectable != null)
            {
                collectable.TryCollectItem();
                HideInteractionPrompt();
                return;
            }

            OpenCloseDoor door = currentInteractable.GetComponent<OpenCloseDoor>();

            if (door != null)
            {
                door.InteractWithDoor();
                return;
            }

            Interactive interactive = currentInteractable.GetComponent<Interactive>();
            if (interactive != null)
            {
                if (interactive.requireItem != ItemName.Note && inventory != null)
                {
                    if (inventory.HasItem(interactive.requireItem))
                    {
                        interactive.CheckItem(interactive.requireItem);
                        if (ScoreManager.Instance != null)
                            ScoreManager.Instance.AddScore(20);
                    }
                    else
                    {
                        interactive.EmptyClicked();
                    }
                }
                else
                {
                    interactive.EmptyClicked();
                }
            }
        }
    }
}