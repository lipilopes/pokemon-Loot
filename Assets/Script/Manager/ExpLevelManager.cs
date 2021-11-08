using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpLevelManager : MonoBehaviour
{
    public static ExpLevelManager Instance;

    [SerializeField] int level=1;
    public int Level {get{return level;} }
    [SerializeField] float exp;
    public float Exp {get{return exp;} }
    [SerializeField] float maxExp;
    public float MaxExp {get{return maxExp;} }
    [SerializeField] float expTotal;
    public float ExpTotal {get{return expTotal;} }

    private void Awake() 
    {
         if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);       
    }

    private void Start() 
    {
        Load();
    }

    void Load()
    {
        level       = PlayerPrefs.GetInt("Level",1);
        exp         = PlayerPrefs.GetFloat("Exp", 0);
        expTotal    = PlayerPrefs.GetFloat("TotalExp", 0);
        CalcMaxExp();

        HudManager.Instance.UpdateExpLevel(expTotal > 0,false);
        HudManager.Instance.UpdateExpLevelUP(false);                 
    }

    public int GetRarityExp(DropRarity rarity)
    {
        switch (rarity)
        {
            case DropRarity.Common:     return 5; 
            case DropRarity.Rare:       return 10; 
            case DropRarity.Epic:       return 15; 
            case DropRarity.Legendary:  return 20; 
            default: return 0;
        }  
    }
   
    void CalcMaxExp()
    {
        //Total Exp normal 1695 - shinys 2540 T= 4235
        if(level <= 50)//Only Normals
            maxExp = 33.9f;//1695/50
        else
        if(level == 99)
            maxExp = 49.9f;
        else
            maxExp = 50.8f;//2540/50
    }

    public void SetExp(float dropExp,bool bonus=true)
    {
        float leftoverExp = 0;
        float setExp = dropExp+(bonus ? 5 : 0);
        float exExp = setExp+exp;

         if(exExp > MaxExp)
        {               
            leftoverExp = exExp-MaxExp;  

            setExp -= leftoverExp;              
        }

        exp += setExp;
        
        PlayerPrefs.SetFloat("TotalExp", PlayerPrefs.GetFloat("TotalExp")+setExp);

        expTotal    = PlayerPrefs.GetFloat("TotalExp", 0);

        HudManager.Instance.UpdateExpLevel();

        Debug.Log("SetExp <b>"+setExp+"</b>"+(bonus ? " [<b>+5</b>]" : "")+ " of exp.");

        if(exp >= MaxExp)
        {
            LevelUp(); 

            if(leftoverExp > 0)
            {
                Debug.LogWarning("leftover "+leftoverExp);
                SetExp(leftoverExp,false);
            }
        }

        
        PlayerPrefs.SetFloat("Exp",Exp);
    }

    public void LevelUp()
    {
        level++;

        PlayerPrefs.SetInt("Level",Level);

        //HudManager.Instance.UpdateExpLevelUP();

        exp = 0;

        CalcMaxExp();
    }
}
