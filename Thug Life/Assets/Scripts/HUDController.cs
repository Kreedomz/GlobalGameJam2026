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
    public TMP_Text maskText;
    public bool maskOn = false;

    [Header("Seen UI")]
    public GameObject seenTextObject; // SeenText 
    public float seenFlashSeconds = 1.0f;

    Coroutine _seenRoutine;

    void Start()
    {
        ApplyAll();
    }

    void Update() //testing
    {
        if (Input.GetKeyDown(KeyCode.M)) SetMask(!maskOn);
        if (Input.GetKeyDown(KeyCode.K)) SetReputation01(rep01 - 0.1f);
        if (Input.GetKeyDown(KeyCode.L)) AddMoney(25);
        if (Input.GetKeyDown(KeyCode.Space)) FlashSeen();
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

    public void SetMask(bool on)
    {
        maskOn = on;
        ApplyMask();
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
        if (repFill != null) repFill.fillAmount = rep01;
        // Change color based on rep
        if (repFill != null)
        {
            if (rep01 > 0.6f) repFill.color = Color.green;
            else if (rep01 > 0.3f) repFill.color = Color.yellow;
            else repFill.color = Color.red;
        }
    }

    void ApplyMoney()
    {
        if (moneyText != null)
            moneyText.text = $"${money} / ${moneyGoal}";
    }

    void ApplyMask()
    {
        if (maskText != null)
            maskText.text = maskOn ? "MASK: ON" : "MASK: OFF";
    }
}
