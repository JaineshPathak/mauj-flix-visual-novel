using UnityEngine.U2D;

public class AtlasDB : MonoBehaviourSingleton<AtlasDB>
{
    //public static AtlasDB instance;

    public SpriteAtlas charactersAtlas;
    public SpriteAtlas backgroundsAtlas;
    public SpriteAtlas nextEpsAtlas;

    /*private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }*/
}
