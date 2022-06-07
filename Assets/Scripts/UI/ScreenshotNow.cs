using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class ScreenshotNow : MonoBehaviour
{
	public RectTransform rectT; // Assign the UI element which you wanna capture
	public Image img;
	int width; // width of the object to capture
	int height; // height of the object to capture

	public GameObject[] toDisable;

	// Use this for initialization
	void Start()
	{
		width = System.Convert.ToInt32(rectT.rect.width);
		height = System.Convert.ToInt32(rectT.rect.height);
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.N))
		{
			StartCoroutine(takeScreenShot()); // screenshot of a particular UI Element.
		}		

	}
	public IEnumerator takeScreenShot()
	{
		for (int i = 0; i < toDisable.Length && (toDisable.Length > 0); i++)
		{
			toDisable[i].SetActive(false);
		}

		yield return new WaitForEndOfFrame(); // it must be a coroutine         

		/*Vector3[] corners = new Vector3[4];
		rectT.GetWorldCorners(corners);
		var startX = corners[0].x;
		var startY = corners[0].y;

		int width = (int)corners[3].x - (int)corners[0].x;
		int height = (int)corners[1].y - (int)corners[0].y;*/

		Vector2 temp = rectT.transform.position;
		var startX = temp.x - width / 2;
		var startY = temp.y - height / 2;

		var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
		tex.Apply();

		// Encode texture into PNG
		var bytes = tex.EncodeToPNG();
		Destroy(tex);

		//File.WriteAllBytes(Application.dataPath + "ScreenShot.png", bytes);

		string imgsrc = System.Convert.ToBase64String(bytes);
		Texture2D scrnShot = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		scrnShot.LoadImage(System.Convert.FromBase64String(imgsrc));

		Sprite sprite = Sprite.Create(scrnShot, new Rect(0, 0, scrnShot.width, scrnShot.height), new Vector2(.5f, .5f));
		img.sprite = sprite;

		for (int i = 0; i < toDisable.Length && (toDisable.Length > 0); i++)
		{
			toDisable[i].SetActive(true);
		}

	}

	public void Capture()
	{
		StartCoroutine(takeScreenShot()); // screenshot of a particular UI Element.
	}
}