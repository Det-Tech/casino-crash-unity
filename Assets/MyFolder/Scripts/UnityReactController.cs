using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnityReactController : MonoBehaviour
{
    public Text txt_Test;
    // Start is called before the first frame update
    void Start()
    {
        txt_Test.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TestCall(int v)
    {
        StartCoroutine(iTestShow(v.ToString(), 2f));
    }

    IEnumerator iTestShow(string v, float t)
    {
        txt_Test.enabled = true;
        txt_Test.text = v.ToString();
        yield return new WaitForSeconds(t);
        txt_Test.enabled = false;        
    }


}
