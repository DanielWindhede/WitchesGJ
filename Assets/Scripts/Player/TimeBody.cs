using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBody
{
    public enum TimeRewindCompletionBehaviour
    {
        DoNothing,
        StopRewind,
    }

    private TimeRewindCompletionBehaviour _currentBehaviour;

    public struct PointInTime
    {
        public Vector3 position;
        public Quaternion rotation;
        public PointInTime(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    [SerializeField] private int _pointDepth = 200;
    private bool _isRewinding;
    List<PointInTime> _pointsInTime;

    public delegate void CompletionBehaviourCallback();
    private CompletionBehaviourCallback _callback;

    public bool IsRewinding
    {
        get { return _isRewinding; }
    }

    public void Initialize(CompletionBehaviourCallback callback = null)
    {
        _isRewinding = false;
        _pointsInTime = new List<PointInTime>();
        _currentBehaviour = TimeRewindCompletionBehaviour.DoNothing;
        this._callback = callback;
    }

    public void DoFixedUpdate(Transform transform)
    {
        if (_isRewinding)
            Rewind(transform);
        else
            Record(transform);
    }

    private void Rewind(Transform transform)
    {
        if (_pointsInTime.Count > 0)
        {
            transform.position = _pointsInTime[0].position;
            transform.rotation = _pointsInTime[0].rotation;
            _pointsInTime.RemoveAt(0);
        }
        else if (_currentBehaviour == TimeRewindCompletionBehaviour.StopRewind)
        {
            StopRewind(_currentBehaviour);
            if (_callback != null)
                _callback();
        }
    }

    private void Record(Transform transform)
    {
        if (_pointsInTime.Count > _pointDepth)
            _pointsInTime.RemoveAt(0);
        _pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
    }

    public void StartRewind(TimeRewindCompletionBehaviour rewindCompletionBehaviour = TimeRewindCompletionBehaviour.DoNothing)
    {
        _isRewinding = true;
        _currentBehaviour = rewindCompletionBehaviour;
    }

    public void StopRewind(TimeRewindCompletionBehaviour rewindCompletionBehaviour = TimeRewindCompletionBehaviour.DoNothing)
    {
        _isRewinding = false;
        _currentBehaviour = rewindCompletionBehaviour;
    }
}
