using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{

    public class OverworldDebug : MonoBehaviour
    {
        [SerializeField] private bool _showCursor;
        public bool ShowCursor
        {
            get { return _showCursor; }
            set
            {
                _showCursor = value;
                Cursor.visible = value;

                if (!value)
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
            }
        }

        private static OverworldDebug _instance = null;
        public static OverworldDebug Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(OverworldDebug)) as OverworldDebug;
                }

                if (_instance == null)
                {
                    var obj = new GameObject("FPSDebug");
                    _instance = obj.AddComponent<OverworldDebug>();
                }

                return _instance;
            }
        }

        private void Start()
        {
            ShowCursor = _showCursor;
        }

        private void OnApplicationQuit()
        {
            _instance = null;
        }
    }
}