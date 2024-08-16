using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Gameplay")]
    [SerializeField] private float totalPlayTime = 0f;
    [SerializeField] private int totalClicks = 0;
    [SerializeField] private int pairs = 0;
    [SerializeField] private int score = 0;

    [Header("UI")]
    [SerializeField] private GridLayoutGroup group;
    [SerializeField] private TextMeshProUGUI timeTMP;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabBtn;

    [Header("Json stuffs")]
    public TextAsset jsonFile;
    [SerializeField] private Blocks blocks;

    //Internal Variables
    public char constraint;
    public int constraintCount;
    public List<GameObject> prefabList;
    public List<GameObject> checkPairsList;
    public GameObject[] pairList = new GameObject[2];
    public bool started = false;
    public int totalPairs = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(started)
            totalPlayTime += Time.deltaTime;

        timeTMP.text = ((int) totalPlayTime).ToString();

        //Win Checker
        if(totalPairs == pairs){
            Debug.Log("Trigger Win");
        }
    }

    #region Init 
    private void Init()
    {
        blocks = JsonUtility.FromJson<Blocks>(jsonFile.text);

        MaxValue(blocks.blocks);

        group.constraint = constraint == 'R' ? GridLayoutGroup.Constraint.FixedRowCount : GridLayoutGroup.Constraint.FixedColumnCount;
        group.constraintCount = constraintCount;

        foreach (var block in blocks.blocks)
        {
            GameObject btn = Instantiate(prefabBtn, new Vector3(0, 0, 0), Quaternion.identity, group.gameObject.transform);
            btn.GetComponent<BlockBtn>().InitBtn(block);
            prefabList.Add(btn);
        }
    }
    #endregion

    #region Util
    private void MaxValue(Block[] Arr)
    {
        int maxR = 0;
        int maxC = 0;
        maxR = Arr[0].R;
        maxC = Arr[0].C;
        foreach (var block in Arr)
        {
            if (block.R > maxR)
                maxR = block.R;
            if (block.C > maxC)
                maxC = block.C;
        }
        //Debug.Log("C: " + maxC + " R:" + maxR);
        constraint = maxR > maxC ? 'R' : 'C';
        constraintCount = maxR > maxC ? maxR : maxC;
        totalPairs = (maxR * maxC) / 2;
    }

    public void CheckPair(GameObject block)
    {
        int index = 0;
        totalClicks++;
        started = true;
        index = pairList[0] != null ? 1 : 0;
        pairList[index] = block;
        if (pairList[0] != null && pairList[1] != null)
        {
            int block1 = pairList[0].GetComponent<BlockBtn>().block.number;
            int block2 = pairList[1].GetComponent<BlockBtn>().block.number;
            if (block1 == block2)
            {
                pairs++;

                pairList[0].GetComponent<BlockBtn>().Correct();
                pairList[1].GetComponent<BlockBtn>().Correct();

                pairList[0] = null;
                pairList[1] = null;
            }else{
                pairList[0].GetComponent<BlockBtn>().Incorrect();
                pairList[1].GetComponent<BlockBtn>().Incorrect();

                pairList[0] = null;
                pairList[1] = null;
            }
        }
    }
    #endregion
}
