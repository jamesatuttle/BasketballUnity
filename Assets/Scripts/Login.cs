using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.SqliteClient;

public class Login : MonoBehaviour {

	public static Login instance; //create an instance of the class, so that other classes can access instances of its methods

	//stores the game object
	public Text LoginText;
	private Text LoginHelp;

	private InputField UserName;

	Button NextButton;
	Button BackButton;
	Button YesButton;
	Button NoButton;

	StartScreen startScreen; //an instance of the start screen

	private string UsernameString; //the username of the user
	public static int LeaderboardID; //the leaderboard id, to update the score
	public static int UserID; //the leaderboard id, to be used to pull the data of the user

	//variables to connect to the database
	private static string construct; //the location of the database file
	private static IDbConnection dbConnection; //the connection to the database
	private static IDbCommand dbCommand; //used to perform SQL statements and commands
	private static IDataReader dataReader; //used to read information from the database

	bool usernameAdded;

	//Awake is called at the start of the game, used to initialise variables
	void Awake() {
		construct = "URI=file:" + Application.dataPath + "\\TrainingData.db";
		instance = this;

		LoginText = GameObject.Find ("LoginText").GetComponent<Text> ();
		LoginHelp = GameObject.Find ("LoginHelp").GetComponent<Text> ();
		YesButton = GameObject.Find ("YesButton").GetComponent<Button> ();
		NoButton = GameObject.Find ("NoButton").GetComponent<Button> ();
		UserName = GameObject.Find ("UsernameInput").GetComponent<InputField> ();
		NextButton = GameObject.Find ("NextButton").GetComponent<Button> ();
		BackButton = GameObject.Find ("BackButton").GetComponent<Button> ();
	}

	//called after Awake, at the start of the game
	void Start () {
		usernameAdded = false;
		HideText (LoginText);
		UsernameString = "";
	}

	/*
	* Update is called once per frame
	* Controls the login functionality
	* Allows the appropriate page to be loaded
	*/
	void Update () {

		switch (GamePlay.ActiveScreenValue) {
		case (int)GamePlay.ActiveScreen.playedBeforeQuestion:
			SetupPlayedBeforeQuestion ();
			break;
		case (int)GamePlay.ActiveScreen.preGame:
			SetUpPlayingGame ();
			break;
		case (int)GamePlay.ActiveScreen.mainGame:
			SetUpPlayingGame ();
			break;
		case (int)GamePlay.ActiveScreen.welcome:
			Welcome ();
			break;
		case (int)GamePlay.ActiveScreen.registerName:
			SetupRegister ();
			break;
		}
	}

	/*
	* Hides the buttons on the display
	* Hides the login text and the input field
	*/
	void SetUpPlayingGame() {
		HideButton (NextButton);
		HideButton (BackButton);
		HideButton (YesButton);
		HideButton (NoButton);

		HideText(LoginText);
		HideText(LoginHelp);

		HideInputField(UserName);
	}

	/*
	* Generic method to hide the button which is passed through
	*/
	void HideButton(Button button) {
		button.image.enabled = false;
		button.GetComponentInChildren<Text> ().text = "";
	}

	/*
	* A method to show the passed in button
	* The button text is set depending on which button has been passed in
	*/
	void ShowButton(Button button) {
		button.image.enabled = true;

		switch (button.name) {
		case "NextButton":
			button.GetComponentInChildren<Text> ().text = "NEXT";
			break;
		case "BackButton":
			button.GetComponentInChildren<Text> ().text = "BACK";
			break;
		case "YesButton":
			button.GetComponentInChildren<Text> ().text = "YES";
			break;
		case "NoButton":
			button.GetComponentInChildren<Text> ().text = "NO";
			break;
		}
	}

	/*
	* Hides the inputField by disabling the image and placeholder text
	* Clears the inputted text
	* Prevnts users from interacting with the input field
	*/
	void HideInputField(InputField inputField) {
		inputField.image.enabled = false;
		inputField.placeholder.enabled = false;
		inputField.text = "";
		inputField.interactable = false;
		inputField.placeholder.GetComponentInChildren<Text> ().text = "";
	}

	/*
	* Hides the ask name screen
	* Hides the buttons, input field and text
	*/
	public void HideAskNameScreen() {
		HideButton (NextButton);
		HideButton (BackButton);
		HideInputField (UserName);
		HideText (LoginText);
		HideText (LoginHelp);
	}

	/*
	* Shows the input field by enabling the input image and placeholder text
	* Sets the input field to interactable
	*/
	void ShowInputField(InputField inputField) {
		inputField.image.enabled = true;
		inputField.placeholder.enabled = true;
		inputField.enabled = true;
		inputField.interactable = true;
		inputField.placeholder.GetComponentInChildren<Text> ().text = "Username...";
	}

	/*
	* A generic method to hide whichever text is passed through
	*/
	void HideText(Text text) {
		text.text = "";
	}

	/*
	* A function which is called when the user clicks a button to go back to the start screen
	* Sets the active screen to the start screen
	*/
	public void BackButton_Pressed_BackToStart() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen;
	}

	/*
	* A function which takes the user to the initial login question
	*/
	public void BackButton_Pressed_BackToPlayedBeforeQuestion() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.playedBeforeQuestion;
	}

	/*
	* A method to set up the initial quesiton
	* Hides the previous login input field and text
	* Displays the YES, NO and BACK buttons
	* Activates these buttons to allow them to be used
	*/
	public void SetupPlayedBeforeQuestion() {
		LoginText.text = "HAVE YOU PLAYED BEFORE?";
		HideInputField (UserName);
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

	/*
	* Sets up the login page
	* Sets the active screen to the enterName screen
	* Displays the appropriate text and buttons and actives these with listeners
	*/
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

	/*
	* Reads the username which the user inputs
	* if the username exists then the WelcomeBack page is displayed
	* if the username does not exist then it displays the appropriate error message
	*/
	public void ReadUsernameInput() {
		if (UserName.text != "") {
			UsernameString = UserName.text;
			if (DoesUsernameExist ()) {
				if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.enterName) {
					WelcomeBack ();
				}
			}
			else
				LoginHelp.text = "Username not found";
		} else
			LoginHelp.text = "Enter a Username";
	}

	/*
	* Reads the username for the register
	* If the username exists then an error is displayed
	* if the username does not exist then the username is added to the database
	*/
	public void ReadUsernameInput_Register() {
		if (UserName.text != "") {
			UsernameString = UserName.text;
			if (DoesUsernameExist () && !usernameAdded)
				LoginHelp.text = "Username already exists";
			else if (usernameAdded) {
				LoginHelp.text = "Username added";
				NextButton.onClick.AddListener (Welcome);
			} else {
				AddNewUsername ();
			}
		} else
			LoginHelp.text = "Enter a Username";
	}

	/*
	* Sets up the register page by displaying the text, input field and buttons
	*/
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

	/*
	* Displays the welcome back screen
	* Shows the next and back buttons and uses the inputted username to display this on the page
	*/
	public void WelcomeBack() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.welcomeBack;

		LoginText.text = "WELCOME BACK " + UsernameString.ToUpper ();
		HighlightText (LoginText);
		HideInputField (UserName);

		ShowButton (NextButton);
		ShowButton (BackButton);

		NextButton.GetComponentInChildren<Text> ().text = "Play";

		HideButton (YesButton);
		HideButton (NoButton);

		BackButton.onClick.AddListener (ReadUsernameInput);
		NextButton.onClick.AddListener (GamePlay.SetUpPregame);
		HideText (LoginHelp);
		LoginHelp.enabled = false;
	}

	/*
	* Displays the welcome screen
	* Shows the next and back buttons and uses the inputted username to display this on the page
	*/
	public void Welcome() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.welcome;

		LoginText.text = "WELCOME " + UsernameString.ToUpper ();
		HighlightText (LoginText);
		HideText (LoginHelp);
		HideInputField (UserName);

		ShowButton (NextButton);
		ShowButton (BackButton);

		NextButton.GetComponentInChildren<Text> ().text = "Play";

		HideButton (YesButton);
		HideButton (NoButton);

		BackButton.onClick.AddListener (ReadUsernameInput_Register);
		NextButton.onClick.AddListener (GamePlay.SetUpPregame);
	}

	/*
	* Display the passed in text in yellow to highlight it
	*/
	public void HighlightText(Text text) {
		text.color = Color.yellow;
	}

	/*
	* Returns true if text is highlighted in yellow, otherwise false
	*/
	public bool IsHighlighted(Text text) {
		if (text.color == Color.yellow)
			return true;
		else
			return false;
	}

	/*
	* Checks against the users table to see if the username exists
	* if it does then returns true, else returns false
	*/
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

	/*
	* Adds the new username to the users table in the database
	* also adds a new row into the leaderboard with the new user's userID
	*/
	private void AddNewUsername() {

		int userId = 0;
		int score = 0;

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "INSERT INTO Users(Username) values ('" + UsernameString + "')";
		dbCommand.ExecuteNonQuery ();

		dbCommand.CommandText = "SELECT ID FROM Users WHERE Username = '" + UsernameString + "';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			userId = (int)(dataReader ["ID"]);
		}

		UserID = userId;

		dbCommand.CommandText = "INSERT INTO Leaderboard(UserID, Score) values ('" + userId + "', '" + score + "')";
		dbCommand.ExecuteNonQuery ();

		dbCommand.CommandText = "SELECT ID FROM Leaderboard WHERE UserId = '" + userId + "' AND Score = '0';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			LeaderboardID = (int)(dataReader ["ID"]);
		}

		dbConnection.Close ();

		usernameAdded = true;
	}

	/*
	* Adds a new row into the leaderboard with the new userId
	*/
	public void AddNewLeaderboardRow() {

		int userId = 0;
		int score = 0;

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "SELECT ID FROM Users WHERE Username = '" + UsernameString + "';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			userId = (int)(dataReader ["ID"]);
		}

		UserID = userId;

		dbCommand.CommandText = "INSERT INTO Leaderboard(UserID, Score) values ('" + userId + "', '" + score + "')";
		dbCommand.ExecuteNonQuery ();

		dbCommand.CommandText = "SELECT ID FROM Leaderboard WHERE UserId = '" + userId + "' AND Score = '0';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			LeaderboardID = (int)(dataReader ["ID"]);
		}

		dbConnection.Close ();
	}


}
