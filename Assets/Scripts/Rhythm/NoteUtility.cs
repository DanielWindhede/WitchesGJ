
using UnityEngine;

namespace Rhythm
{
    public class NoteUtility
    {
        public static Note.Direction VectorToDirection(Vector2 direction)
        {
            return (Note.Direction)(direction.x + direction.y * 2);
        }
        public static Vector2 DirectionToVector(Note.Direction direction)
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
    }
}