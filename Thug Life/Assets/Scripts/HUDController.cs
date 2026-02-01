using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Reputation UI")]
    public Image repFill; // RepBar_Fill
    [Range(0f, 1f)] public float rep01 = 1f;

    [Header("Money UI")]
    public TMP_Text moneyText;
    public int money = 0;
    public int moneyGoal = 300;

    [Header("Mask UI")]
    [SerializeField] Image maskImage;
    [SerializeField] Sprite maskOnSprite;
    [SerializeField] Sprite maskOffSprite;

    [Header("RobUI")]
    [SerializeField] TMP_Text robText;

    [Header("Seen UI")]
    public GameObject seenTextObject; // SeenText 
    public float seenFlashSeconds = 1.0f;

    Coroutine _seenRoutine = null;

    void Start()
    {
        
    }

    void Update() //testing
    {
        //if (Input.GetKeyDown(KeyCode.M)) SetMask(!maskOn);
        //if (Input.GetKeyDown(KeyCode.K)) SetReputation01(rep01 - 0.1f);
        //if (Input.GetKeyDown(KeyCode.L)) AddMoney(25);
        //if (Input.GetKeyDown(KeyCode.Space)) FlashSeen();
        ApplyAll();
    }

    public void SetReputation01(float value01)
    {
        rep01 = Mathf.Clamp01(value01);
        ApplyRep();
    }

    public void AddMoney(int amount)
    {
        money = Mathf.Max(0, money + amount);
        ApplyMoney();
    }

    public void FlashSeen()
    {
        if (_seenRoutine != null) StopCoroutine(_seenRoutine);
        _seenRoutine = StartCoroutine(SeenRoutine());
    }

    IEnumerator SeenRoutine()
    {
        if (seenTextObject != null) seenTextObject.SetActive(true);
        yield return new WaitForSeconds(seenFlashSeconds);
        if (seenTextObject != null) seenTextObject.SetActive(false);
        _seenRoutine = null;
    }

    void ApplyAll()
    {
        ApplyRep();
        ApplyMoney();
        ApplyMask();
        if (seenTextObject != null) seenTextObject.SetActive(false);
    }

    void ApplyRep()
    {
        Player player = FindFirstObjectByType<Player>();
        if (repFill != null && player != null)
        {
            float repFillAmount = (float)player.GetPlayerReputation() / (float)player.GetPlayerMaxReputation();
            repFill.fillAmount = repFillAmount;
        }

    }

    void ApplyMoney()
    {
        Player player = FindAnyObjectByType<Player>();
        if (moneyText != null && player != null)
            moneyText.text = $"${player.playerMoney}";
    }

    void ApplyMask()
    {
        Player player = FindAnyObjectByType<Player>();
        if (player.maskState == Player.MaskState.On)
        {
            maskImage.sprite = maskOnSprite;
        }
        else if (player.maskState == Player.MaskState.Off)
        {
            maskImage.sprite = maskOffSprite;
        }
    }

    public void ToggleRobUI(bool toggle)
    {
        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            if (toggle)
            {
                robText.gameObject.SetActive(true);

            }
            else
            {
                //robText.gameObject.SetActive(false);
            }
        }
    }
}
