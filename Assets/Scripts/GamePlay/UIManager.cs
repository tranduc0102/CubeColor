using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Sington<UIManager>
{ 
   [Header("Element UI")]
   [SerializeField] private Transform breakList;
   [SerializeField] private BreakColor breakColor;
   [SerializeField] private List<BreakColor> currentList = new List<BreakColor>();

   [Header("UI Screen")]

   #region Setting

   private bool checkClick = false;
   [SerializeField]
   private Button btnSetting;
   [SerializeField] 
   private GameObject panelSetting;

   [SerializeField] 
   private Button btnBackLevelSetting;

   [SerializeField] 
   private Button btnReplaySetting;

   [SerializeField] 
   private Button btnHome;

   [SerializeField] 
   private Button btnClose;
   #endregion

   #region PanelWhenFinishLevel
   [SerializeField] 
   private GameObject panelFinishLevel;

   [SerializeField] 
   private Button btnNextLevelPanelFinishLevel;

   [SerializeField] 
   private Button btnReplayPanelFinishLevel;
   #endregion

   #region Panel Win All

   [SerializeField] 
   private GameObject panelWinAll;

   #endregion
   public List<BreakColor> CurrentList
   {
       get { return currentList;}
       set { currentList = value; }
   }
   protected override void LoadComponent()
   {
       base.LoadComponent();
       breakList = transform.GetChild(1).GetChild(0);
       breakColor = Resources.Load<BreakColor>("Prefabs/Effect/BlockColor");
       Setting();
       PanelWhenFinsihLevel();
       PanelWinAll();
   }

   private void Setting()
   {
       btnSetting = transform.GetChild(1).GetChild(1).gameObject.GetComponent<Button>();
       if(panelSetting!=null) return;
       panelSetting = transform.GetChild(0).GetChild(1).gameObject;
       btnReplaySetting = panelSetting.transform.GetChild(0).GetComponent<Button>();
       btnBackLevelSetting = panelSetting.transform.GetChild(1).GetComponent<Button>();
       btnHome = panelSetting.transform.GetChild(2).GetComponent<Button>();
       btnClose = panelSetting.transform.GetChild(3).GetComponent<Button>();
   }

   private void PanelWhenFinsihLevel()
   {
       if(panelFinishLevel!=null)return;
       panelFinishLevel = transform.GetChild(0).GetChild(0).gameObject;
       btnNextLevelPanelFinishLevel = panelFinishLevel.transform.GetChild(0).GetComponent<Button>();
       btnReplayPanelFinishLevel = panelFinishLevel.transform.GetChild(1).GetComponent<Button>();
   }

   private void PanelWinAll()
   {
       if(panelWinAll!=null)return;
       panelWinAll = transform.GetChild(0).GetChild(2).gameObject;
       
   }
   private void OnEnable()
   {
       Observer.Instance.RegisterListener(EventID.CheckCondition,_=>CheckCondition());
       Observer.Instance.RegisterListener(EventID.FinishGame,_=>ClearCurrentList());
       Observer.Instance.RegisterListener(EventID.ActiveUIPanelFinishLevel,_=>{panelFinishLevel.SetActive(true);});
       Observer.Instance.RegisterListener(EventID.FinishAll, _ =>
       {
           panelWinAll.SetActive(true);
       }); //đăng kí sự kiện khi win tất cả 
   }

   protected override void Start()
   {
       base.Start();
       btnSetting.onClick.AddListener(() =>
       {
           AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
           if (!checkClick)
           {
               panelSetting.SetActive(true);
           }

           if (checkClick)
           {
               panelSetting.SetActive(false);
           }

           checkClick = !checkClick;
       });
       btnClose.onClick.AddListener(() => { 
           AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
           panelSetting.SetActive(false);
           checkClick = false;
       });
       btnBackLevelSetting.onClick.AddListener(() =>
       {
           AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
           if (PlayerPrefs.GetInt("SaveByID") > 0)
           {
               PlayerPrefs.SetInt("SaveByID",PlayerPrefs.GetInt("SaveByID") - 1);
               PlayerPrefs.Save();
               SceneManager.LoadSceneAsync("InGame"); // Tải lại cảnh InGame
           }
           panelSetting.SetActive(false);
       });
       btnReplaySetting.onClick.AddListener(() =>
       {
           AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
           SceneManager.LoadSceneAsync("InGame");
       });
       btnHome.onClick.AddListener(() =>
       {
           AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
           SceneManager.LoadSceneAsync("Lobby");
       });
       btnNextLevelPanelFinishLevel.onClick.AddListener(()=>
       {
           AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
           int id = GameManager.Instance.CurrentID + 1;
           GameManager.Instance.CurrentID = id;
           panelFinishLevel.SetActive(false);
       });
       btnReplayPanelFinishLevel.onClick.AddListener(() =>
       {
           AudioManager.Instance.PlaySFX(SoundEffect.ClickButton);
           SceneManager.LoadSceneAsync("InGame");
       });
       
   }

   private void CheckCondition()
   {
       if (GameManager.Instance.CurrentLevel.condition)
       {
           foreach (var obj in GameManager.Instance.CurrentLevel.blockColors)
           {
               StartCoroutine(SpawnBreakColor(obj.color));
           }   
       }
       else
       {
           ClearCurrentList();
       }
   }

   private IEnumerator SpawnBreakColor(ColorType type)
   {
       yield return new WaitForSeconds(0.5f);
       BreakColor colorCondition = Pooling.Spawn(breakColor,breakList.transform.position,quaternion.identity);
       colorCondition.transform.SetParent(breakList, false);
       colorCondition.SetColor = type;
       currentList.Add(colorCondition);
   }

   private void ClearCurrentList()
   {
       if(currentList == null) return;
       foreach (var obj in currentList)
       {
           Pooling.Despawn(obj.gameObject);
       }
       currentList.Clear();
   }
}
