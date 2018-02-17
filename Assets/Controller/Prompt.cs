using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;

public class Prompt : MonoBehaviour {
    private GameObject promptObj;
    private Text messageText;
    private Button yesButton;
    private Button noButton;

    public string Message {
        set { this.messageText.text = value; }
    }

    public UnityAction OnYesClick {
        set { this.yesButton.onClick.AddListener(value); }
    }

    public UnityAction OnNoClick {
        set { this.noButton.onClick.AddListener(value); }
    }

    private void Awake() {
        this.promptObj = Instantiate(Resources.Load("Prompt")) as GameObject;

        this.messageText = this.promptObj.GetComponentInChildren<Text>();

        Button[] buttons = this.promptObj.GetComponentsInChildren<Button>();
        this.yesButton = buttons[0];
        this.noButton = buttons[1];

        this.yesButton.onClick.AddListener(closePrompt);
        this.noButton.onClick.AddListener(closePrompt);
    }

    void Start() {
        
    }

    public void SetParent(Transform transform) {
        this.promptObj.transform.SetParent(transform, false);
    }

    private void closePrompt() {
        Debug.Log("Closing prompt...");
        Destroy(this.gameObject);
        Destroy(promptObj);
    }
}