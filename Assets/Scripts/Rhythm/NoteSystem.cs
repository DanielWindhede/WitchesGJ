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
        [SerializeField] private AudioSource _audio;
        [SerializeField] private GameObject exampleNote;
        [SerializeField] private Text sampleText;
        [SerializeField] private Canvas canvas;

        [System.Serializable]
        class Note
        {
            public enum Direction
            {
                Up = 2,
                Right = 1,
                Down = -2,
                Left = -1,
            }

            public enum NoteState
            {
                Idle,
                Hit,
                Miss,
            }

            public Direction direction;
            public int start;
            [HideInInspector] public int end;

            [HideInInspector] public NoteState state;
            [HideInInspector] public GameObject gameObject;
            public Note(int start, Direction direction)
            {
                this.start = start;
                this.end = start + 10;
                this.state = NoteState.Idle;
                this.direction = direction;
            }
        }

        private Dictionary<Note.Direction, HashSet<Note>> notes = new Dictionary<Note.Direction, HashSet<Note>>()
        {
            { Note.Direction.Up,    new HashSet<Note>() },
            { Note.Direction.Right, new HashSet<Note>() },
            { Note.Direction.Down,  new HashSet<Note>() },
            { Note.Direction.Left,  new HashSet<Note>() },
        };

        [SerializeField]
        private List<Note> noteCollection;

        void Start()
        {
            foreach (var note in noteCollection)
            {
                AddNote(new Note(note.start, note.direction));
            }
            //AddNote(new Note(startSegment, _audio.clip.samples / 1024));
        }

        private void AddNote(Note note)
        {
            notes[note.direction].Add(note);
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
            {
                _audio.Play();
            }
            if (_audio.isPlaying)
            {
                int sample = _audio.timeSamples / 1024;

                InstantiateNotes(sample);

                Vector2 controlDirection = Vector2.zero;

                if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
                    controlDirection = Vector2.up;
                else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
                    controlDirection = Vector2.down;
                else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
                    controlDirection = Vector2.right;
                else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
                    controlDirection = Vector2.left;

                if (controlDirection != Vector2.zero)
                {
                    MarkNotesAtSample(sample, VectorToDirection(controlDirection));
                }

                RemoveNotes(sample);
                sampleText.text = "Sample: " + sample;
            }
        }

        private Note.Direction VectorToDirection(Vector2 direction)
        {
            return (Note.Direction)(direction.x + direction.y * 2);
        }
        private Vector2 DirectionToVector(Note.Direction direction)
        {
            switch (direction)
            {
                case Note.Direction.Up:
                    return Vector2.up;
                case Note.Direction.Right:
                    return Vector2.right;
                case Note.Direction.Down:
                    return Vector2.down;
                case Note.Direction.Left:
                    return Vector2.left;
                default:
                    return Vector2.zero;
            }
        }

        private void InstantiateNotes(int sample)
        {
            foreach (Note.Direction dir in Enum.GetValues(typeof(Note.Direction)))
            {
                //var noteList = notes[dir].Where(x => x.start <= sample && x.end >= sample && x.state == Note.NoteState.Idle && !x.gameObject).ToList();
                var noteList = notes[dir].Where(x => x.start <= sample && x.end >= sample && !x.gameObject).ToList();
                foreach (var note in noteList)
                {
                    var obj = Instantiate(exampleNote, canvas.transform);
                    obj.GetComponent<RectTransform>().localPosition = DirectionToVector(dir) * 150;
                    if (note.state != Note.NoteState.Idle)
                        obj.GetComponent<Image>().color = note.state == Note.NoteState.Hit ? Color.green : Color.red;
                    note.gameObject = obj;
                }
            }
        }

        private List<Note> GetNotesAtSample(int sample, Note.Direction direction)
        {
            var list = notes[direction].Where(x => x.start <= sample && x.end >= sample && x.state == Note.NoteState.Idle).ToList();
            if (list.Count > 0)
                return list;
            return null;
        }
        int sampleLeniency = 10;
        private void MarkNotesAtSample(int sample, Note.Direction direction)
        {
            var adjacentNotes = notes[direction].Where(x => x.start - sampleLeniency <= sample && x.end + sampleLeniency >= sample).ToList();
            var hitableNotes = adjacentNotes.Where(x => x.start <= sample && x.end >= sample).ToList();
            foreach (var note in adjacentNotes)
            {
                if (hitableNotes.Contains(note))
                {
                    note.state = Note.NoteState.Hit;
                    if (note.gameObject)
                        note.gameObject.GetComponent<Image>().color = Color.green;
                    Hit();
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
        private void RemoveNotes(int sample)
        {
            foreach (Note.Direction dir in Enum.GetValues(typeof(Note.Direction)))
            {
                //var list = notes[dir].Where(x => x.state != Note.NoteState.Idle || x.end < sample).ToList();
                var list = notes[dir].Where(x => x.end < sample).ToList();
                if (list.Count > 0)
                {
                    foreach (var n in list)
                    {
                        if (n.state == Note.NoteState.Idle)
                            Miss();
                        Destroy(n.gameObject);
                        notes[dir].Remove(n);
                    }
                }
            }
        }

        private void Hit()
        {
            var node = notes[Note.Direction.Up].Where(x => x.end < 0).ToList()[0];
            int middleSegment = node.start + (node.end - node.start) / 2;
            print("hit!");
        }

        private void Miss()
        {
            print("miss!");
        }
    }
}
