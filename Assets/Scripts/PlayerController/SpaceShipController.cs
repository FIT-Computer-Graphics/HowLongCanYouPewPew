using UnityEngine;

namespace Scripts.PlayerController
{
    public class SpaceShipController : MonoBehaviour
    {
        [SerializeField] private float forwardSpeed;
        [SerializeField] private float hoverSpeed;
        private float activeForwardSpeed, activeHoverSpeed;
        private const float ForwardAcceleration = 2.5f;
        private const float HoverAcceleration = 2;

        [SerializeField] private float lookRateSpeed = 90f;
        private Vector2 lookInput, screenCenter, mouseDistance;

        private float rollInput;

        private const float RollSpeed = 90f;
        private const float RollAcceleration = 3.5f;
        private bool isPaused;

        private void Start()
        {
            screenCenter.x = Screen.width * .5f;
            screenCenter.y = Screen.height * .5f;
            LockCursor();

        }

        private void Update()
        {
            if (CheckPause()) return;
            CalculateMovement();
        }

        private void CalculateMovement()
        {
            lookInput.x = Input.mousePosition.x;
            lookInput.y = Input.mousePosition.y;

            mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
            mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;
            mouseDistance = Vector2.ClampMagnitude(mouseDistance, 0.8f);

            rollInput = Mathf.Lerp(rollInput, -Input.GetAxis("Horizontal"), RollAcceleration * Time.deltaTime);

            transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput * RollSpeed * Time.deltaTime, Space.Self);

            activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxis("Vertical") * forwardSpeed, ForwardAcceleration * Time.deltaTime);
            
            activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxis("Hover") * hoverSpeed, HoverAcceleration);

            var transform1 = transform;
            transform1.position += activeForwardSpeed * Time.deltaTime * transform1.forward +
                                   activeHoverSpeed * Time.deltaTime * transform1.up;
        }
        private bool CheckPause()
        {
            switch (isPaused)
            {
                case true:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        isPaused = false;
                        LockCursor();
                    }
                    else return true;

                    break;
                }
                case false when Input.GetKeyDown(KeyCode.Escape):
                    isPaused = true;
                    UnlockCursor();
                    break;
            }
            return false;
        }
        
        private static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
    }
}
