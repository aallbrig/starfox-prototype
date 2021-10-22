using System;
using System.Collections;
using System.Collections.Generic;
using Generated;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonoBehaviours
{
    public interface IWeapon
    {
        public void Fire();
    }

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

    [Serializable]
    public class SwipeInfo
    {
        public Vector2 Vector { get; private set; }
        public Vector2 Direction { get; private set; }
        public float Timing { get; private set; }
        public static SwipeInfo Of(Vector2 vector, Vector2 direction, float timing) =>
            new SwipeInfo(vector, direction, timing);

        private SwipeInfo(Vector2 vector, Vector2 direction, float timing)
        {
            Vector = vector;
            Direction = direction;
            Timing = timing;
        }

        public override string ToString()
        {
            return $"Direction: {Direction} Vector: {Vector} Timing: {Timing}";
        }
    }

    public class StarFighterController : MonoBehaviour
    {
        public event Action PressEndDataCollected;
        public event Action<SwipeInfo> SwipeInfoCalculated;

        [SerializeField] private float speed = 70f;
        [SerializeField] private PointerPressData pressStartData;
        [SerializeField] private PointerPressData pressEndData;
        [SerializeField] private Vector3 targetLocalPosition;
        private float _fireVersusMoveThreshold = 0.1f;

        private StarFighterControls _playerActions;
        private void Awake() => _playerActions = new StarFighterControls();
        private void OnEnable() => _playerActions.Enable();
        private void OnDisable() => _playerActions.Disable();
        private void Start()
        {
            _playerActions.Player.Interact.started += HandleInteractStarted;
            _playerActions.Player.Interact.canceled += HandleInteractCancelled;
            PressEndDataCollected += CalculateSwipeFacts;
            SwipeInfoCalculated += ManeuverStarFighter;
            SwipeInfoCalculated += DetermineFireCommand;
        }
        private void DetermineFireCommand(SwipeInfo swipeInfo)
        {
            if (swipeInfo.Timing >= _fireVersusMoveThreshold) return;
            Fire();
        }

        private void Update()
        {
            if (targetLocalPosition != Vector3.zero && transform.localPosition != targetLocalPosition)
            {
                transform.localPosition += targetLocalPosition * Time.deltaTime;
            }
        }

        private void ManeuverStarFighter(SwipeInfo swipeInfo)
        {
            if (swipeInfo.Timing <= _fireVersusMoveThreshold) return;

            var newLocalPosition = new Vector3(swipeInfo.Direction.x, swipeInfo.Direction.y, 0) * speed;

            targetLocalPosition = newLocalPosition;

            StartCoroutine(ExecuteAfter(swipeInfo.Timing, ClearTargetLocalPosition));
        }

        private IEnumerator ExecuteAfter(float timing, Action action)
        {
            yield return new WaitForSeconds(timing);

            action?.Invoke();
        }

        private void ClearTargetLocalPosition() => targetLocalPosition = Vector3.zero;

        private void CalculateSwipeFacts()
        {
            var direction = (pressEndData.Position - pressStartData.Position).normalized;
            var vector = pressEndData.Position - pressStartData.Position;
            var swipeTiming = pressEndData.Timing - pressStartData.Timing;
            var swipeInfo = SwipeInfo.Of(vector, direction, swipeTiming);

            Debug.Log(swipeInfo);
            SwipeInfoCalculated?.Invoke(swipeInfo);
        }

        private void HandleInteractStarted(InputAction.CallbackContext context)
        {
            pressStartData = PointerPressData.Of(
                _playerActions.Player.Position.ReadValue<Vector2>(),
                _playerActions.Player.Radius.ReadValue<Vector2>()
            );
        }

        private void HandleInteractCancelled(InputAction.CallbackContext context)
        {
            pressEndData = PointerPressData.Of(
                _playerActions.Player.Position.ReadValue<Vector2>(),
                _playerActions.Player.Radius.ReadValue<Vector2>()
            );

            PressEndDataCollected?.Invoke(); // No point passing in unused args
        }

        public void Fire()
        {
            Debug.Log("Firing!");
            var weapons = GetComponentsInChildren<IWeapon>();

            foreach (var weapon in weapons)
            {
                Debug.Log($"Firing weapon {weapon}");
                weapon.Fire();
            }
        }
    }
}