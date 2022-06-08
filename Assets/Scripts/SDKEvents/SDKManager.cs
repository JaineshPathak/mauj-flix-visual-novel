using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace underDOGS.SDKEvents
{
    // Custom Data Strut which will be passed to Handlers.
    public struct SDKEventData
    {
        // Might need a change with different needs
        public float level;
        public string status;

        public string GetEventDataString()
        {
            // Format this however you like it.
            return $"Level {level} {status}";
        }
    }

    public struct SDKEventStringData
    {
        public string eventParameterName;       //Eg: Story, Episode, etc
        public string eventParameterData;       //Eg: StoryTitle, EpisodeNumber

        public string GetEventDataString()
        {
            return eventParameterName + ": " + eventParameterData;
        }
    }

    public struct SDKEventEpisodeData
    {        
        public string storyTitle;           //Story Title
        public int episodeNum;              //Episode Number

        public string GetEventDataString()
        {
            return SDKEventsNames.storyParameterName + ": " + storyTitle + "[" + episodeNum + "]";
        }
    }

    public class SDKManager : MonoBehaviourSingletonPersistent<SDKManager>
    {
        //public static SDKManager instance;

        public SDKEventHandler[] SDK_Events;

        public override void Awake()
        {
            /*if (instance == null)
            {
                instance = this;
                foreach (SDKEventHandler eventHandler in SDK_Events)
                {
                    eventHandler.InitSDK();
                }
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }*/

            base.Awake();

            foreach (SDKEventHandler eventHandler in SDK_Events)
                eventHandler.InitSDK();
        }

        private void OnApplicationQuit()
        {
            string timeText = TimeUtils.GetTimeInFormat(Time.realtimeSinceStartup);

#if UNITY_EDITOR
            print(timeText);
#endif

            if (instance != null)
            {
                SDKEventStringData timeOnAppData;
                timeOnAppData.eventParameterName = SDKEventsNames.timeOnAppParameterName;
                timeOnAppData.eventParameterData = timeText;
                
                instance.SendEvent(SDKEventsNames.timeOnAppEventName, timeOnAppData);
            }
        }


        /// <summary>
        /// This is required for Facebook SDK to reinitialize the SDK if it is not intialized.
        /// </summary>
        /// <param name="isPaused">Whether the application is paused or resumed</param>
        private void OnApplicationPause(bool isPaused)
        {
            foreach (SDKEventHandler eventHandler in SDK_Events)
            {
                eventHandler.OnApplicationPausedCallback(isPaused);
            }
        }

        /// <summary>
        /// Send the data to the respective Event Handlers to send the actual events.
        /// </summary>
        /// <param name="data"></param>
        public void SendEvent(SDKEventData data)
        {
            foreach (SDKEventHandler eventHandler in SDK_Events)
            {
                eventHandler.SendEvent(data);
            }
        }

        /*public void SendEvent(SDKEventStringData data)
        {
            foreach (SDKEventHandler eventHandler in SDK_Events)
            {
                eventHandler.SendEvent(data);
            }
        }*/

        public void SendEvent(string eventName, SDKEventStringData data)
        {
            foreach (SDKEventHandler eventHandler in SDK_Events)
            {
                eventHandler.SendEvent(eventName, data);
            }
        }

        public void SendEvent(SDKEventEpisodeData data)
        {
            foreach (SDKEventHandler eventHandler in SDK_Events)
            {
                eventHandler.SendEvent(data);
            }
        }

        public string GetGender(int index)
        {
            switch (index)
            {
                case 0:
                    return "Male";

                case 1:
                    return "Female";
                
                default:
                    return string.Empty;
            }
        }

        /*public void SendEvent(string eventName)
        {
            foreach (SDKEventHandler eventHandler in SDK_Events)
                eventHandler.SendEvent(eventName);
        }

        public void SendEvent(string eventName, float eventFloatData)
        {
            foreach (SDKEventHandler eventHandler in SDK_Events)
                eventHandler.SendEvent(eventName, eventFloatData);
        }

        public void SendEvent(string eventName, string eventStringData)
        {
            foreach (SDKEventHandler eventHandler in SDK_Events)
                eventHandler.SendEvent(eventName, eventStringData);
        }*/
    }
}