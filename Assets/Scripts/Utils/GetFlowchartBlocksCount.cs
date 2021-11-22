using UnityEngine;
using Fungus;

public class GetFlowchartBlocksCount : MonoBehaviour
{
    public Flowchart theFlowchart;

    [Space(15)]
    public int totalBlocksCount;

    private void OnValidate()
    {
        if (theFlowchart != null)
            totalBlocksCount = theFlowchart.BlocksCount;
    }
}
