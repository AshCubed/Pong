using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pong
{
    public class BackgroundObjectCreation : MonoBehaviour
    {
        [Tooltip("Collider area where items can spawn")]
        [SerializeField] private BoxCollider2D _colliderArea;
        [Tooltip("Maximum number of objects that can be spawned")]
        [SerializeField] private int _totalObjectsToSpawn;
        [Tooltip("All items which can be spawned")]
        [SerializeField] private List<GameObject> _asteroidsToSpawn;
        
        private List<GameObject> _objectsSpawned;
        private Vector2 _cubeSize;
        private Vector2 _cubeCenter;

        private void Awake()
        {
            var cubeTrans = _colliderArea.gameObject.GetComponent<Transform>();
            _cubeCenter = cubeTrans.position;
 
            // Multiply by scale because it does affect the size of the collider
            var localScale = cubeTrans.localScale;
            var size = _colliderArea.size;
            _cubeSize.x = localScale.x * size.x;    
            _cubeSize.y = localScale.y * size.y;
        }
        
        // Start is called before the first frame update
        private void Start()
        {
            SpawnObjects();
        }

        /// <summary>
        /// Spawns all objects randomly based on settings in this script
        /// </summary>
        private void SpawnObjects()
        {
            if (_objectsSpawned is { Count: > 0 })
            {
                foreach (var item in _objectsSpawned)
                {
                    Destroy(item);
                }
            }
            _objectsSpawned = new List<GameObject>();
            
            for (var i = 0; i < _totalObjectsToSpawn; i++)
            {
                var newGo1 = Instantiate(_asteroidsToSpawn[Random.Range(0, _asteroidsToSpawn.Count)], 
                    GetRandomPosition(), Quaternion.identity, transform);
                newGo1.SetActive(false);
                _objectsSpawned.Add(newGo1);
            }

            StartCoroutine(RevealObjects());
        }

        /// <summary>
        /// Runs through <see cref="_objectsSpawned"/> and sets each active after 1 second
        /// </summary>
        /// <returns></returns>
        private IEnumerator RevealObjects()
        {
            foreach (var item in _objectsSpawned)
            {
                yield return new WaitForSeconds(1f);
                item.SetActive(true);
            }
        }

        /// <summary>
        /// Returns random position within a box collider
        /// </summary>
        /// <returns>Random position</returns>
        private Vector2 GetRandomPosition()
        {
            var randomPosition = new Vector2(
                Random.Range(-_cubeSize.x / 2, _cubeSize.x / 2), 
                Random.Range(-_cubeSize.y / 2, _cubeSize.y / 2));
            return _cubeCenter + randomPosition;
        }
    }
}
