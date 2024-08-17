using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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
    [SerializeField] private RectTransform panelNewScore;
    [SerializeField] private TextMeshProUGUI panelText;
    public TMP_InputField nameInputF;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabBtn;

    [Header("Json stuffs")]
    public TextAsset jsonFile;
    [SerializeField] private Blocks blocks;

    //Internal Variables
    public string customJSON;
    public char constraint;
    public int constraintCount;
    public List<GameObject> prefabList;
    public GameObject[] pairList = new GameObject[2];
    public bool started = false;
    public bool finished = false;
    public bool isCustomJSON = false;
    public int totalPairs = 0;
    public state State;

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
        if (started)
            totalPlayTime += Time.deltaTime;

        timeTMP.text = ((int)totalPlayTime).ToString();

        if (totalPairs == pairs && !finished)
        {
            finished = true;
            started = false;
            SetScore();
            PanelInCustomJSON(true);
            PanelInNewScore();
        }
    }

    #region Init 
    private void Init()
    {
        started = false;
        finished = false;
        if (group.transform.childCount != 0)
        {
            for (int i = 0; i < group.transform.childCount; i++)
            {
                Destroy(group.transform.GetChild(i).gameObject);
            }
        }

        if (!isCustomJSON)
            blocks = JsonUtility.FromJson<Blocks>(jsonFile.text);
        else
        {
            blocks = JsonUtility.FromJson<Blocks>(customJSON);
            if (!CheckViability())
            {
                isCustomJSON = false;
                Init();
                return;
            }
        }

        MaxValue(blocks.blocks);

        if (blocks.blocks.Length > 32)
            group.cellSize = new Vector2(50, 50);
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
            }
            else
            {
                pairList[0].GetComponent<BlockBtn>().Incorrect();
                pairList[1].GetComponent<BlockBtn>().Incorrect();

                pairList[0] = null;
                pairList[1] = null;
            }
        }
    }

    public bool CheckViability()
    {
        Debug.Log(blocks.blocks.Length);
        if (blocks.blocks.Length < 4 || blocks.blocks.Length > 64)
        {
            return false;
        }
        foreach (var item in blocks.blocks)
        {
            if (item.number > 9)
            {
                return false;
            }
        }
        return true;
    }

    public void PanelInCustomJSON(bool isEndGame)
    {
        panelText.text = isEndGame ? "Registrar tu record" : "Ingresar link del JSON";
        State = state.json;
        PanelInNewScore();
    }

    public void PanelInNewScore()
    {
        panelNewScore.transform.localPosition = new Vector3(0f, -791f, 0f);
        panelNewScore.DOAnchorPos(new Vector2(0f, 0f), 1f, false).SetEase(Ease.OutElastic);
    }

    public void PanelOutNewScore()
    {
        panelNewScore.transform.localPosition = new Vector3(0f, 0f, 0f);
        panelNewScore.DOAnchorPos(new Vector2(0f, -791f), 1f, false).SetEase(Ease.OutElastic);
    }

    public void SetScore()
    {
        score = (int)totalPlayTime + totalClicks + pairs;
    }

    public void ResetGame()
    {
        StartCoroutine(ResetGameRoutine());
    }

    public IEnumerator ResetGameRoutine()
    {
        yield return new WaitForSeconds(2f);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void PanelBtnAction()
    {
        if (State == state.register)
        {
            this.GetComponent<DatabaseManager>().RegisterNewScore();
        }
        else
        {
            StartCoroutine(GetTextJSON());
        }
    }

    IEnumerator GetTextJSON()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(nameInputF.text))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                string json = www.downloadHandler.text;
                customJSON = json;
                isCustomJSON = true;
                PanelOutNewScore();
                Init();
                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;
            }
        }
    }
    #endregion

    #region Gets
    public int GetTotalPlayTime()
    {
        return (int)totalPlayTime;
    }

    public int GetTotalClicks()
    {
        return totalClicks;
    }

    public int GetPairs()
    {
        return pairs;
    }

    public int GetScore()
    {
        return score;
    }
    #endregion
}

public enum state { register, json }