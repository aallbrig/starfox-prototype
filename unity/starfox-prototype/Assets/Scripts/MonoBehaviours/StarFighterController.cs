using System;
using Generated;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonoBehaviours
{
    [Serializable]
    public class PointerPressData
    {
        public static PointerPressData Of(Vector2 position, Vector2 pressRadius) =>
            new PointerPressData(position, pressRadius);

        public Vector2 Position { get; private set; }
        public Vector2 Radius { get; private set; }
        public float Timing { get; private set; }

        private PointerPressData(Vector2 position, Vector2 pressRadius)
        {
            Position = position;
            Radius = pressRadius;
            Timing = Time.time;
        }

        public override string ToString() => $"Position: {Position} Radius: {Radius} Timing: {Timing}";
    }

    // listen for on touch start, on touch end
    // Translate a swipe direction into the yaw, pitch, and roll
    // MVP: rotate the game object based on swipe inputs
    public class StarFighterController : MonoBehaviour
    {
        [SerializeField] private PointerPressData pressStartData;
        [SerializeField] private PointerPressData pressEndData;

        private StarFighterControls _playerActions;
        private void Awake() => _playerActions = new StarFighterControls();
        private void OnEnable() => _playerActions.Enable();
        private void OnDisable() => _playerActions.Disable();
        private void Start()
        {
            _playerActions.Player.Interact.started += HandleInteractStarted;
            _playerActions.Player.Interact.canceled += HandleInteractCancelled;
        }

        private void HandleInteractStarted(InputAction.CallbackContext context)
        {
            pressStartData = PointerPressData.Of(
                _playerActions.Player.Position.ReadValue<Vector2>(),
                _playerActions.Player.Radius.ReadValue<Vector2>()
            );
            Debug.Log("Press start: " + pressStartData);
        }

        private void HandleInteractCancelled(InputAction.CallbackContext context)
        {
            pressEndData = PointerPressData.Of(
                _playerActions.Player.Position.ReadValue<Vector2>(),
                _playerActions.Player.Radius.ReadValue<Vector2>()
            );
            Debug.Log("Press end: " + pressEndData);
        }

    }
}