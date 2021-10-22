using UnityEngine;

namespace MonoBehaviours
{
    public class LaserProjectile : MonoBehaviour
    {
        public float speed = 20f;
        private float _lifetimeInSeconds = 5f;
        private float _spawnTime;

        private void Start()
        {
            _spawnTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - _spawnTime > _lifetimeInSeconds)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }
    }
}