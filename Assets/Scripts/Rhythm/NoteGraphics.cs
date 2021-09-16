using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    [RequireComponent(typeof(RectTransform))]
    public class NoteGraphics : MonoBehaviour
    {
        //public Note.Direction direction;
        public float initialLerpDuration;
        public float finalLerpDuration;
        [SerializeField] private Vector2 targetPosition;

        private RectTransform rectTransform;
        private Vector2 initialPosition;
        private Vector2 valueToLerp;
        private float indicatorSize;

        public void Initialize(float initialLerpDuration, float finalLerpDuration, float indicatorSize, Vector2 targetPosition)
        {
            this.rectTransform = GetComponent<RectTransform>();
            this.initialPosition = rectTransform.position;
            this.initialLerpDuration = initialLerpDuration;
            this.finalLerpDuration = finalLerpDuration;
            this.targetPosition = targetPosition;
            this.indicatorSize = indicatorSize;
            StartCoroutine(StartLerp());
        }
        void Update()
        {
            rectTransform.position = valueToLerp;
        }

        IEnumerator StartLerp()
        {
            Vector2 delta = targetPosition - initialPosition;
            Vector2 direction = delta.normalized;
            Vector2 extraDistance = direction * indicatorSize;
            float totalDuration = initialLerpDuration + finalLerpDuration;

            float distance = (targetPosition + extraDistance).magnitude;
            float speed = distance / (initialLerpDuration + finalLerpDuration);
            float t = distance / speed;
            yield return StartCoroutine(LerpToTarget(targetPosition + extraDistance, initialLerpDuration + finalLerpDuration, initialLerpDuration / t));

            this.initialPosition = rectTransform.position;
            yield return StartCoroutine(LerpToTarget(targetPosition + extraDistance, finalLerpDuration / totalDuration));

            //print(targetPosition);
            //yield return StartCoroutine(LerpToTarget(targetPosition, initialLerpDuration));

            //Vector2 delta = targetPosition - initialPosition;
            //Vector2 direction = delta.normalized;
            ////Vector2 extraDistance = direction * (delta.magnitude / initialLerpDuration * finalLerpDuration);
            //Vector2 extraDistance = direction * indicatorSize;
            //this.initialPosition = rectTransform.position;
            //yield return StartCoroutine(LerpToTarget(targetPosition + extraDistance, finalLerpDuration));
        }

        IEnumerator LerpToTarget(Vector2 target, float lerpDuration, float stopAtT = 1)
        {
            float timeElapsed = 0;

            while (timeElapsed < lerpDuration)
            {
                if (timeElapsed >= stopAtT)
                    break;

                valueToLerp = Vector2.Lerp(initialPosition, target, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;

                yield return null;
            }

            valueToLerp = target;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(new Vector3(targetPosition.x, targetPosition.y), 5f);
        }
    }
}