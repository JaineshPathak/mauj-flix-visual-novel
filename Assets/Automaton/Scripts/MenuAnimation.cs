using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuAnimation : MonoBehaviour
{
    public Vector3 starPos = new Vector3(540, 530, 0);
    public Vector3 endPos = new Vector3(540, -1050, 0);
    public Transform ButtonGroup;
    public float speed = 1000f;
    public bool isMoving;
    public float time;
    // Start is called before the first frame update
    private void Start()
    {
        float distance = Vector3.Distance(ButtonGroup.position, starPos); // ButtonGroup
        time = distance / speed;
    }
    private void OnEnable()
    {
        Debug.Log("Access");
        isMoving = true;
        exitPos();
    }
    private void Update()
    {
        if (isMoving == true)
        {
            StartCoroutine(UIcoroutine());
        }
    }
    IEnumerator UIcoroutine()
    {
        ButtonGroup.position = Vector3.MoveTowards(ButtonGroup.position, starPos, Time.deltaTime * speed); // ButtonGroup
        yield return new WaitForSeconds(time);
        isMoving = false;

    }
    public void exitPos()
    {
        ButtonGroup.position = endPos; // ButtonGroup
    }
} 