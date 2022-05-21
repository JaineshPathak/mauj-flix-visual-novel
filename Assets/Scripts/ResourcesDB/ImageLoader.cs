using System;
using UnityEngine;
using UnityEngine.U2D;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class ImageLoader : MonoBehaviour
{
    [Header("Images DB")]
    //public bool testMode;
    public SpriteAtlas bgSpriteAtlas;    

    [Header("Sprite Renderer")]
    public SpriteRenderer spriteRenderer;

    [Space(15)]

    public int currentTextureIndexToLoad;
    public string currentTextureNameToLoad;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();        
    }    

    public Sprite[] GetSpritesList()
    {
        if (bgSpriteAtlas == null)
            return null;

        Sprite[] spritesList = null;
        
        Array.Resize(ref spritesList, bgSpriteAtlas.spriteCount);
        bgSpriteAtlas.GetSprites(spritesList);

        return spritesList;
    }

    /*private void OnEnable()
    {
        EpisodesSpawner.OnBackgroundAtlasLoaded += OnSpriteAtlasLoaded;
    }

    private void OnDisable()
    {
        EpisodesSpawner.OnBackgroundAtlasLoaded -= OnSpriteAtlasLoaded;
    }

    private void OnDestroy()
    {
        EpisodesSpawner.OnBackgroundAtlasLoaded -= OnSpriteAtlasLoaded;
    }

    private void OnSpriteAtlasLoaded(SpriteAtlas spriteAtlas)
    {
        //if (imagesDB.testMode)
        //return;

        bgSpriteAtlas = spriteAtlas;
    }*/

    private void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if( (spriteRenderer) && spriteRenderer.sprite && string.IsNullOrEmpty(currentTextureNameToLoad))
            currentTextureNameToLoad = spriteRenderer.sprite.texture.name;        

        //if (imagesDB == null)
            //imagesDB = GetComponentInParent<ImagesDB>();
    }

    private void Start()
    {
        bgSpriteAtlas = AtlasDB.instance.backgroundsAtlas;

        if (spriteRenderer && bgSpriteAtlas)
        {
            spriteRenderer.sprite = bgSpriteAtlas.GetSprite(currentTextureNameToLoad);

#if UNITY_EDITOR
            spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
#endif
        }
    }

    /*public void NextImage()
    {
        currentTextureIndexToLoad++;
        currentTextureIndexToLoad = Mathf.Clamp(currentTextureIndexToLoad, 0, spritesList.Length - 1);
    }

    public void PreviousImage()
    {
        currentTextureIndexToLoad--;
        currentTextureIndexToLoad = Mathf.Clamp(currentTextureIndexToLoad, 0, spritesList.Length - 1);
    }*/
}