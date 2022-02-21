using UnityEngine;
using TMPro;

public class UITopPanel : MonoBehaviour
{
    public TextMeshProUGUI diamondsText;

    private void OnEnable()
    {
        FirebaseFirestoreHandler.OnFirestoreLoaded += OnFirestoreLoaded;
    }

    private void OnDisable()
    {
        FirebaseFirestoreHandler.OnFirestoreLoaded -= OnFirestoreLoaded;
    }

    private void OnDestroy()
    {
        FirebaseFirestoreHandler.OnFirestoreLoaded -= OnFirestoreLoaded;
    }

    private void OnFirestoreLoaded(FirebaseFirestoreHandler fireStoreHandler)
    {
        if (fireStoreHandler == null)
            return;

        if (fireStoreHandler.mainMenuDiamondsText == null)
            fireStoreHandler.mainMenuDiamondsText = diamondsText;
    }

    public void TryGoogleSignIn()
    {
        if (FirebaseAuthHandler.instance == null)
            return;

        FirebaseAuthHandler.instance.OnGoogleSignIn();
    }

    public void TryGoogleSignOut()
    {
        if (FirebaseAuthHandler.instance == null)
            return;

        FirebaseAuthHandler.instance.OnGoogleSignOut();
    }
}
