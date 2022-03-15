using UnityEditor;
using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MflixDailyRewards))]
public class MflixDailyRewardsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty instanceIdProp = serializedObject.FindProperty("instanceId");
        SerializedProperty useCloudClockProp = serializedObject.FindProperty("useCloudClock");
        SerializedProperty rewardsProp = serializedObject.FindProperty("rewards");
        SerializedProperty rewardsGiftProp = serializedObject.FindProperty("rewardsGift");
        SerializedProperty cloudClockListProp = serializedObject.FindProperty("cloudClockList");
        SerializedProperty isSingletonProp = serializedObject.FindProperty("isSingleton");
        SerializedProperty keepOpenProp = serializedObject.FindProperty("keepOpen");
        SerializedProperty resetPrizeProp = serializedObject.FindProperty("resetPrize");

        EditorGUILayout.PropertyField(instanceIdProp, new GUIContent("Instance ID", "Change this ID number if you are using multiple instances"));
        EditorGUILayout.PropertyField(isSingletonProp, new GUIContent("Is Singleton?", "Is it singleton or the reward is reloaded on each scene?"));
        EditorGUILayout.PropertyField(keepOpenProp, new GUIContent("Keep Open?", "Do the Canvas pops up even when there is no reward available?"));
        EditorGUILayout.PropertyField(resetPrizeProp, new GUIContent("Reset Prize?", "Resets the prize after the second day?"));

        useCloudClockProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Use Cloud Clock?"), useCloudClockProp.boolValue);
        if (useCloudClockProp.boolValue && EditorTools.DrawHeader("Cloud Clock"))
        {
            // Cloud Clock List
            for (int i = 0; i < cloudClockListProp.arraySize; i++)
            {
                var cloudClockProp = cloudClockListProp.GetArrayElementAtIndex(i);
                var cloudClockNameProp = cloudClockProp.FindPropertyRelative("name");

                if (EditorTools.DrawHeader(cloudClockNameProp.stringValue))
                {
                    EditorTools.BeginContents();


                    var cloudClockUrlProp = cloudClockProp.FindPropertyRelative("url");
                    var cloudClockTimeFormatProp = cloudClockProp.FindPropertyRelative("timeFormat");

                    EditorGUILayout.PropertyField(cloudClockNameProp, new GUIContent("Name"));
                    EditorGUILayout.PropertyField(cloudClockUrlProp, new GUIContent("URL"));
                    EditorGUILayout.PropertyField(cloudClockTimeFormatProp, new GUIContent("Time Format"));

                    EditorTools.EndContents();

                    if (GUILayout.Button("Remove Cloud Clock"))
                    {
                        cloudClockListProp.DeleteArrayElementAtIndex(i);
                    }
                }
            }

            if (GUILayout.Button("Add Cloud Clock"))
            {
                cloudClockListProp.InsertArrayElementAtIndex(cloudClockListProp.arraySize);
            }
        }

        EditorGUILayout.Space(15f);

        if (EditorTools.DrawHeader("Rewards"))
        {
            // Rewards
            for (int i = 0; i < rewardsProp.arraySize; i++)
            {
                if (EditorTools.DrawHeader("Day " + (i + 1)))
                {
                    EditorTools.BeginContents();
                    SerializedProperty rewardProp = rewardsProp.GetArrayElementAtIndex(i);

                    /*var unitRewardProp = rewardProp.FindPropertyRelative("unit");
                    var rewardQtProp = rewardProp.FindPropertyRelative("reward");
                    var rewardSpriteProp = rewardProp.FindPropertyRelative("sprite");

                    EditorGUILayout.PropertyField(unitRewardProp, new GUIContent("Unit"));
                    EditorGUILayout.PropertyField(rewardQtProp, new GUIContent("Reward"));
                    rewardSpriteProp.objectReferenceValue = EditorGUILayout.ObjectField("Sprite", rewardSpriteProp.objectReferenceValue, typeof(Sprite), false);*/

                    var isBigRewardProp = rewardProp.FindPropertyRelative("isBigReward");
                    EditorGUILayout.PropertyField(isBigRewardProp, new GUIContent("Is Big Reward"));

                    EditorGUILayout.Space(10f);

                    var hasDiamondProp = rewardProp.FindPropertyRelative("hasDiamondReward");
                    var diamondRewardNameProp = rewardProp.FindPropertyRelative("rewardDiamondName");
                    var diamondRewardAmountProp = rewardProp.FindPropertyRelative("rewardDiamondAmount");
                    var diamondRewardSpriteNormalProp = rewardProp.FindPropertyRelative("rewardDiamondSpriteNormal");
                    var diamondRewardSpriteLockedProp = rewardProp.FindPropertyRelative("rewardDiamondSpriteLocked");

                    EditorGUILayout.PropertyField(hasDiamondProp, new GUIContent("Has Diamond Reward"));
                    EditorGUILayout.PropertyField(diamondRewardNameProp, new GUIContent("Diamond Reward Name"));
                    EditorGUILayout.PropertyField(diamondRewardAmountProp, new GUIContent("Diamond Reward Amount"));
                    diamondRewardSpriteNormalProp.objectReferenceValue = EditorGUILayout.ObjectField("Diamond Sprite Normal", diamondRewardSpriteNormalProp.objectReferenceValue, typeof(Sprite), false);
                    diamondRewardSpriteLockedProp.objectReferenceValue = EditorGUILayout.ObjectField("Diamond Sprite Locked", diamondRewardSpriteLockedProp.objectReferenceValue, typeof(Sprite), false);

                    EditorGUILayout.Space(10f);

                    var hasTicketProp = rewardProp.FindPropertyRelative("hasTicketReward");
                    var ticketRewardNameProp = rewardProp.FindPropertyRelative("rewardTicketName");
                    var ticketRewardAmountProp = rewardProp.FindPropertyRelative("rewardTicketAmount");
                    var ticketRewardSpriteNormalProp = rewardProp.FindPropertyRelative("rewardTicketSpriteNormal");
                    var ticketRewardSpriteLockedProp = rewardProp.FindPropertyRelative("rewardTicketSpriteLocked");

                    EditorGUILayout.PropertyField(hasTicketProp, new GUIContent("Has Ticket Reward"));
                    EditorGUILayout.PropertyField(ticketRewardNameProp, new GUIContent("Ticket Reward Name"));
                    EditorGUILayout.PropertyField(ticketRewardAmountProp, new GUIContent("Ticket Reward Amount"));
                    ticketRewardSpriteNormalProp.objectReferenceValue = EditorGUILayout.ObjectField("Ticket Sprite Normal", ticketRewardSpriteNormalProp.objectReferenceValue, typeof(Sprite), false);
                    ticketRewardSpriteLockedProp.objectReferenceValue = EditorGUILayout.ObjectField("Ticket Sprite Locked", ticketRewardSpriteLockedProp.objectReferenceValue, typeof(Sprite), false);

                    EditorTools.EndContents();

                    if (GUILayout.Button("Remove Reward"))
                    {
                        rewardsProp.DeleteArrayElementAtIndex(i);
                    }
                }
            }

            if (GUILayout.Button("Add Reward"))
            {
                rewardsProp.InsertArrayElementAtIndex(rewardsProp.arraySize);
            }
        }

        EditorGUILayout.Space(15f);

        if (EditorTools.DrawHeader("Rewards Gift"))
        {
            for (int i = 0; i < rewardsGiftProp.arraySize; i++)
            {
                SerializedProperty giftProp = rewardsGiftProp.GetArrayElementAtIndex(i);
                var giftDayToRewardProp = giftProp.FindPropertyRelative("dayToReward");
                
                if (EditorTools.DrawHeader("Day " + giftDayToRewardProp.intValue))
                {
                    EditorTools.BeginContents();
                    
                    var giftDiamondAmountProp = giftProp.FindPropertyRelative("giftDiamondAmount");
                    var gifTicketAmountProp = giftProp.FindPropertyRelative("giftTicketAmount");
                    var giftClosedSpriteProp = giftProp.FindPropertyRelative("giftClosedSprite");
                    var giftOpenedSpriteProp = giftProp.FindPropertyRelative("giftOpenedSprite");

                    EditorGUILayout.PropertyField(giftDayToRewardProp, new GUIContent("Day To Reward"));
                    EditorGUILayout.PropertyField(giftDiamondAmountProp, new GUIContent("Diamond Amount"));
                    EditorGUILayout.PropertyField(gifTicketAmountProp, new GUIContent("Ticket Amount"));
                    giftClosedSpriteProp.objectReferenceValue = EditorGUILayout.ObjectField("Gift Closed Sprite", giftClosedSpriteProp.objectReferenceValue, typeof(Sprite), false);
                    giftOpenedSpriteProp.objectReferenceValue = EditorGUILayout.ObjectField("Gift Opened Sprite", giftOpenedSpriteProp.objectReferenceValue, typeof(Sprite), false);

                    EditorTools.EndContents();

                    if (GUILayout.Button("Remove Reward Gift"))
                    {
                        rewardsGiftProp.DeleteArrayElementAtIndex(i);
                    }
                }
            }

            if (GUILayout.Button("Add Reward Gift"))
            {
                rewardsGiftProp.InsertArrayElementAtIndex(rewardsGiftProp.arraySize);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
