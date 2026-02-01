using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Goals")]
    public int moneyGoal = 300;
    public float repMax = 100f;

    [Header("Current")]
    public int money = 0;
    public float rep = 100f;

    [Header("UI")]
    public HUDController hud;
    public EndScreenUI endUI;

    bool _ended = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        ApplyUI();
        LockCursor();
    }

    public void AddMoney(int amount)
    {
        if (_ended) return;

        money += Mathf.Max(0, amount);
        ApplyUI();

        if (money >= moneyGoal)
        {
            _ended = true;
            if (endUI != null)
                endUI.ShowWin(money, moneyGoal);
        }
    }

    public void LoseReputation(float amount)
    {
        if (_ended) return;

        rep -= Mathf.Abs(amount);
        rep = Mathf.Clamp(rep, 0f, repMax);

        ApplyUI();

        if (rep <= 0f)
        {
            _ended = true;
            if (endUI != null)
                endUI.ShowLose(money, moneyGoal);
        }
    }

    void ApplyUI()
    {
        if (hud != null)
        {
            hud.moneyGoal = moneyGoal;
            hud.money = money;
            hud.SetReputation01(repMax <= 0f ? 0f : rep / repMax);
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

