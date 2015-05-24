using UnityEngine;
using System.Collections;

namespace Afterward {
    public class SpawnManager : Singleton<SpawnManager> {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Maximal number of simultaneous enemies")]
        [Range (0, 50)]
        float _nbEnemiesMax = 10;
        [SerializeField]
        [Tooltip ("Delay between two spawns")]
        [Range (0, 10)]
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
        [Tooltip ("Enemy prefab")]
        GameObject _enemyPrefab;
        [SerializeField]
        [Tooltip ("Turret prefab")]
        GameObject _turretPrefab;
        #endregion

        #region Unity
        void Start () {
            StartCoroutine ("UpdateSpawn");
        }
        #endregion

        #region Private properties
        float _nbEnemies = 0;
        #endregion

        #region Private methods
        IEnumerator UpdateSpawn () {
            do {
                if (GameManager.instance.enemies.Count < _nbEnemiesMax) {
                    GameManager.instance.AddEnemy (
                        ObjectPool.Spawn (
                            _enemyPrefab,
                            new Vector3 (Random.Range (_spawnXMin, _spawnXMax), 0.25f, Random.Range (_spawnZMin, _spawnZMax)),
                            Quaternion.Euler (Vector3.up * Random.Range (-180, 180))
                        ).GetComponent<Enemy> ()
                    );
                }
                yield return new WaitForSeconds (_spawnDelay / TimeManager.instance.timeScale);
            } while (true);
        }
        #endregion
    }
}