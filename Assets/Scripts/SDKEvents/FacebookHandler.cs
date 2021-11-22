#if ENABLE_SDKS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

namespace underDOGS.SDKEvents
{
    [CreateAssetMenu(fileName = "FacebookSDKSettings", menuName = "Settings/SDKs/FacebookSDKSettings")]
    public class FacebookHandler : SDKEventHandler
    {
        public override void InitSDK()
        {
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
        }

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        public override void OnApplicationPausedCallback(bool isPaused)
        {
            if (!isPaused)
            {
                //app resume
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                }
                else
                {
                    // Initialize the Facebook SDK
                    FB.Init(InitCallback);
                }
            }
        }

        public override void SendEvent(SDKEventData data)
        {
#if UNITY_EDITOR
            Debug.Log($"Facebook SDK Send Event called: {data.GetEventDataString()}");
#endif

            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("Level", data.level);
            FB.LogAppEvent($"level_{data.status}",parameters: _params);
        }

        public override void SendEvent(string eventName, SDKEventStringData data)
        {
            //Forbidden Events
            switch (eventName)
            {
                case SDKEventsNames.topBannerEventNameStart:        //topbanner
                    return;

                case SDKEventsNames.episodePlayBtnEventName:        //playbtn
                    return;

                case SDKEventsNames.episodeCompleteHomeEventName:   //homebtn_clicked
                    return;

                case SDKEventsNames.shareButtonEventName:   //sharebtn_clicked
                    return;

                case SDKEventsNames.branchEndEventName:     //branchending_completed
                    return;

                case SDKEventsNames.branchEndEventNameYes:  //branchending_yesbtn_clicked
                    return;

                case SDKEventsNames.branchEndEventNameNo:   //branchending_nobtn_clicked
                    return;
            }

#if UNITY_EDITOR
            Debug.Log($"Facebook SDK Send Event called: {eventName} {data.GetEventDataString()}");
#endif
            
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add(data.eventParameterName, data.eventParameterData);

            FB.LogAppEvent(eventName,parameters: _params);
            FB.LogAppEvent(eventName + "_" + data.eventParameterName + "_" + data.eventParameterData);            
        }

        public override void SendEvent(SDKEventEpisodeData data)
        {
#if UNITY_EDITOR
            Debug.Log($"Facebook SDK Send Event called: {SDKEventsNames.episodeCompleteEventName} {data.GetEventDataString()}");
#endif

            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add(SDKEventsNames.storyParameterName, data.storyTitle);
            _params.Add(SDKEventsNames.episodeParameterName, data.episodeNum);

            FB.LogAppEvent(SDKEventsNames.episodeCompleteEventName,parameters: _params);
            FB.LogAppEvent(SDKEventsNames.episodeCompleteEventName + "_" + data.storyTitle + "_ep" + data.episodeNum);      //Eg: episode_completed_Pehla_Pyar_ep4
            //FB.LogAppEvent(data.storyTitle + "_ep" + data.episodeNum + "_completed");       //Eg: Padosan_Ep4_Completed
        }

        /*public override void SendEvent(string eventName)
        {
#if UNITY_EDITOR
            Debug.Log($"Facebook SDK: Send EMPTY Event called: " + eventName);
#endif

            FB.LogAppEvent(eventName);
        }

        public override void SendEvent(string eventName, float eventFloatData)
        {
#if UNITY_EDITOR
            Debug.Log($"Facebook SDK: Send FLOAT Event called: " + eventName + ", " + eventFloatData.ToString());
#endif

            FB.LogAppEvent(eventName, eventFloatData);
        }

        public override void SendEvent(string eventName, string eventStringData)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add(eventName, eventStringData);
            
            FB.LogAppEvent(eventName, parameters: _params);
        }*/
    }
}
#endif