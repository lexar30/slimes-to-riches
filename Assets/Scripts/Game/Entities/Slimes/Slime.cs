using Unity.ProjectAuditor.Editor;
using UnityEngine;

namespace SlimesToRiches.Arena.Entities.Slimes
{
    public class Slime : MonoBehaviour
    {
        private int maxHp = 0;
        private int currentHp = 0;

        private Vector2 normalizedPos = Vector2.zero;
        private Vector2 normalizedTargetPos = Vector2.zero;
        private float speed = 0.0f;
        private float idlingTime = 1.3f;
        private float idlingTimer = 0.0f;
        private bool isIdling = false;

        public RectTransform ArenaRect;

        private void ChooseNextTargetPos()
        {
            normalizedTargetPos.x = Random.Range(0.0f, 1.0f);
            normalizedTargetPos.y = Random.Range(0.0f, 1.0f);
        }

        private void Move(float dt)
        {
            float step = speed * dt;
            //Vector2 nextNormalizedPos = Vector2.MoveTowards(
            //    new Vector2(normalizedPos.x * ArenaRect.rect.width, normalizedPos.y * ArenaRect.rect.height)
            //    , new Vector2(normalizedTargetPos.x * ArenaRect.rect.width, normalizedTargetPos.y * ArenaRect.rect.height)
            //    , step
            //);

            normalizedPos = Vector2.MoveTowards(normalizedPos, normalizedTargetPos, step);

            RectTransform rect = this.GetComponent<RectTransform>();
            float halfWidth = rect.rect.width * 0.5f;
            float halfHeight = rect.rect.height * 0.5f;

            rect.anchoredPosition = new Vector2(
                Mathf.Lerp(ArenaRect.rect.xMin + halfWidth, ArenaRect.rect.xMax - halfWidth, normalizedPos.x),
                Mathf.Lerp(ArenaRect.rect.yMin + halfHeight, ArenaRect.rect.yMax - halfHeight, normalizedPos.y)
            );
        }

        private void Start()
        {
            speed = 0.5f;
            maxHp = currentHp = 1;
            ChooseNextTargetPos();

            normalizedPos.x = this.transform.position.x / ArenaRect.rect.width;
            normalizedPos.y = this.transform.position.y / ArenaRect.rect.height;
        }

        private void Update()
        {
            if (isIdling)
            {
                idlingTimer -= Time.deltaTime;
                if (idlingTimer <= 0.01f)
                {
                    isIdling = false;
                    ChooseNextTargetPos();
                    return;
                }
            }

            Move(Time.deltaTime);

            if (Vector2.Distance(normalizedPos, normalizedTargetPos) < 0.01f)
            {
                idlingTimer = idlingTime;
                isIdling = true;
            }
        }
    }
}
