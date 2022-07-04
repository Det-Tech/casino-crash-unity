using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoHandler : MonoBehaviour
{
    public Text txt_username;
    public Text txt_multipier;
    public Text txt_amount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValues(string username, string multipier, string amount, string betted, string cashed)
    {
        
        bool b_betted = bool.Parse(betted);
        bool b_cashed = bool.Parse(cashed);
        txt_username.text = username;
        if (!b_betted && !b_cashed)
        {
            txt_multipier.text = float.Parse(multipier).ToString("0.00") + "x";
            txt_multipier.color = Color.gray;
            txt_amount.text = amount;
            txt_amount.color = Color.gray;
            Debug.Log("called AAA");
        }

        if (b_betted && !b_cashed)
        {
            txt_multipier.text = float.Parse(multipier).ToString("0.00") + "x";
            txt_multipier.color = Color.gray;
            txt_amount.text = amount;
            txt_amount.color = Color.green;
            Debug.Log("called BBB");
        }


        if (b_betted && b_cashed)
        {
            txt_multipier.text = float.Parse(multipier).ToString("0.00") + "x";
            txt_multipier.color = Color.green;
            txt_amount.text = amount;
            txt_amount.color = Color.green;
            Debug.Log("called CCC");
        }

    }

}
