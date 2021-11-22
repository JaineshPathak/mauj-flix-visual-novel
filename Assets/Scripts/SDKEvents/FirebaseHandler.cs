#if ENABLE_SDKS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;

namespace underDOGS.SDKEvents
{
    [CreateAssetMenu(fileName = "FirebaseAnalyticsSDKSettings", menuName = "Settings/SDKs/FirebaseAnalyticsSDKSettings")]
    public class FirebaseHandler : SDKEventHandler
    {
        public override void InitSDK()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(continuationAction: task =>
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                if (FirebaseMessagingHandler.instance != null)
                    FirebaseMessagingHandler.instance.InitFirebaseMessagingHandler();

                if (FirebaseDBHandler.instance != null)
                    FirebaseDBHandler.instance.InitFirebaseDBHandler();
            });
        }

        public override void OnApplicationPausedCallback(bool isPaused)
        { 
        }

        /*public override void SendEvent(string eventName, Dictionary<string, string> data)
        {
#if UNITY_EDITOR
            Debug.Log("FireBase SDK Send Event called");
            Debug.Log("Event Name: " + eventName);
            foreach (KeyValuePair<string, string> author in data)
            {
                Debug.Log(author.Key + " => " + author.Value);
            }
#endif

            //TO DO: Optimise this for future use
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in data)
            {
                dict.Add(pair.Key, pair.Value);
                Parameter[] MyParameters =
                 {
                    new Parameter(pair.Key,pair.Value),
                };
                
                FirebaseAnalytics.LogEvent(eventName, MyParameters);
            }
        }*/

        public override void SendEvent(SDKEventData data)
        {
#if UNITY_EDITOR
            Debug.Log($"Firebase Analytics SDK Send Event called: {data.GetEventDataString()}");
#endif

            /*Parameter[] myParameters = 
            { 
                new Parameter("Level", data.level.ToString())
            };*/

            FirebaseAnalytics.LogEvent($"level_{data.status}", "Level", data.level);
        }

        public override void SendEvent(string eventName, SDKEventStringData data)
        {
            //Forbidden Events
            switch(eventName)
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
            Debug.Log($"Firebase Analytics SDK Send Event called: {eventName} {data.GetEventDataString()}");
#endif
            
            FirebaseAnalytics.LogEvent(eventName, data.eventParameterName, data.eventParameterData);
        }

        public override void SendEvent(SDKEventEpisodeData data)
        {
#if UNITY_EDITOR
            Debug.Log($"Firebase Analytics SDK Send Event called: {SDKEventsNames.episodeCompleteEventName} {data.GetEventDataString()}");
#endif

            Parameter[] myParameters =
            {
                new Parameter(SDKEventsNames.storyParameterName, data.storyTitle),
                new Parameter(SDKEventsNames.episodeParameterName, data.episodeNum)
            };

            FirebaseAnalytics.LogEvent(SDKEventsNames.episodeCompleteEventName, myParameters);
        }
    }
}
#endif