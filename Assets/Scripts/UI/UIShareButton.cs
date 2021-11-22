using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using underDOGS.SDKEvents;

public class UIShareButton : MonoBehaviour
{
    public string storyTitle;
    public string storyTitleEnglish;
    public Sprite storyImageSmall;

    [Space(15f)]

    public string shareTextStart = "खेलो कहानी ";
    public string shareTextEnd = " अभी, Maujflix पर";

    [Space(15f)]

#if UNITY_ANDROID
    public string playStoreLink;
#endif

#if UNITY_IOS
    public string appStoreLink;
#endif

    private string appLink;

    public void Setup(string _storyTitle, string _storyTitleEnglish, Sprite _storyImageSmall)
    {
        storyTitle = _storyTitle;
        storyTitleEnglish = _storyTitleEnglish;
        storyImageSmall = _storyImageSmall;
    }

    //Called from UI OnClick
    public void ShareStory()
    {
        if (storyTitle.Length <= 0)
            return;

        if (storyImageSmall == null)
            return;

        //Send a "[StoryTitle]_sharebtn_clicked" event
        if(SDKManager.instance != null)
        {
            string storyTitleProper = storyTitleEnglish;
            storyTitleProper = storyTitleProper.Replace(" ", "");

            SDKEventStringData shareBtnData;
            shareBtnData.eventParameterName = SDKEventsNames.storyParameterName;
            shareBtnData.eventParameterData = storyTitleProper;

            SDKManager.instance.SendEvent(SDKEventsNames.shareButtonEventName, shareBtnData);
        }

        StartCoroutine(ShareStoryRoutine());
    }

    private IEnumerator ShareStoryRoutine()
    {
        yield return new WaitForEndOfFrame();

        /*Texture2D storySS = new Texture2D((int)storyImageSmall.rect.width, (int)storyImageSmall.rect.height);
        Color[] pixels = storyImageSmall.texture.GetPixels((int)storyImageSmall.textureRect.x, (int)storyImageSmall.textureRect.y, (int)storyImageSmall.textureRect.width, (int)storyImageSmall.textureRect.height);

        storySS.SetPixels(pixels);
        storySS.Apply();*/

        Texture2D storyTexture = storyImageSmall.texture;
#if UNITY_EDITOR
        print("Texture Readable: "+storyImageSmall.texture.isReadable);
#endif

        RenderTexture rt = new RenderTexture(storyTexture.width, storyTexture.height, 0);
        RenderTexture.active = rt;

        Graphics.Blit(storyTexture, rt);

        Texture2D storySS = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, true);
        storySS.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
        storySS.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared_img.png");
        File.WriteAllBytes(filePath, storySS.EncodeToPNG());

        RenderTexture.active = null;

        Destroy(storySS);

        // To avoid memory leaks
        //Destroy(storySS);

#if UNITY_ANDROID
        appLink = playStoreLink;
#elif UNITY_IOS
        appLink = appStoreLink;
#endif

        new NativeShare().AddFile(filePath)
            .SetSubject(storyTitle).SetText(shareTextStart + "'" + storyTitle + "'" + shareTextEnd).SetUrl(appLink)
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();
    }

    /*private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath)
            .SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }*/
}
