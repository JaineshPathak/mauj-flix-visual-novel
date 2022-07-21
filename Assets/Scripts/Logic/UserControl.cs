﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControl : MonoBehaviourSingletonPersistent<UserControl>
{
    private bool adminMode = false;
    public bool AdminMode
    {
        get { return adminMode; }
    }

    private IEnumerator Start()
    {
        //Let Ads Manager Initialize first, so wait for 2 seconds
        yield return new WaitForSeconds(2f);

        if(FirebaseRemoteConfigHandler.instance != null && IronSource.Agent != null)
        {
            string adID = IronSource.Agent.getAdvertiserId();
            string[] adminDevicesList = FirebaseRemoteConfigHandler.instance.GetAdminDevicesList();

            for (int i = 0; i < adminDevicesList.Length && (adminDevicesList.Length > 0); i++)
            {
                if (adminDevicesList[i].Equals(adID))
                {
                    adminMode = true;                    
                    break;
                }
            }

            if(adminMode)
                Debug.Log($"User Control: Ad ID {adID} matches, Admin mode is active!");
            else
                Debug.Log($"User Control: Ad ID {adID} mismatches, Admin mode is inactive!");

        }
    }
}
