using System;
using UnityEngine;

namespace Game.Core
{
    public class AbstractManager : MonoBehaviour
    {
        protected bool isReady = false;
        public bool IsReady { get { return isReady; } }

        public virtual void Init()
        {
            isReady = true;
            //Debug.Log("init " + name);
        }
    }
}