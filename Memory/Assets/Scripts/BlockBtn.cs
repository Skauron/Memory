using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class BlockBtn : MonoBehaviour
{
    [SerializeField] public Block block;
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private Image imageBtn;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<Sprite> Sprites;
    [SerializeField] private List<AudioClip> Clips;
    [SerializeField] private ParticleSystem particles;

    private float fadeTime = 1f;
    private Tween faceTween;

    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<Image>();
        button = this.GetComponent<Button>();
        audioSource = this.GetComponent<AudioSource>();
        imageBtn.sprite = Sprites[block.number];
        imageBtn.GetComponent<CanvasGroup>().alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Init
    public void InitBtn(Block inputBlock)
    {
        block.R = inputBlock.R;
        block.C = inputBlock.C;
        block.number = inputBlock.number;
        this.gameObject.name = block.R.ToString() + block.C.ToString();
    }
    #endregion

    #region Util
    public void Click()
    {
        button.interactable = false;
        GameManager.Instance.CheckPair(this.gameObject);
        StartCoroutine(Animation(1f, () =>
        {
            
        }));
    }

    public void Correct()
    {
        image.color = Color.green;
        button.interactable = false;
        audioSource.clip = Clips[0];
        audioSource.Play();
        particles.Play();
    }

    public void Incorrect()
    {
        audioSource.clip = Clips[1];
        audioSource.Play();
        StartCoroutine(IncorrectClick());
    }

    IEnumerator IncorrectClick()
    {
        yield return new WaitForSeconds(1f);
        image.color = Color.white;
        StartCoroutine(Animation(0f, () =>
        {
            button.interactable = true;
        }));
    }

    IEnumerator Animation(float fadeValue, TweenCallback onEnd)
    {
        imageBtn.transform.localScale = fadeValue == 1f ? Vector3.zero : Vector3.one;
        if (faceTween != null)
        {
            faceTween.Kill(false);
        }
        imageBtn.transform.DOScale(fadeValue, fadeTime).SetEase(Ease.OutBounce);
        faceTween = imageBtn.GetComponent<CanvasGroup>().DOFade(fadeValue, fadeTime);
        faceTween.onComplete += onEnd;
        yield return null;
    }
    #endregion
}
