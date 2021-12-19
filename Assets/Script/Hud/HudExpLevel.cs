using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudExpLevel : MonoBehaviour
{
    public static HudExpLevel Instance;

    [SerializeField] GameObject         tooltipExpLevelGo;
    [SerializeField] Animator           tooltipExpLevelAnim;
    [SerializeField] Image              tooltipExpLevelNewImage;//white
    [SerializeField] Image              tooltipExpLevelTotalImage;//red
    [SerializeField] TextMeshProUGUI    tooltipExpLevelText;

    WaitForSeconds waitExpLevel   = new WaitForSeconds(0.45f+0.05f);//Anim Time + delay
    WaitForSeconds waitExpLevelToClose   = new WaitForSeconds(2f);

    private void Awake() 
    {
         if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);       
    }

    public void UpdateExpLevel(bool anim=true,bool playAudio=true)
    {
        ExpLevelManager expM = ExpLevelManager.Instance;

        if(anim)
        {
            tooltipExpLevelAnim.SetTrigger("Show");
            
            if(tooltipExpLevelText.text != ExpLevelManager.Instance.Level.ToString())
            {
                UpdateExpLevelUP();
                tooltipExpLevelTotalImage.fillAmount = 0;
            }             
        }

        tooltipExpLevelNewImage.fillAmount = expM.Exp / expM.MaxExp;

        StartCoroutine(UpdateExpLevelTotalBar(anim,playAudio));
    }
  
    IEnumerator UpdateExpLevelTotalBar(bool anim=true,bool playAudio=true)
    {
        yield return waitExpLevel;

        if(playAudio)
            tooltipExpLevelGo.GetComponent<PlayAudio>().PlayAudioClip(2);

        while (tooltipExpLevelNewImage.fillAmount != tooltipExpLevelTotalImage.fillAmount)
        {
            if (tooltipExpLevelTotalImage.fillAmount > tooltipExpLevelNewImage.fillAmount)
            {
                tooltipExpLevelTotalImage.fillAmount = tooltipExpLevelNewImage.fillAmount;
                break;
            }
            else
                if (tooltipExpLevelTotalImage.fillAmount < tooltipExpLevelNewImage.fillAmount)
            {
                if ((tooltipExpLevelNewImage.fillAmount - tooltipExpLevelTotalImage.fillAmount) <= 0.19f)//0.77f
                    tooltipExpLevelTotalImage.fillAmount += 0.004f;
                else
                    tooltipExpLevelTotalImage.fillAmount += 0.01f;//0.009f;

                yield return null;
            }
        }

        if(tooltipExpLevelTotalImage.fillAmount == 1)
        {   
            UpdateExpLevelUP();  

            yield return waitExpLevel;  

            tooltipExpLevelTotalImage.fillAmount = 0;
            tooltipExpLevelNewImage.fillAmount   = 0;
        }

        yield return waitExpLevelToClose;

        if(anim && tooltipExpLevelNewImage.fillAmount == tooltipExpLevelTotalImage.fillAmount)
            tooltipExpLevelAnim.SetTrigger("Close");
    }

    public void UpdateExpLevelUP(bool playAudio=true)
    {
        tooltipExpLevelText.text = ""+ExpLevelManager.Instance.Level;

        if(playAudio)
            tooltipExpLevelGo.GetComponent<PlayAudio>().PlayAudioClip(3,false);
    }
}
