using System;
using UnityEngine;

namespace MonoBehaviours
{
    public class LaserWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] private Transform laserSpawnPoint;
        [SerializeField] private GameObject laserPrefab;

        public void Fire()
        {
            var laser = Instantiate(laserPrefab);
            laser.transform.position = laserSpawnPoint.transform.position;
            laser.transform.rotation = laserSpawnPoint.transform.rotation;
        }
    }
}