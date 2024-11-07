using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILobby : BaseComponent
{
    [Header("UI Lobby")] [SerializeField] private Transform srcollButtonStartGame;
    [SerializeField] private Button btnNewGame;
    [SerializeField] private Button btnCountinue;
    [SerializeField] private Button btnRuleGame;
    [SerializeField] private GameObject panelRule;
    [SerializeField] private Button btnClose;
    private bool checkClick = false;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (srcollButtonStartGame == null)
        {
            srcollButtonStartGame = transform.GetChild(2);
            btnNewGame = srcollButtonStartGame.GetChild(1).GetComponent<Button>();
            btnCountinue = srcollButtonStartGame.GetChild(0).GetComponent<Button>();
        }

        if (btnRuleGame == null)
        {
            btnRuleGame = transform.GetChild(3).GetComponent<Button>();
        }

        if (panelRule == null)
        {
               panelRule = transform.GetChild(4).gameObject;
               btnClose = panelRule.transform.GetChild(0).GetComponent<Button>();
        }
    }

    protected override void Start()
    {
        base.Start();
        if (PlayerPrefs.GetInt("SaveByID") <= 0)
        {
            btnCountinue.gameObject.SetActive(false);
        }
        else
        {
            btnCountinue.gameObject.SetActive(true);
        }
        btnNewGame.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
            PlayerPrefs.SetInt("SaveByID",0);
            PlayerPrefs.Save();
            SceneManager.LoadSceneAsync("InGame");
        });
        btnCountinue.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
            SceneManager.LoadSceneAsync("InGame");
        });
        btnRuleGame.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
            if (!checkClick)
            {
                panelRule.SetActive(true);
            }
            else
            {
                panelRule.SetActive(false);
            }
            checkClick = !checkClick;
        });
        btnClose.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
            checkClick = false;
            panelRule.SetActive(false);
        });
    }
}
