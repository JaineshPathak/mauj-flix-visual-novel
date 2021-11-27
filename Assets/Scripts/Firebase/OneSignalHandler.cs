/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneSignalHandler : MonoBehaviour
{
    public static OneSignalHandler instance;

    public string oneSignalAppId = "ab4419b6-248e-41bd-9e27-2e7e7e7c7e10";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Uncomment this method to enable OneSignal Debugging log output 
        OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.VERBOSE, OneSignal.LOG_LEVEL.ERROR);

        // Replace 'YOUR_ONESIGNAL_APP_ID' with your OneSignal App ID.
        OneSignal.StartInit(oneSignalAppId)
          .HandleNotificationOpened(OneSignalHandleNotificationOpened)
          .Settings(new Dictionary<string, bool>() {
            { OneSignal.kOSSettingsAutoPrompt, false },
            { OneSignal.kOSSettingsInAppLaunchURL, false } })
            .EndInit();

        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;

        // iOS - Shows the iOS native notification permission prompt.
        //   - Instead we recomemnd using an In-App Message to prompt for notification 
        //     permission to explain how notifications are helpful to your users.
        OneSignal.PromptForPushNotificationsWithUserResponse(OneSignalPromptForPushNotificationsReponse);
    }

    // Gets called when the player opens a OneSignal notification.
    private static void OneSignalHandleNotificationOpened(OSNotificationOpenedResult result)
    {
        // Place your app specific notification opened logic here.
    }

    // iOS - Fires when the user anwser the notification permission prompt.
    private void OneSignalPromptForPushNotificationsReponse(bool accepted)
    {
        // Optional callback if you need to know when the user accepts or declines notification permissions.
    }
}*/