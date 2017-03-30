using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

	public static Login instance;

	public Text LoginText;
	private Text Yes;
	private Text No;
	private Text _firstNameLabel;
	private Text _lastNameLabel;
	private bool _registering;

	private InputField FirstName;
	private InputField LastName;

	Button NextButton;
	Button BackButton;

	StartScreen startScreen;

	private string _firstName;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		LoginText = GameObject.Find ("LoginText").GetComponent<Text> ();
		Yes = GameObject.Find ("Yes").GetComponent<Text> ();
		No = GameObject.Find ("No").GetComponent<Text> ();

		FirstName = GameObject.Find ("FirstNameInput").GetComponent<InputField> ();
		LastName = GameObject.Find ("LastNameInput").GetComponent<InputField> ();

		NextButton = GameObject.Find ("NextButton").GetComponent<Button> ();
		BackButton = GameObject.Find ("BackButton").GetComponent<Button> ();

		HideText (LoginText);
		HideText (Yes);
		HideText (No);

		_registering = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.enterName) {
			SetupLogin ();
		}
	}

	void HideButton(Button button) {
		button.image.enabled = false;
		button.GetComponentInChildren<Text> ().text = "";
	}

	void ShowButton(Button button) {
		button.image.enabled = true;

		if (button.name == "NextButton")
			button.GetComponentInChildren<Text> ().text = "NEXT";

		if (button.name == "BackButton")
			button.GetComponentInChildren<Text> ().text = "BACK";
	}

	void HideInputField(InputField inputField) {
		inputField.image.enabled = false;
		inputField.placeholder.enabled = false;
	}

	public void HideAskNameScreen() {
		HideButton (NextButton);
		HideButton (BackButton);
		HideInputField (FirstName);
		HideInputField (LastName);
		HideText (LoginText);
	}

	void ShowInputField(InputField inputField) {
		inputField.image.enabled = true;
		inputField.ActivateInputField ();
	}

	void HideText(Text text) {
		text.text = "";
	}

	public void BackButton_Pressed() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen;
		StartScreen.instance.SetUpStartScreen ();
	}

	public void SetupLogin() {
		LoginText.text = "PLEASE ENTER YOUR NAME";
		HighlightText (LoginText);

		ShowInputField (FirstName);
		ShowInputField (LastName);

		ShowButton (NextButton);
		ShowButton (BackButton);

		BackButton.onClick.AddListener (BackButton_Pressed);
	}

	public void FirstNameTextChanged(string firstName) {
		_firstName = firstName;
		print (_firstName);
	}

	public  void HighlightText(Text text) {
		text.color = Color.yellow;

		if (text == Yes)
			No.color = Color.white;
		if (text == No)
			Yes.color = Color.white;
	}

	public  bool IsHighlighted(Text text) {
		if (text.color == Color.yellow)
			return true;
		else
			return false;
	}
}
