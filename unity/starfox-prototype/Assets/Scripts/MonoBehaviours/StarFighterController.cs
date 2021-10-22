using Generated;
using UnityEngine;

namespace MonoBehaviours
{
    // listen for on touch start, on touch end
    // Translate a swipe direction into the yaw, pitch, and roll
    // MVP: rotate the game object based on swipe inputs
    public class StarFighterController : MonoBehaviour
    {
        private StarFighterControls _playerActions;
        private void Awake() => _playerActions = new StarFighterControls();
        private void OnEnable() => _playerActions.Enable();
        private void OnDisable() => _playerActions.Disable();
    }
}