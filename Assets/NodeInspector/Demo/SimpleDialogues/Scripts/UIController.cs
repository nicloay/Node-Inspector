using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIController : MonoBehaviour {
    public static UIController Instance;
    public Canvas     UIRoot;
    public Text       ActorName;
    public Text       Text;
    public Button     SingleButton;
    public Button     Button1;
    public Button     Button2;

    public float TimePerCharacter = 0.03f;

    void Start(){
        Instance = this;
    }

    Action onDoneAction;
    public void ShowText(string actorName, string text, Action onDone ){
        UIRoot.enabled = true;
        ActorName.text = actorName;
        onDoneAction = onDone;
        StartCoroutine(PlayText(text));
    }

    bool playTextInProgress;
    bool textShowSuccefull;
    IEnumerator PlayText(string text){
        HideAllButtons();
        UIRoot.enabled = true;
        float time = 0.0f;
        playTextInProgress = true;
        textShowSuccefull = false;
        while(!textShowSuccefull){
            time+=Time.deltaTime;
            int characterNumber = (int)( time/ TimePerCharacter);
            if (characterNumber >= text.Length){
                textShowSuccefull = true;
            } else {
                Text.text = text.Substring(0,characterNumber);
            }
            yield return null;
        }            
        Text.text = text;
        playTextInProgress = false;
    }

    public void Update(){
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)){
            if (playTextInProgress && !textShowSuccefull){
                textShowSuccefull = true;
            } else {
                ClearAndExecuteOnDoneActionIfNotNull();
            }
        }
    }

    void ClearAndExecuteOnDoneActionIfNotNull(){
        if (onDoneAction != null){
            Action tempAction = onDoneAction;
            onDoneAction = null;
            tempAction();
        }
    }

    public void CloseDialogue(){
        UIRoot.enabled = false;
    }

    void HideAllButtons(){
        SingleButton.gameObject.SetActive(false);
        Button1.gameObject.SetActive(false);
        Button2.gameObject.SetActive(false);
    }

    public void ShowSingleButton(string caption, Action onButtonClick){
        ShowButton(SingleButton, caption, onButtonClick);
    }

    public void ShowTwoButtons(string button1Caption, Action onButton1Click, 
        string button2Caption, Action onButton2Click){
        ShowButton(Button1, button1Caption, onButton1Click);
        ShowButton(Button2, button2Caption, onButton2Click);
    }

    void ShowButton(Button button, string caption, Action onButtonClick){
        button.GetComponentInChildren<Text>().text = caption;
        button.gameObject.SetActive(true);
        button.onClick.AddListener(
            new UnityEngine.Events.UnityAction(() => {
                HideAllButtons();
                onButtonClick();  
            }));
    }
}
