using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.SqliteClient;

public class Login : MonoBehaviour {

	public static Login instance;

	public Text LoginText;
	private Text LoginHelp;

	private InputField UserName;

	Button NextButton;
	Button BackButton;
	Button YesButton;
	Button NoButton;

	StartScreen startScreen;

	private string UsernameString;

	private static string construct;
	private static IDbConnection dbConnection;
	private static IDbCommand dbCommand;
	private static IDataReader dataReader;

	bool usernameAdded;

	void Awake() {
		construct = "URI=file:" + Application.dataPath + "\\TrainingData.db";
		instance = this;
	}

	// Use this for initialization
	void Start () {
		LoginText = GameObject.Find ("LoginText").GetComponent<Text> ();
		LoginHelp = GameObject.Find ("LoginHelp").GetComponent<Text> ();

		YesButton = GameObject.Find ("YesButton").GetComponent<Button> ();
		NoButton = GameObject.Find ("NoButton").GetComponent<Button> ();

		UserName = GameObject.Find ("UsernameInput").GetComponent<InputField> ();

		NextButton = GameObject.Find ("NextButton").GetComponent<Button> ();
		BackButton = GameObject.Find ("BackButton").GetComponent<Button> ();

		usernameAdded = false;

		HideText (LoginText);
	}
	
	// Update is called once per frame
	void Update () {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.playedBeforeQuestion) {
			SetupPlayedBeforeQuestion ();
		}

		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.preGame || GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame) {
			HideButton (NextButton);
			HideButton (BackButton);
			HideButton (YesButton);
			HideButton (NoButton);

			HideText(LoginText);
			HideText(LoginHelp);

			HideInputField(UserName);
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
		inputField.text = "";
		inputField.placeholder.enabled = false;
	}

	public void HideAskNameScreen() {
		HideButton (NextButton);
		HideButton (BackButton);
		HideInputField (UserName);
		HideText (LoginText);
		HideText (LoginHelp);
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
		HideText (LoginHelp);
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

		HideText (LoginHelp);

		BackButton.onClick.AddListener (BackButton_Pressed_BackToPlayedBeforeQuestion);
		NextButton.onClick.AddListener (ReadUsernameInput);
	}

	public void ReadUsernameInput() {
		if (UserName.text != "") {
			UsernameString = UserName.text;
			if (DoesUsernameExist ()) {
				//LoginHelp.text = "Username found";
				if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.enterName)
					WelcomeBack ();
			}
			else
				LoginHelp.text = "Username not found";
		}
	}

	public void ReadUsernameInput_Register() {
		if (UserName.text != "") {
			UsernameString = UserName.text;
			if (DoesUsernameExist () && !usernameAdded)
				LoginHelp.text = "Username already exists";
			else if (usernameAdded) {
				LoginHelp.text = "Username added";
				//usernameAdded = false;
			} else {
				AddNewUsername ();
			}
		}
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
		NextButton.onClick.AddListener (ReadUsernameInput_Register);
	}

	public void WelcomeBack() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.welcome;

		LoginText.text = "WELCOME BACK" + UsernameString.ToUpper ();
		HighlightText (LoginText);
		HideText (LoginHelp);
		HideInputField (UserName);

		ShowButton (NextButton);
		ShowButton (BackButton);

		NextButton.GetComponentInChildren<Text> ().text = "Play";

		HideButton (YesButton);
		HideButton (NoButton);

		BackButton.onClick.AddListener (ReadUsernameInput);
		NextButton.onClick.AddListener (GamePlay.SetUpPregame);
	}

	public void HighlightText(Text text) {
		text.color = Color.yellow;
	}

	public bool IsHighlighted(Text text) {
		if (text.color == Color.yellow)
			return true;
		else
			return false;
	}

	private bool DoesUsernameExist() {
		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = "SELECT * FROM Users WHERE Username = '" + UsernameString + "';";
		dataReader = dbCommand.ExecuteReader ();

		bool usernameFound = false;

		while (dataReader.Read ()) {
			if (dataReader ["Username"].ToString () != "") {
				usernameFound = true;
			}
		}

		dbConnection.Close ();

		return usernameFound;
	}

	private void AddNewUsername() {
		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "INSERT INTO Users(Username) values ('" + UsernameString + "')";

		dbCommand.ExecuteNonQuery ();

		dbConnection.Close ();

		usernameAdded = true;
	}
}
