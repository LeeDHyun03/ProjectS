using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaterWalkEffectCreator
{
    public abstract class Base : MonoBehaviour
    {
        [SerializeField] private GameObject legMask;
        [SerializeField] private GameObject wavePrefab;
        [SerializeField] private float waveInterval = 0.3f;

        [HideInInspector] public bool inWaterTile;

        private bool wasOnTile = false;
        private float timer;

        protected abstract bool CheckIfInMovementState();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            CheckIfInWater();

            if (CheckIfInMovementState())
            {
                timer -= Time.deltaTime;
                if (inWaterTile && timer <= 0f)
                {
                    SpawnEffect();
                    timer = waveInterval;
                }
            }
        }
        private void CheckIfInWater()
        {
            Vector3 worldPos = transform.position;
            Vector3Int cellPos = WaterTilemap.tilemap.WorldToCell(worldPos);

            inWaterTile = WaterTilemap.tilemap.HasTile(cellPos);

            if (!wasOnTile && inWaterTile)
            {
                OnEnterTile();
            }
            else if (wasOnTile && !inWaterTile)
            {
                OnExitTile();
            }

            wasOnTile = inWaterTile;
        }

        protected void OnEnterTile()
        {
            legMask.SetActive(true);
        }
        protected void OnExitTile()
        {
            legMask.SetActive(false);
        }
        private void SpawnEffect()
        {
            ObjectPooler.Instance.SpawnFromPool("WaterWalkEffect", transform.position - new Vector3(0, 0.2f, 0), Quaternion.identity);
        }
    }
}

