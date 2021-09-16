using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    public class NoteRecorder : MonoBehaviour
    {
        public struct NoteRecorderData
        {
            public float time;
            public Note.Direction direction;
        }

        bool record;

        private Dictionary<Note.Direction, HashSet<Note>> notes = new Dictionary<Note.Direction, HashSet<Note>>()
        {
            { Note.Direction.Up,    new HashSet<Note>() },
            { Note.Direction.Right, new HashSet<Note>() },
            { Note.Direction.Down,  new HashSet<Note>() },
            { Note.Direction.Left,  new HashSet<Note>() },
        };
        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
            {
                record = true;
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

            notes[NoteUtility.VectorToDirection(controlDirection)].Add()
        }
    }

}