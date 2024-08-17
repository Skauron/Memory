using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewScore : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Result result)
    {
        nameText.text = result.name;
        scoreText.text = result.score.ToString();
    }
}
