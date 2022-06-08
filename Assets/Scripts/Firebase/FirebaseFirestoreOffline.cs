using System.Collections.Generic;
using UnityEngine;
using TMPro;

//This class is used to avoid calling Read/Write operations of Firebase Firestore all the time which can increase the monthly cost
/*How this class works:
 * - When app starts, fetch the current diamonds/tickets amount from FirestoreHandler and copy it in this.diamondsAmount and this.ticketsAmount
 * - When app shutsdown (OnApplicationQuit), push the this.diamondsAmount and this.ticketsAmount to FirestoreHandler and update the Firestore Database
*/
public class FirebaseFirestoreOffline : MonoBehaviourSingletonPersistent<FirebaseFirestoreOffline>
{
    //public static FirebaseFirestoreOffline instance;

    [Header("Diamonds and Tickets")]
    [SerializeField] private float diamondsAmount;
    [SerializeField] private float ticketsAmount;

    public float DiamondsAmount
    {
        get { return diamondsAmount; }
        set 
        { 
            diamondsAmount = value;
            PopulateDiamondsTexts();
        }
    }

    public float TicketsAmount
    {
        get { return ticketsAmount; }
        set
        {
            ticketsAmount = value;
            PopulateTicketsTexts();
        }
    }

    [Header("UI Texts")]
    [SerializeField] private List<TextMeshProUGUI> diamondsAmountTextList = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> ticketsAmountTextList = new List<TextMeshProUGUI>();

    private bool isFirestoreOfflineInitialized;
    private FirebaseFirestoreHandler firestoreHandler;

    public void RegisterDiamondAmountText(TextMeshProUGUI diamondText)
    {
        if (diamondsAmountTextList.Contains(diamondText))
            return;

        diamondsAmountTextList.Add(diamondText);

        PopulateDiamondsTexts();
    }

    private void PopulateDiamondsTexts()
    {
        for (int i = 0; i < diamondsAmountTextList.Count && (diamondsAmountTextList.Count > 0); i++)
        {
            if (diamondsAmountTextList[i] != null)
                diamondsAmountTextList[i].text = GetDiamondsAmountString();
        }
    }

    public void RegisterTicketAmountText(TextMeshProUGUI ticketText)
    {
        if (ticketsAmountTextList.Contains(ticketText))
            return;

        ticketsAmountTextList.Add(ticketText);

        PopulateTicketsTexts();
    }

    private void PopulateTicketsTexts()
    {
        for (int i = 0; i < ticketsAmountTextList.Count && (ticketsAmountTextList.Count > 0); i++)
        {
            if (ticketsAmountTextList[i] != null)
                ticketsAmountTextList[i].text = GetTicketsAmountString();
        }
    }

    /*private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }*/

    private void OnApplicationFocus(bool focus)
    {
        if (firestoreHandler == null && FirebaseFirestoreHandler.instance != null)
            firestoreHandler = FirebaseFirestoreHandler.instance;

        if(firestoreHandler != null)
            firestoreHandler.PushOfflineData(DiamondsAmount, TicketsAmount);
    }

    private void OnApplicationPause(bool pause)
    {
        if (firestoreHandler == null && FirebaseFirestoreHandler.instance != null)
            firestoreHandler = FirebaseFirestoreHandler.instance;

        if (firestoreHandler != null)
            firestoreHandler.PushOfflineData(DiamondsAmount, TicketsAmount);
    }

    private void OnApplicationQuit()
    {
        if (firestoreHandler == null && FirebaseFirestoreHandler.instance != null)
            firestoreHandler = FirebaseFirestoreHandler.instance;

        if (firestoreHandler != null)
            firestoreHandler.PushOfflineData(DiamondsAmount, TicketsAmount);
    }

    //Called from FirestoreHandler, copy the diamonds and tickets fetched from firestore database server
    public void InitFirebaseFirestoreOffline(FirebaseFirestoreHandler _firestoreHandler, float _diamondsAmount, float _ticketsAmount)
    {
        if (isFirestoreOfflineInitialized)
            return;

        isFirestoreOfflineInitialized = true;

        firestoreHandler = _firestoreHandler;
        DiamondsAmount = _diamondsAmount;
        TicketsAmount = _ticketsAmount;
    }

    public void UpdateFirebaseFirestoreData(float _diamondsAmount, float _ticketsAmount)
    {
        DiamondsAmount = _diamondsAmount;
        TicketsAmount = _ticketsAmount;
    }

    public int GetDiamondsAmountInt()
    {
        return Mathf.RoundToInt(DiamondsAmount);
    }

    public string GetDiamondsAmountString()
    {
        //return AbbrevationUtility.AbbreviateNumber(DiamondsAmount);
        return Mathf.RoundToInt(DiamondsAmount).ToString();
    }

    public void DepositDiamondsAmount(float depositAmount)
    {
        DiamondsAmount += depositAmount;
    }

    public void DebitDiamondsAmount(float deductAmount)
    {
        DiamondsAmount -= deductAmount;
        if (DiamondsAmount <= 0)
            DiamondsAmount = 0;
    }

    public int GetTicketsAmountInt()
    {
        return Mathf.RoundToInt(TicketsAmount);
    }

    public string GetTicketsAmountString()
    {
        //return AbbrevationUtility.AbbreviateNumber(TicketsAmount);
        return Mathf.RoundToInt(TicketsAmount).ToString();
    }

    public void DepositTicketsAmount(float depositAmount)
    {
        TicketsAmount += depositAmount;
    }

    public void DebitTicketsAmount(float deductAmount)
    {
        TicketsAmount -= deductAmount;
        if (TicketsAmount <= 0)
            TicketsAmount = 0;
    }
}
