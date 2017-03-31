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

	private InputField UserName;

	Button NextButton;
	Button BackButton;
	Button YesButton;
	Button NoButton;

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

		YesButton = GameObject.Find ("YesButton").GetComponent<Button> ();
		NoButton = GameObject.Find ("NoButton").GetComponent<Button> ();

		UserName = GameObject.Find ("UsernameInput").GetComponent<InputField> ();

		NextButton = GameObject.Find ("NextButton").GetComponent<Button> ();
		BackButton = GameObject.Find ("BackButton").GetComponent<Button> ();

		HideText (LoginText);
		HideText (Yes);
		HideText (No);

		_registering = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.playedBeforeQuestion) {
			SetupPlayedBeforeQuestion ();
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

		if (button.name == "YesButton")
			button.GetComponentInChildren<Text> ().text = "YES";

		if (button.name == "NoButton")
			button.GetComponentInChildren<Text> ().text = "NO";
	}

	void HideInputField(InputField inputField) {
		inputField.image.enabled = false;
		inputField.placeholder.enabled = false;
	}

	public void HideAskNameScreen() {
		HideButton (NextButton);
		HideButton (BackButton);
		HideInputField (UserName);
		HideText (LoginText);
	}

	void ShowInputField(InputField inputField) {
		inputField.image.enabled = true;
		inputField.placeholder.enabled = true;
	}

	void HideText(Text text) {
		text.text = "";
	}

	public void BackButton_Pressed_BackToStart() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen;
		StartScreen.instance.SetUpStartScreen ();
	}

	public void BackButton_Pressed_BackToPlayedBeforeQuestion() {
		SetupPlayedBeforeQuestion ();
	}

	public void SetupPlayedBeforeQuestion() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.playedBeforeQuestion;
		LoginText.text = "HAVE YOU PLAYED BEFORE?";
		HighlightText (LoginText);

		ShowButton (YesButton);
		ShowButton (NoButton);
		ShowButton (BackButton);
		HideButton (NextButton);
		BackButton.onClick.AddListener (BackButton_Pressed_BackToStart);
		YesButton.onClick.AddListener (SetupLogin);
		NoButton.onClick.AddListener (SetupRegister);
	}

	public void SetupLogin() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.enterName;

		LoginText.text = "PLEASE ENTER YOUR USERNAME";
		HighlightText (LoginText);

		ShowInputField (UserName);

		ShowButton (NextButton);
		ShowButton (BackButton);

		HideButton (YesButton);
		HideButton (NoButton);

		BackButton.onClick.AddListener (BackButton_Pressed_BackToPlayedBeforeQuestion);
	}

	public void SetupRegister() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.registerName;

		LoginText.text = "PLEASE ENTER A USERNAME";
		HighlightText (LoginText);

		ShowInputField (UserName);

		ShowButton (NextButton);
		ShowButton (BackButton);

		HideButton (YesButton);
		HideButton (NoButton);

		BackButton.onClick.AddListener (BackButton_Pressed_BackToPlayedBeforeQuestion);
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
