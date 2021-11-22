#if ENABLE_SDKS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

namespace underDOGS.SDKEvents
{
    [CreateAssetMenu(fileName = "AppsFlyerSDKSettings", menuName = "Settings/SDKs/AppsFlyerSDKSettings")]
    public class AppsFlyerHandler : SDKEventHandler, IAppsFlyerConversionData
    {
        [SerializeField] private string devID;
#if UNITY_IPHONE
        [SerializeField] private string appID;
#endif

        public override void InitSDK()
        {
            if (devID.Length <= 0)
            {
                Debug.LogError("AppsFlyer Dev Key is EMPTY!");
                return;
            }

            AppsFlyer.setIsDebug(true);
            Debug.Log("AppsFlyer UNITY SDK: "+ AppsFlyer.getSdkVersion());

#if UNITY_ANDROID
            AppsFlyer.initSDK(devID, null, SDKManager.instance);
#elif UNITY_IPHONE
            AppsFlyer.initSDK(devID, appID, SDKManager.instance);
#endif
            AppsFlyer.startSDK();
        }

        public override void SendEvent(SDKEventData data)
        {
#if UNITY_EDITOR
            Debug.Log($"AppsFlyer SDK Send Event called: {data.GetEventDataString()}");
#endif

            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("Level", data.level.ToString());            

            AppsFlyer.sendEvent($"level_{data.status}", _params);
        }

        public override void SendEvent(string eventName, SDKEventStringData data)
        {
#if UNITY_EDITOR
            Debug.Log($"AppsFlyer SDK Send Event called: {eventName} {data.GetEventDataString()}");
#endif

            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add(data.eventParameterName, data.eventParameterData);

            AppsFlyer.sendEvent(eventName, _params);
        }

        public override void SendEvent(SDKEventEpisodeData data)
        {
#if UNITY_EDITOR
            Debug.Log($"AppsFlyer SDK Send Event called: {SDKEventsNames.episodeCompleteEventName} {data.GetEventDataString()}");
#endif

            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add(SDKEventsNames.storyParameterName, data.storyTitle);
            _params.Add(SDKEventsNames.episodeParameterName, data.episodeNum.ToString());

            AppsFlyer.sendEvent(SDKEventsNames.episodeCompleteEventName, _params);
        }








        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            // add deferred deeplink logic here
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("onConversionDataFail", error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }
    }
}
#endif