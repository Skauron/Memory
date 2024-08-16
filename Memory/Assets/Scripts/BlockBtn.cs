using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BlockBtn : MonoBehaviour
{
    [SerializeField] public Block block;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    // Start is called before the first frame update
    void Start()
    {
        text.gameObject.SetActive(false);
        image = this.GetComponent<Image>();
        button = this.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitBtn(Block inputBlock)
    {
        block.R = inputBlock.R;
        block.C = inputBlock.C;
        block.number = inputBlock.number;
        text.text = block.number.ToString();
        this.gameObject.name = block.R.ToString() + block.C.ToString();
    }

    public void Click()
    {
        text.gameObject.SetActive(true);
        button.interactable = false;
        GameManager.Instance.CheckPair(this.gameObject);
    }

    public void Correct(){
        image.color = Color.green;
        button.interactable = false;
    }

    public void Incorrect(){
        text.gameObject.SetActive(false);
        button.interactable = true;
    }
}
