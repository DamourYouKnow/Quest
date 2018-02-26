using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using Quest.Core;
using Quest.Core.Cards;

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
        this.gameObject.transform.SetParent(GameObject.Find("GameCanvas").transform, false);
        this.promptObj.transform.SetParent(GameObject.Find("GameCanvas").transform, false);
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

public class CardPrompt : MonoBehaviour {
	private GameObject promptObj;
	private Text messageText;
	private GameObject cards;
	private GameObject discards;
	private Button yesButton;
	private CardArea currentCards;

	public CardArea CurrentCards{
		set{this.currentCards = value;}
	}
	public string Message {
		set { this.messageText.text = value; }
	}

	public UnityAction OnYesClick {
		set { this.yesButton.onClick.AddListener(value); }
	}

	private void Awake() {
		this.promptObj = Instantiate(Resources.Load("CardPrompt")) as GameObject;

		this.messageText = this.promptObj.GetComponentInChildren<Text>();

		this.yesButton = this.promptObj.GetComponentInChildren<Button>();

		this.cards = Instantiate (Resources.Load ("CardPanel")) as GameObject;
		this.discards = Instantiate (Resources.Load ("CardPanel")) as GameObject;

		for (int i = 0; i < currentCards.Count; i++) {
			GameObject card = Instantiate (Resources.Load ("DraggableCard", typeof(GameObject))) as GameObject;
			card.transform.SetParent (cards.transform, false);
			card.transform.localScale = new Vector3 (1, 1, 1);
			card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + currentCards.Cards [i].ImageFilename);
		}
		this.yesButton.onClick.AddListener(closePrompt);
	}

	void Start() {
		this.gameObject.transform.SetParent(GameObject.Find("GameCanvas").transform, false);
		this.promptObj.transform.SetParent(GameObject.Find("GameCanvas").transform, false);
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

public class SponsorQuestPrompt : MonoBehaviour {
	private GameObject promptObj;
	private Text messageText;
	private GameObject card;
	private Button yesButton;
	private Button noButton;
	private Card quest;

	public Card Quest{
		set{this.quest = value;}
	}
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
		this.promptObj = Instantiate(Resources.Load("SponsorQuestPrompt")) as GameObject;

		this.messageText = this.promptObj.GetComponentInChildren<Text>();

		Button[] buttons = this.promptObj.GetComponentsInChildren<Button>();
		this.yesButton = buttons[0];
		this.noButton = buttons[1];

		this.yesButton.onClick.AddListener(closePrompt);
		this.noButton.onClick.AddListener(closePrompt);

	}

	void Start() {
		this.gameObject.transform.SetParent(GameObject.Find("GameCanvas").transform, false);
		this.promptObj.transform.SetParent(GameObject.Find("GameCanvas").transform, false);
		GameObject SponsorCard = GameObject.Find ("SponsorQuestCards");
		this.card = Instantiate (Resources.Load ("Card", typeof(GameObject))) as GameObject;
		card.transform.SetParent (SponsorCard.transform, false);
		card.transform.localScale = new Vector3 (1, 1, 1);
		card.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + quest.ImageFilename);
	}

	public void SetParent(Transform transform) {
		this.promptObj.transform.SetParent(transform, false);
	}

	private void closePrompt() {
		Debug.Log("Closing prompt...");
		Destroy(this.gameObject);
		Destroy(promptObj);
		Destroy (this.card);
	}
}