using System;
using UnityEngine;

namespace WeavingPuzzle.Events
{
    public static class WeavingEvents
    {
        public static event Action<int, int> OnBlockColored;
        public static event Action OnWeavingStarted;
        public static event Action OnWeavingCompleted;
        public static event Action OnCanvasReset;
        public static event Action<Vector3> OnThreadMoved;

        public static void BlockColored(int x, int y)
        {
            OnBlockColored?.Invoke(x, y);
        }

        public static void WeavingStarted()
        {
            OnWeavingStarted?.Invoke();
        }

        public static void WeavingCompleted()
        {
            OnWeavingCompleted?.Invoke();
        }

        public static void CanvasReset()
        {
            OnCanvasReset?.Invoke();
        }

        public static void ThreadMoved(Vector3 position)
        {
            OnThreadMoved?.Invoke(position);
        }
    }
}
