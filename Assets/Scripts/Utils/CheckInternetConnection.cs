using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class CheckInternetConnection : MonoBehaviour
{
    public static CheckInternetConnection instance;

    public bool isInternetConnected;

    public static event Action<bool> BroadcastInternetConnectionStatus;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(CheckConnectionPoll());        
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator CheckConnectionPoll()
    {
        while(true)
        {            
            StartCoroutine(CheckInternetAsync((isConnected) =>
            {
                if (isConnected)
                {
                    //Debug.Log("Connected!");
                    isInternetConnected = true;
                    BroadcastInternetConnectionStatus?.Invoke(true);
                }
                else
                {
                    //Debug.Log("Not Connected!");
                    isInternetConnected = false;
                    BroadcastInternetConnectionStatus?.Invoke(false);
                    //return false;
                }
            }));

            yield return new WaitForSecondsRealtime(2f);
        }
    }    

    /*public void CheckConnection()
    {
        StartCoroutine(CheckInternetAsync((isConnected) =>
        {
            if(isConnected)
            {
                Debug.Log("Connected!");
                isInternetConnected = true;
            }
            else
            {
                Debug.Log("Not Connected!");
                isInternetConnected = false;
                //return false;
            }
        }));
    }*/

    IEnumerator CheckInternetAsync(Action<bool> status)
    {
        UnityWebRequest unityWebRequest = new UnityWebRequest("https://www.underdogsthestudio.com");

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            //Debug.Log(unityWebRequest.error);
            status(false);
        }
        else
            status(true);
    }
}
