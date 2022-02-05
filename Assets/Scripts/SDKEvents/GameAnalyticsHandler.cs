#if ENABLE_SDKS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

namespace underDOGS.SDKEvents
{    

    [CreateAssetMenu(fileName = "GameAnalyticsSettings", menuName = "Settings/SDKs/GameAnalyticsSettings")]
    public class GameAnalyticsHandler : SDKEventHandler
    {
        private Player_Data playerData;

        public override void InitSDK()
        {
            GameAnalytics.Initialize();            
        }

        public override void SendEvent(SDKEventData data)
        {
#if UNITY_EDITOR
            Debug.Log($"Game Analytics Send Event called: [{data.status},{data.level.ToString("000")}]");
#endif
            GameAnalytics.NewProgressionEvent(GetStatus(data.status), $"level{data.level.ToString("000")}");
        }

        public override void SendEvent(string eventName, SDKEventStringData data)
        {
#if UNITY_EDITOR
            Debug.Log($"Game Analytics SDK Send Event called: {eventName} {data.GetEventDataString()}");
#endif

            if (playerData == null && EpisodesSpawner.instance != null)
                playerData = EpisodesSpawner.instance.playerData;

            //GameAnalytics.NewDesignEvent(eventName + ":" + data.eventParameterName + ":" + data.eventParameterData);

            switch(eventName)
            {
                case SDKEventsNames.thumbnailsClickedEventName:         //thumbnails_clicked

                    if (playerData != null)
                    {
                        string playerGender = SDKManager.instance.GetGender(playerData.genderType);       //Male Or Female

                        //Eg: "Male_thumbnails_clicked_Udaan" OR "Female_thumbnails_clicked_Udaan"
                        GameAnalytics.NewDesignEvent(playerGender + "_" + SDKEventsNames.thumbnailsClickedEventName + "_" + data.eventParameterData);
                    }
                    else
                        GameAnalytics.NewDesignEvent(eventName + "_" + data.eventParameterData);
                    break;

                case SDKEventsNames.topBannerEventNameStart:        //topbanner

                    //Eg: topbanner_Udaan_playbtn_clicked
                    GameAnalytics.NewDesignEvent(SDKEventsNames.topBannerEventNameStart + "_" + data.eventParameterData + "_" + SDKEventsNames.topBannerEventNameEnd);
                    break;

                case SDKEventsNames.episodePlayBtnEventName:        //playbtn

                    //Eg: Udaan_episode1_playbtn
                    GameAnalytics.NewDesignEvent(data.eventParameterName + "_" + data.eventParameterData + "_" + SDKEventsNames.episodePlayBtnEventName);
                    break;

                case SDKEventsNames.episodeCompleteHomeEventName:   //homebtn_clicked

                    //Eg: Udaan_episode1_completed_homebtn_clicked
                    GameAnalytics.NewDesignEvent(data.eventParameterName + "_" + data.eventParameterData + "_completed_" + SDKEventsNames.episodeCompleteHomeEventName);
                    break;

                case SDKEventsNames.shareButtonEventName:       //sharebtn_clicked

                    //Eg: Udaan_sharebtn_clicked
                    GameAnalytics.NewDesignEvent(data.eventParameterData + "_" + SDKEventsNames.shareButtonEventName);
                    break;

                case SDKEventsNames.branchEndEventName:         //branchending_completed

                    //Eg: Udaan_episode1_branchending_completed
                    GameAnalytics.NewDesignEvent(data.eventParameterName + "_" + data.eventParameterData + "_" + SDKEventsNames.branchEndEventName);
                    break;

                case SDKEventsNames.branchEndEventNameYes:      //branchending_yesbtn_clicked

                    //Eg: Udaan_episode1_branchending_yesbtn_clicked
                    GameAnalytics.NewDesignEvent(data.eventParameterName + "_" + data.eventParameterData + "_" + SDKEventsNames.branchEndEventNameYes);
                    break;

                case SDKEventsNames.branchEndEventNameNo:      //branchending_nobtn_clicked

                    //Eg: Udaan_episode1_branchending_nobtn_clicked
                    GameAnalytics.NewDesignEvent(data.eventParameterName + "_" + data.eventParameterData + "_" + SDKEventsNames.branchEndEventNameNo);
                    break;
            }
        }

        public override void SendEvent(SDKEventEpisodeData data)
        {
            if (playerData == null && EpisodesSpawner.instance != null)
                playerData = EpisodesSpawner.instance.playerData;

            //Eg: Udaan_episode1_completed
            GameAnalytics.NewDesignEvent(data.storyTitle + "_episode" + data.episodeNum + "_completed");
            //GameAnalytics.NewDesignEvent(SDKEventsNames.episodeCompleteEventName + ":" + data.storyTitle + ":" + data.episodeNum);
        }

        private GAProgressionStatus GetStatus(string status)
        {
            switch (status)
            {
                case "start":
                    return GAProgressionStatus.Start;
                case "restart":
                    return GAProgressionStatus.Undefined;
                case "fail":
                    return GAProgressionStatus.Fail;
                case "complete":
                    return GAProgressionStatus.Complete;                
                default:
                    return GAProgressionStatus.Undefined;
            }
        }
    }
}
#endif