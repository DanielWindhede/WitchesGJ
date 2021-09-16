using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Rhythm
{

    public class NoteSystem : MonoBehaviour
    {
        [System.Serializable]
        public struct TargetPositions
        {
            [HideInInspector] public Vector2 Up { get { return up.position; } }
            [HideInInspector] public Vector2 Right { get { return right.position; } }
            [HideInInspector] public Vector2 Down { get { return down.position; } }
            [HideInInspector] public Vector2 Left { get { return left.position; } }

            [SerializeField] private Transform up;
            [SerializeField] private Transform right;
            [SerializeField] private Transform down;
            [SerializeField] private Transform left;
        }

        private Dictionary<Note.Direction, HashSet<Note>> notes = new Dictionary<Note.Direction, HashSet<Note>>()
        {
            { Note.Direction.Up,    new HashSet<Note>() },
            { Note.Direction.Right, new HashSet<Note>() },
            { Note.Direction.Down,  new HashSet<Note>() },
            { Note.Direction.Left,  new HashSet<Note>() },
        };

        [SerializeField] private List<Note> noteCollection; // DEBUG
        [SerializeField] private float _inputLeniency = 0.3f;
        [SerializeField] private float _lerpDuration = 3;
        [SerializeField] private float _indicatorSize = 25;
        [SerializeField] private bool _debugMode = true;
        [SerializeField] private AudioSource _audio;
        [SerializeField] private GameObject exampleNote;
        [SerializeField] private Text sampleText;
        [SerializeField] private Canvas canvas;
        [SerializeField] private TargetPositions targetPositions;

        private bool _play;
        private float _audioTime;

        public float InputLeniency { get { return _inputLeniency; } }
        public float LerpDuration { get { return _lerpDuration; } }
        public float IndicatorSize
        { 
            get
            {
                var dir = Note.Direction.Up;
                Vector2 initialPosition = GetTargetPosition(dir) + NoteUtility.DirectionToVector(dir) * 100;

                Vector2 targetPosition = GetTargetPosition(dir) + NoteUtility.DirectionToVector(dir) * _indicatorSize;

                //float finalLerpDuration = (note.end - note.start);
                float finalLerpDuration = InputLeniency; // InputLeniency
                float initialLerpDuration = _lerpDuration - finalLerpDuration;

                Vector2 delta = targetPosition - initialPosition;
                float extraDistance = (delta.magnitude / initialLerpDuration * finalLerpDuration);

                return _indicatorSize;
                return extraDistance;
            } 
        } // TODO: use InputLeniency


        void Start()
        {
            //AddNote(new Note(startSegment, _audio.clip.samples / 1024));
        }

        private void AddNote(Note note)
        {
            notes[note.direction].Add(note);
        }

        public void Initialize()
        {
            foreach (var note in noteCollection)
            {
                AddNote(new Note(note.start, InputLeniency, note.direction));
            }
        }

        void Update()
        {
            if (_debugMode && UnityEngine.Input.GetKeyDown(KeyCode.R))
            {
                _audio.Stop();

                if (notes.Count > 0)
                {
                    foreach (var item in notes)
                    {
                        foreach (var note in notes[item.Key])
                        {
                            Destroy(note.gameObject);
                        }
                        notes[item.Key].Clear();
                    }
                }

                Initialize();

                _play = true;
                _audioTime = -_lerpDuration;
            }

            if (_play)
            {
                HandleTimer();

                InstantiateNotes(_audioTime);

                Vector2 controlDirection = Vector2.zero;

                if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
                    controlDirection = Vector2.up;
                else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
                    controlDirection = Vector2.down;
                else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
                    controlDirection = Vector2.right;
                else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
                    controlDirection = Vector2.left;



                foreach (Note.Direction dir in Enum.GetValues(typeof(Note.Direction)))
                {
                    //var noteList = notes[dir].Where(x => x.start <= sample && x.end >= sample && x.state == Note.NoteState.Idle && !x.gameObject).ToList();
                    var noteList = notes[dir].Where(x => x.start <= _audioTime && x.end >= _audioTime && x.gameObject && x.gameObject.GetComponent<Image>().color == Color.white).ToList();

                    foreach (var note in noteList)
                    {
                        note.gameObject.GetComponent<Image>().color = Color.yellow;
                    }
                }



                if (controlDirection != Vector2.zero)
                {
                    MarkNotesAtSample(_audioTime, NoteUtility.VectorToDirection(controlDirection));
                }

                RemoveNotes(_audioTime);
                sampleText.text = "Sample: " + _audioTime;
            }
        }

        private void HandleTimer()
        {
            if (_audioTime < 0)
                _audioTime += Time.deltaTime;
            else if (_audioTime >= 0 && !_audio.isPlaying)
            {
                _audioTime = 0;
                _audio.Play();
            }
            else
            {
                _audioTime = _audio.time;
            }
        }

        private void InstantiateNotes(float time)
        {
            foreach (Note.Direction dir in Enum.GetValues(typeof(Note.Direction)))
            {
                //var noteList = notes[dir].Where(x => x.start <= sample && x.end >= sample && x.state == Note.NoteState.Idle && !x.gameObject).ToList();
                var noteList = notes[dir].Where(x => x.start - _lerpDuration + (x.end - x.start) / 2 <= time && x.end >= time && !x.gameObject).ToList();

                foreach (var note in noteList)
                {
                    var obj = Instantiate(exampleNote, canvas.transform);
                    obj.GetComponent<RectTransform>().position = GetTargetPosition(dir) + NoteUtility.DirectionToVector(dir) * 100;

                    Vector2 targetPosition = GetTargetPosition(dir) + NoteUtility.DirectionToVector(dir) * IndicatorSize;

                    //float finalLerpDuration = (note.end - note.start);
                    float finalLerpDuration = (note.end - note.start); // InputLeniency
                    float initialLerpDuration = _lerpDuration - finalLerpDuration;
                    obj.GetComponent<NoteGraphics>().Initialize(initialLerpDuration, finalLerpDuration, IndicatorSize, targetPosition);

                    if (note.state != Note.NoteState.Idle)
                        obj.GetComponent<Image>().color = note.state == Note.NoteState.Hit ? Color.green : Color.red;

                    note.gameObject = obj;
                }
            }
        }

        private Vector2 GetTargetPosition(Note.Direction direction)
        {
            switch (direction)
            {
                case Note.Direction.Up:
                    return targetPositions.Up;
                case Note.Direction.Right:
                    return targetPositions.Right;
                case Note.Direction.Down:
                    return targetPositions.Down;
                case Note.Direction.Left:
                    return targetPositions.Left;
                default:
                    return targetPositions.Up;
            }
        }

        private void MarkNotesAtSample(float time, Note.Direction direction)
        {
            var adjacentNotes = notes[direction].Where(x => x.start - _inputLeniency <= time && x.end + _inputLeniency >= time).ToList();
            var hitableNotes = adjacentNotes.Where(x => x.start <= time && x.end >= time).ToList();
            foreach (var note in adjacentNotes)
            {
                if (hitableNotes.Contains(note))
                {
                    note.state = Note.NoteState.Hit;
                    if (note.gameObject)
                        note.gameObject.GetComponent<Image>().color = Color.green;
                    Hit(note);
                }
                else
                {
                    note.state = Note.NoteState.Miss;
                    if (note.gameObject)
                        note.gameObject.GetComponent<Image>().color = Color.red;
                    Miss();
                }
            }
        }
        private void RemoveNotes(float time)
        {
            foreach (Note.Direction dir in Enum.GetValues(typeof(Note.Direction)))
            {
                //var list = notes[dir].Where(x => x.state != Note.NoteState.Idle || x.end < sample).ToList();
                var list = notes[dir].Where(x => x.end < time).ToList();
                if (list.Count > 0)
                {
                    foreach (var n in list)
                    {
                        if (n.state == Note.NoteState.Idle)
                            Miss();
                        DeleteNote(n);
                    }
                }
            }
        }

        private void Hit(Note note)
        {
            print("hit!");
            DeleteNote(note);
        }

        private void Miss()
        {
            print("miss!");
        }

        private void DeleteNote(Note note)
        {
            Destroy(note.gameObject);
            notes[note.direction].Remove(note);
        }

        private void OnDrawGizmos()
        {
            float radius = 5f;
            Gizmos.DrawWireSphere(targetPositions.Up + Vector2.up * radius / 2, radius);
            Gizmos.DrawWireSphere(targetPositions.Right + Vector2.right * radius / 2, radius);
            Gizmos.DrawWireSphere(targetPositions.Down + Vector2.down * radius / 2, radius);
            Gizmos.DrawWireSphere(targetPositions.Left + Vector2.left * radius / 2, radius);
        }
    }
}
