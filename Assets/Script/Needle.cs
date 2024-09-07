using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro; 

public class Needle : MonoBehaviour
{
    public Spinner spinner;
    public TextMeshProUGUI scoretext;
    
    void OnTriggerStay2D(Collider2D col)
    {
        if (spinner.isSpinning)
            return;
        scoretext.text = col.gameObject.name;

    }

}
