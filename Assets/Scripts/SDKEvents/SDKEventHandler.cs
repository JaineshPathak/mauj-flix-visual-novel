using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace underDOGS.SDKEvents
{
    public abstract class SDKEventHandler : ScriptableObject
    {
        public abstract void InitSDK();
        public virtual void OnApplicationPausedCallback(bool isPaused) { }
        public abstract void SendEvent(SDKEventData data);
        public abstract void SendEvent(string eventName, SDKEventStringData data);
        public abstract void SendEvent(SDKEventEpisodeData data);

        //public abstract void SendEvent(SDKEventStringData data);
        /*public abstract void SendEvent(string eventName);
        public abstract void SendEvent(string eventName, float eventFloatData);
        public abstract void SendEvent(string eventName, string eventStringData);*/
    }
}