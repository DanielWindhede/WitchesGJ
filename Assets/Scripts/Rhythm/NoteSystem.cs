using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rhythm
{
    public class NoteSystem : MonoBehaviour
    {
        [SerializeField] private AudioSource _audio;
        [SerializeField] private GameObject item;
        [SerializeField] private int startSegment = 50;
        [SerializeField] private int endSegment = 1000;
        [SerializeField] private Note.Direction direction;
        class Note
        {
            public enum Direction
            {
                Up = 2,
                Right = 1,
                Down = -2,
                Left = -1,
            }

            public bool hasInteracted;
            public int start;
            public int end;
            public Direction direction;

            public Note(int start, int end, Direction direction)
            {
                this.start = start;
                this.end = end;
                this.hasInteracted = false;
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

        void Start()
        {
            AddNote(new Note(startSegment, endSegment, direction));
            //AddNote(new Note(startSegment, _audio.clip.samples / 1024));
            item.SetActive(false);
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
                var list = GetNotesAtSample(_audio.timeSamples / 1024, direction);
                if (list != null)
                {
                    item.SetActive(true);
                }

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
                    MarkNotesAtSample(_audio.timeSamples / 1024, VectorToDirection(controlDirection));

                RemoveNotes(_audio.timeSamples / 1024);
            }
        }

        private Note.Direction VectorToDirection(Vector2 direction)
        {
            return (Note.Direction)(direction.x + direction.y * 2);
        }

        private List<Note> GetNotesAtSample(int sample, Note.Direction direction)
        {
            var list = notes[direction].Where(x => x.start <= sample && x.end >= sample && !x.hasInteracted).ToList();
            if (list.Count > 0)
                return list;
            return null;
        }
        private void MarkNotesAtSample(int sample, Note.Direction direction)
        {
            foreach (var note in notes[direction].Where(x => x.start <= sample && x.end >= sample).ToList())
            {
                note.hasInteracted = true;
            }
        }
        private void RemoveNotes(int sample)
        {
            foreach (Note.Direction dir in Enum.GetValues(typeof(Note.Direction)))
            {
                var list = notes[dir].Where(x => x.hasInteracted || x.end < sample).ToList();
                if (list.Count > 0)
                {
                    notes[dir].RemoveWhere(x => x.hasInteracted || x.end < sample);
                    item.SetActive(false);
                }
            }
        }
    }
}
