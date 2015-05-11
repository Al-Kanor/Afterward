using UnityEngine;
using System.Collections;

namespace DiosesModernos {
    public class SpawnManager : Singleton<SpawnManager> {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Delay between two spawns")]
        float _spawnDelay = 1;
        [SerializeField]
        [Tooltip ("Spawn X min")]
        float _spawnXMin = -20;
        [SerializeField]
        [Tooltip ("Spawn X max")]
        float _spawnXMax = 20;
        [SerializeField]
        [Tooltip ("Spawn Z min")]
        float _spawnZMin = -20;
        [SerializeField]
        [Tooltip ("Spawn Z max")]
        float _spawnZMax = 20;

        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Turret prefab")]
        GameObject _turretPrefab;
        #endregion

        #region Unity
        void Start () {
            StartCoroutine ("UpdateSpawn");
        }
        #endregion

        #region Private methods
        IEnumerator UpdateSpawn () {
            do {
                ObjectPool.Spawn (
                    _turretPrefab,
                    new Vector3 (Random.Range (_spawnXMin, _spawnXMax), 1, Random.Range (_spawnZMin, _spawnZMax)),
                    Quaternion.Euler (Vector3.up * Random.Range (-180, 180))
                );
                yield return new WaitForSeconds (_spawnDelay / TimeManager.instance.timeScale);
            } while (true);
        }
        #endregion
    }
}