using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Mod.Helpers
{
    public class CoroutineHelper : MonoBehaviour
    {
        private static CoroutineHelper _instance;
        public static CoroutineHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject("CoroutineHelper");
                    _instance = gameObject.AddComponent<CoroutineHelper>();

                    DontDestroyOnLoad(gameObject);
                }
                return _instance;
            }
        }
    }
}
