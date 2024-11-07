using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Sington<GameManager>
{
   [Header("Data Game")]
   [SerializeField] private DataGame data;
   [Header("Player")]
   [SerializeField] private Cube player;
   [Header("Effect")]
   [SerializeField] private Transform prefabLightActive;
   [SerializeField] private Transform prefabLightUnActive;

   [Header("Animation LoadNewGame")] 
   [SerializeField] private Transform prefabLoadGame;
   [SerializeField] private Animator animLoadGame;
   
   public int idLevel;
   private DataLevel currentLevel;
   private int countBlock = 1;

   private List<Vector2> listCanNotGo = new List<Vector2>();
   public DataGame Data
   {
      get { return data; }
   }

   public Cube Player
   {
      get { return player; }
   }

   public Transform PrefabLight
   {
      get
      {
         return prefabLightActive;
      }
   }
   public Transform PrefabUnLight
   {
      get
      {
         return prefabLightUnActive;
      }
   }
   public DataLevel CurrentLevel
   {
      get { return currentLevel; }
   }
   public int CountBlock
   {
      get { return countBlock; }
      set { countBlock = value; }
   }

   public List<Vector2> ListCanNotGo
   {
      set
      {
         listCanNotGo = value;
      }
      get { return listCanNotGo; }
   }

   public int CurrentID
   {
      get
      {
         return idLevel;
      }
      set
      {
         if(value < data.listLevels.Count && value > idLevel)
         {
            idLevel += 1; // Tăng lên 1
            PlayerPrefs.SetInt("SaveByID", idLevel);
            PlayerPrefs.Save();
            Observer.Instance.PostEvent(EventID.NextLevel);
         }
      }
   }

   private void OnEnable()
   {
      Observer.Instance.RegisterListener(EventID.NextLevel,_=>NextLevel());
      Observer.Instance.RegisterListener(EventID.FinishLevel,_=>FinishLevel());
   }
   protected override void Start()
   {
      base.Start();
      idLevel = PlayerPrefs.GetInt("SaveByID");
      currentLevel = data.listLevels[idLevel];
      countBlock = currentLevel.blockColors.Count;
      listCanNotGo.AddRange(currentLevel.canNotGoes);
   }

   protected override void LoadComponent()
   {
      base.LoadComponent();
      data = Resources.Load<DataGame>("Data/DataGame");
      player = Resources.Load<Cube>("Prefabs/Cube");
      prefabLightActive = Resources.Load<Transform>("Prefabs/Effect/LightActive");
      prefabLightUnActive = Resources.Load<Transform>("Prefabs/Effect/LightUnActive");
      prefabLoadGame = GameObject.Find("Main Camera").transform.GetChild(0);
      animLoadGame = prefabLoadGame.gameObject.GetComponent<Animator>();
   }

   protected void NextLevel()
   {
      idLevel = PlayerPrefs.GetInt("SaveByID");
      if (idLevel < data.listLevels.Count)
      {
         listCanNotGo.Clear();
         animLoadGame.Play("LoadNewGame");
         currentLevel = data.listLevels[idLevel];
         countBlock = currentLevel.blockColors.Count;
         listCanNotGo.AddRange(currentLevel.canNotGoes);
      }
   }

   protected void FinishLevel()
   {
     animLoadGame.Play("LoadGame");
     StartCoroutine(CheckCurrentClipName());
   }

   private IEnumerator CheckCurrentClipName()
   {
      yield return new WaitForSeconds(1.5f);

      while (GetCurrentClipName() != "LoadNewGame") 
      {
         yield return null;
      }
      // xử lí event sau khi sang Anim LoadNewGame
      Observer.Instance.PostEvent(EventID.ActiveUIPanelFinishLevel);
   }

   private string GetCurrentClipName(){
      int layerIndex = 0;
      return animLoadGame.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name;
   }
}
