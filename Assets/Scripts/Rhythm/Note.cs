using UnityEngine;

namespace Rhythm
{
    [System.Serializable]
    public class Note
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
        public float start;
        [HideInInspector] public float end;

        [HideInInspector] public NoteState state;
        [HideInInspector] public GameObject gameObject;

        public float InstantiateNoteAtTime(float timePerSample, float timeToReach)
        {
            return start + (end - start) / 2 - timeToReach;
        }

        public Note(float start, float leniency, Direction direction)
        {
            this.start = start;
            this.end = start + leniency;
            this.state = NoteState.Idle;
            this.direction = direction;
        }
    }
}