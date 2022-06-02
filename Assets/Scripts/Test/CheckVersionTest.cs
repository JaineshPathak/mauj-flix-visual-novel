using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HtmlAgilityPack;
using System.Threading.Tasks;

public class CheckVersionTest : MonoBehaviour
{
    private void Start()
    {
        GetCurrentVersionAsync();
    }

    public void GetCurrentVersionAsync()
    {
        //await Task.Delay(1000);

        string currentVersion = "";

        string playStoreUrl = "https://play.google.com/store/apps/details?id=com.culttales.maujflix";

        HtmlWeb playStoreWeb = new HtmlWeb();
        HtmlDocument playStoreDoc = playStoreWeb.Load(playStoreUrl);

        if(playStoreDoc != null)
        {
            var playStoreBody = playStoreDoc.DocumentNode.SelectSingleNode("//body");
            //var playChildNodes = playStoreBody.ChildNodes;

            foreach (var node in playStoreBody.Descendants())
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    //print(node.InnerText);
                    if (node.InnerText.Equals("Version"))
                    {
                        print(node.InnerText);
                        print(node.NextSibling.InnerText);
                    }
                }
            }

            /*var nodes = playStoreBody.Elements("div");
            
            foreach(var node in nodes)
            {
                if(node.NodeType == HtmlNodeType.Element)
                {
                    if(node.InnerText == "Current Version")
                    {
                        print(node.InnerText);
                        print(node.NextSibling.InnerText);
                    }
                }
            }*/
        }        
    }
}
