using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    [RequireComponent(typeof(RectTransform))]
    public class Indicators : MonoBehaviour
    {
        public Transform target;
        public Note.Direction direction;

        private NoteSystem _noteSystem;
        private NoteSystem NoteSystem
        {
            get
            {
                if (!_noteSystem)
                    _noteSystem = GameObject.FindObjectOfType<NoteSystem>();

                return _noteSystem;
            }
        }
        private RectTransform _rectTransform;
        private RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }
        void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _noteSystem = GameObject.FindObjectOfType<NoteSystem>();
            SetPosition();
        }

        private void SetPosition()
        {
            if (target)
            {
                RectTransform.sizeDelta = new Vector2(NoteSystem.IndicatorSize, NoteSystem.IndicatorSize);
                //GetComponent<RectTransform>().position = target.position + (Vector3)(NoteUtility.DirectionToVector(direction) * GameObject.FindObjectOfType<NoteSystem>().InputLeniency);
                RectTransform.position = target.position + (Vector3)NoteUtility.DirectionToVector(direction) * (NoteSystem.IndicatorSize / 2); //+ (Vector3)NoteUtility.DirectionToVector(direction) * RectTransform.sizeDelta.x;
            }
        }

        private void OnValidate()
        {
            SetPosition();
        }
    }
}