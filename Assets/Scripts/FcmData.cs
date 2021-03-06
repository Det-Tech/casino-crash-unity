using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;

#pragma warning disable 0618
public class UserData {
    #region ***** DATA *****

    //   public  string device_token;
    public string username;
    public string email_address;
    public string name;
    public string password;
    public string verification_code;
    public string balance;
    public string level;

    #endregion
}
//public class UserLogin  // Api = user_login.php
//{;
//    public string device_token;
//    public string username;-
//    public string password;
//} 
public class FcmData : MonoBehaviour {
    #region Data Fields
    UserData userData = new UserData();


    public static FcmData _instance;
    public static FcmData Instance {
        get {
            return _instance;
        }
    }
    //  UserLogin loginData;

    // private string url = "http://4zeetraders.com/roobet_backend/";

    //  private string myToken = null;

    //  private string verificationCode = null;

    [Header("Login")]
    public TMP_InputField login_Email_usernameText;
    public TMP_InputField login_passwordText;

    [Header("Signup")]
    public TMP_InputField signup_emailText;
    public TMP_InputField signup_nameText;
    public TMP_InputField signup_passwordText;
    public TMP_InputField signup_confirmPasswordText;
    public TMP_InputField change_passwordText;
    public TMP_InputField change_confirmPasswordText;

    [Header("Forget password")]
    public TMP_InputField forgetPass_usernameText;
    public GameObject forgetPass_panel;

    [Header("Verify Code")]
    public TMP_InputField verifyCode_Text;
    public GameObject verifyCode_panel;

    [Header("Panel")]
    public GameObject registerGO;
    public GameObject welcomePanel;
    public GameObject changePassPanel;
    public SwitchPanel sp_Script;


    [Header("Texts")]
    //public TMP_Text errorText;
    public TMP_Text info_errorText;
    public TMP_Text welcomeText;
    public TMP_Text tot_BidsText;
    public TMP_Text tot_UsersText;
    public UserinfoText userText;
    public TMP_Text fcmText;

    public TMP_Text payoutText;
    public TMP_Text roundOverText;

    [Header("GameObjects")]
    //GameObjects
    public GameControl gameControl;

    public GameObject mover;
    public GameObject video;

    public VideoPlayer videoPlayer;

    public RectTransform contentPanel;

    [Header("Input Field bet and cash")]
    //Input Field bet and cash
    public TMP_InputField betText;
    public TMP_InputField cashOutText;

    //Data Arrays

    //Users Bids
    // [SerializeField]
    // string[] online_Users;
    string[] users_Bids_Username;
    string[] users_Bids_Amount;


    //Bool 
    //private bool signedIn = false;
    private bool roomJoined = false;
    #endregion

    private void Start() {
	//	PlayVideo();
		registerGO.SetActive(true);
        verifyCode_panel.SetActive(false);
        forgetPass_panel.SetActive(false);
        welcomePanel.SetActive(false);

        _instance = this;


  
    //    GetOnlineUsersBid();

    }

    #region ****** EXTRAS *******
    Coroutine vidCor;
    public void PlayVideo() {
        video.SetActive(true);
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "scene rocket.mp4");

        //  videoPlayer.url = "https://drive.google.com/file/d/1irYq96Fe7Azj-GEgO0C6UqmnUKNVoP3B/view?usp=sharing";
        IEnumerator _cor() {
            yield return new WaitForEndOfFrame();
            videoPlayer.Play();
        }
        vidCor = StartCoroutine(_cor());
    }

    public void StopVideo() {
        if (vidCor != null) StopCoroutine(vidCor);
        videoPlayer.Stop();
        video.SetActive(false);
    }

    void ChangeText(string msg, bool isRed) {
        info_errorText.gameObject.SetActive(true);
        if (!isRed) {
            info_errorText.color = Color.white;
            info_errorText.text = msg;
        } else {
            info_errorText.color = Color.red;
            info_errorText.text = msg;
        }
    }
    #endregion


    #region*********** Sign UP ***********
    public void SignUp() {
        //if (signup_usernameText.text != "" && signup_emailText.text != "" && signup_nameText.text != "" && signup_passwordText.text != "" && signup_confirmPasswordText.text != "" && signup_passwordText.text == signup_confirmPasswordText.text) {
        //    userData = new UserData();

        //    userData.username = signup_usernameText.text;
        //    userData.email_address = signup_emailText.text;
        //    userData.name = signup_nameText.text;
        //    userData.password = signup_passwordText.text;
        //    Application.ExternalCall("api_signup", userData.username, userData.email_address, userData.password, userData.name);
        //} else {
        //    info_errorText.color = Color.red;
        //    info_errorText.text = "Enter correct data";
        //}

        info_errorText.text = "";
		if (signup_passwordText.text != signup_confirmPasswordText.text) {
            info_errorText.text = "Passwords do not match";
        } else if (signup_nameText.text != "" && signup_emailText.text != "" && signup_passwordText.text != "" && signup_confirmPasswordText.text != "") {
            info_errorText.text = "";
			userData = new UserData();
            userData.username = signup_nameText.text;
            userData.email_address = signup_emailText.text;
            userData.password = signup_passwordText.text;

            void onGetResp(WWW req) {
                if (!req.isDone) {
					info_errorText.text = $"Error While Sending: {req.error}";
                } else if (req.isDone) {
                    var str = req.text;
                    //Debug.Log(str);
                    
                    JSONNode info = null;
                    try {
                        info = JSON.Parse(str);
                    } catch (Exception) {
                        Debug.LogWarning(str);
                        //Debug.LogWarning(e);
                    }

                    if (info == null) {
						info_errorText.text = "Server error, try again later";
                        return;
                    }

                    var code = -1;
                    if (!string.IsNullOrEmpty(info["code"]))
                        code = int.Parse(info["code"].Value);
                    var msg = info["message"];
					//var verify = int.Parse(userInfo["user_info"]["is_verified"].Value);
					//var userName = userInfo["user_info"]["username"].Value;

					info_errorText.text = msg;

                    if (code == 200) {
                        login_Email_usernameText.text = userData.email_address;
                        login_passwordText.text = userData.password;
                        Login();
                    }
                }
                ApiAccess.onResponseGetWWW -= onGetResp;
            }
            ApiAccess.onResponseGetWWW += onGetResp;

            if (signup_nameText.text != "" && signup_emailText.text != "" && signup_passwordText.text != "" && signup_passwordText.text == signup_confirmPasswordText.text) {

                StartCoroutine(ApiAccess.Register(signup_nameText.text, signup_emailText.text, signup_passwordText.text));
            }
        }
    }

    public void api_signup_res(string ResponseData) { // unused
        JSONNode userInfo = JSON.Parse((ResponseData).ToString());
        Debug.Log(userInfo.Value);
        var code = Int32.Parse(userInfo["code"].Value);
        var msg = userInfo["msg"].Value;

        if (code == 1) {
            ChangeText(msg, false);
            sp_Script.SwitchLogin();
            login_Email_usernameText.text = userData.username;
            login_passwordText.text = userData.password;
            //  verifyCode_panel.SetActive(true);
        } else {
            ChangeText(msg, true);
        }
    }
    #endregion


    #region*********** Log IN ***********
    public void user_logged_in(string Response) {
        JSONNode userInfo = JSON.Parse(Response);
        var verify = Int32.Parse(userInfo["user_info"]["is_verified"].Value);
        userData.username = userInfo["user_info"]["username"].Value;
        userData.balance = userInfo["user_info"]["balance"].Value;
        userData.level = userInfo["user_info"]["level"].Value;

        if (verify == 1) {
            registerGO.SetActive(false);
            welcomePanel.SetActive(true);
            welcomeText.text = "Welcome \"" + userData.username + "\" \nBalance: " + userData.balance + "\nLevel: " + userData.level;

        //    StopVideo();
        }
    }

    // todo: rework login and register api
    public void Login() {
        //if (login_Email_usernameText.text != "" && login_passwordText.text != "") {
        //    userData = new UserData();

        //    userData.username = login_Email_usernameText.text;
        //    userData.password = login_passwordText.text;

        //    Application.ExternalCall("api_signin", userData.username, userData.password);
        //} else {
        //    info_errorText.color = Color.red;
        //    info_errorText.text = "Enter correct data";
        //}

        info_errorText.text = "";
        if (login_Email_usernameText.text != "" && login_passwordText.text != "") {
            info_errorText.text = "";
            userData = new UserData();
            userData.email_address = login_Email_usernameText.text;
            userData.password = login_passwordText.text;

            void onGetResp(WWW req) {
                if (!req.isDone) {
                    info_errorText.text = $"Error While Sending: {req.error}";
                } else {
                    var str = req.text;
                    //Debug.Log(str);
                    JSONNode info = null;
                    try {
                        info = JSON.Parse(str);
                    } catch (Exception) {
                        Debug.LogWarning(str);
                        //Debug.LogWarning(e);
                    }
                    var code = -1;

                    if (info == null) {
                        //var s = str.Split(new string[] { "<pre>" }, StringSplitOptions.None)[1];
                        //s = s.Split(new string[] { "</pre>" }, StringSplitOptions.None)[0];

                        info_errorText.text = "Server error, try again later";
                        return;
                    }

                    if (!string.IsNullOrEmpty(info["code"]))
                        code = int.Parse(info["code"].Value);
                    var msg = info["message"];
                    //var verify = int.Parse(userInfo["user_info"]["is_verified"].Value);
                    //var userName = userInfo["user_info"]["username"].Value;

                    info_errorText.text = msg;
                    var token = info["token"];

                    if (code == 200 || !string.IsNullOrEmpty(token)) {
                        //if (verify == 0) {
                        //    verifyCode_panel.SetActive(true);
                        //    RememberToggle.gameObject.SetActive(false);
                        //} else {
                        ApiAccess.auth_token = token;

                        PlayerPrefs.SetString("_PlayerEmail", userData.email_address);
                        PlayerPrefs.SetString("_PlayerPass", userData.password);

                        verifyCode_panel.SetActive(false);
                        registerGO.SetActive(false);
                        welcomePanel.SetActive(true);

                    //    StopVideo();

                        //api_get_bids_res(); //???
                        //}
                    } else {
                        info_errorText.text = msg;
                    }
                }
                ApiAccess.onResponseGetWWW -= onGetResp;
            }
            ApiAccess.onResponseGetWWW += onGetResp;

            //Debug.Log("sending request");
            info_errorText.text = "Logging in...";
            StartCoroutine(ApiAccess.Login(userData.email_address, userData.password));

        } else {
            info_errorText.text = "Enter email or password";
        }
    }

    public void api_signin_res(string ResponseData) {
        JSONNode userInfo = JSON.Parse((ResponseData).ToString());
        Debug.Log(userInfo["code"]);
        var code = Int32.Parse(userInfo["code"].Value);
        var msg = userInfo["msg"].Value;
        var verify = Int32.Parse(userInfo["user_info"]["is_verified"].Value);
        var userName = userInfo["user_info"]["username"].Value;
        var balance = userInfo["user_info"]["balance"].Value;
        var level = userInfo["user_info"]["level"].Value;

        if (code == 1) {
            ChangeText(msg, false);

            if (verify == 0) {
                verifyCode_panel.SetActive(true);
            } else {
                verifyCode_panel.SetActive(false);
                registerGO.SetActive(false);
                welcomePanel.SetActive(true);
                welcomeText.text = "Welcome \"" + userName + "\" \nBalance: " + balance + "\nLevel: " + level;

            //    StopVideo();
            }
        } else {
            ChangeText(msg, true);
        }
    }
    #endregion


    #region ******** Log OUT *********
    public void LogOut() {
        Application.ExternalCall("api_logout");
    }

    public void api_logout_res(string ResponseData) {
        JSONNode userInfo = JSON.Parse((ResponseData).ToString());
        Debug.Log(userInfo.Value);
        var code = Int32.Parse(userInfo["code"].Value);
        var msg = userInfo["msg"].Value;

        if (code == 1) {
            ChangeText(msg, false);
            registerGO.SetActive(true);
            welcomePanel.SetActive(false);
            //signedIn = false;
        } else {
            ChangeText(msg, true);
        }
    }
    #endregion


    #region*********** Forget/Change Password ***********

    public void ForgetPassword() {

        StartCoroutine(ForgetPasswordApi());
    }

    IEnumerator ForgetPasswordApi() {

        if (forgetPass_usernameText.text != "") {
            userData = new UserData();
            userData.username = forgetPass_usernameText.text;

            Application.ExternalCall("api_forget_password", userData.username);


        } else {
            info_errorText.color = Color.red;
            info_errorText.text = "Kindly Re-enter Correct info";
        }
        yield return null;
    }

    public void api_forget_password_res(string ResponseData) {

        JSONNode userInfo = JSON.Parse((ResponseData).ToString());
        Debug.Log(userInfo.Value);
        var code = Int32.Parse(userInfo["code"].Value);
        var msg = userInfo["msg"].Value;

        if (code == 1) {
            ChangeText(msg, false);
            // verifyCode_panel.SetActive(true);
            forgetPass_panel.SetActive(false);
            login_Email_usernameText.text = userData.username;

        } else {
            ChangeText(msg, true);
        }
    }



    public void ChangePassword() {

        StartCoroutine(ChangePasswordApi());
    }

    IEnumerator ChangePasswordApi() {

        if (change_passwordText.text != "" && change_confirmPasswordText.text != "" && change_passwordText.text == change_confirmPasswordText.text) {
            userData = new UserData();
            userData.password = change_passwordText.text;

            Application.ExternalCall("api_change_password", userData.password);


        } else {
            info_errorText.color = Color.red;
            info_errorText.text = "Kindly Re-enter Correct info";
        }
        yield return null;
    }

    public void api_change_password_res(string ResponseData) {

        JSONNode userInfo = JSON.Parse((ResponseData).ToString());
        Debug.Log(userInfo.Value);
        var code = Int32.Parse(userInfo["code"].Value);
        var msg = userInfo["msg"].Value;

        if (code == 1) {
            ChangeText(msg, false);
            // verifyCode_panel.SetActive(true);


            ////////////////////////////////////// ADD NEW PASSWORD CODE
            changePassPanel.SetActive(false);
            // login_usernameText.text = userData.username;

        } else {
            ChangeText(msg, true);
        }
    }

    #endregion


    #region **********  CODE VERIFICATION ***********
    public void VerifyCode() {
        if (verifyCode_Text.text != "") {
            userData = new UserData();
            userData.verification_code = verifyCode_Text.text;
            Application.ExternalCall("api_verify_code", userData.verification_code);
        } else {
            info_errorText.color = Color.red;
            info_errorText.text = "Kindly Re-enter Correct info";
        }
    }

    public void api_verify_code_res(string ResponseData) {
        JSONNode userInfo = JSON.Parse((ResponseData).ToString());
        Debug.Log(userInfo.Value);
        var code = Int32.Parse(userInfo["code"].Value);
        var msg = userInfo["msg"].Value;

        if (code == 1) {
            ChangeText(msg, false);
            verifyCode_panel.SetActive(false);
            registerGO.SetActive(false);
            welcomePanel.SetActive(true);
            welcomeText.text = "Welcome \"" + userData.username + "\"";
        } else {
            ChangeText(msg, true);
        }
    }

    public void ResendVerifyCode() {
        Application.ExternalCall("api_resend_code");
    }

    public void api_resend_code_res(string ResponseData) {
        JSONNode userInfo = JSON.Parse((ResponseData).ToString());
        Debug.Log(userInfo.Value);
        var code = Int32.Parse(userInfo["code"].Value);
        var msg = userInfo["msg"].Value;
        if (code == 1) {
            ChangeText(msg, false);
        } else {
            ChangeText(msg, true);
        }
    }
    #endregion


    #region ******** Get Online Users ********           //Not Using
    //public void GetOnlineUsers() {
    //	Application.ExternalCall("api_get_users_online");
    //}

    //public void api_get_users_online_res(string ResponseData) {
    //	JSONNode userInfo = JSON.Parse((ResponseData).ToString());
    //	Debug.Log(userInfo.Value);
    //	online_Users = new string[userInfo.Count];

    //	if (contentPanel.childCount != 0) {
    //		for (int i = 0; i < contentPanel.childCount; i++) {
    //			Destroy(contentPanel.GetChild(i).gameObject);
    //		}
    //	}

    //	for (int i = 0; i < userInfo.Count; i++) {
    //		online_Users[i] = userInfo["users_online"][i]["name"].Value;
    //		contentPanel.offsetMin = new Vector2(0, userInfo.Count * -10);
    //		TMP_Text t = Instantiate(infoText, contentPanel.transform);
    //		t.text = online_Users[i];
    //	}
    //}
    #endregion


    #region ******** Get Online Users Bid ********
    public void GetOnlineUsersBid() {
      //  Application.ExternalCall("api_get_bids");
    }

    public void api_get_bids_res(string ResponseData) {
        JSONNode userInfo = JSON.Parse((ResponseData).ToString());
        //Debug.Log(userInfo.Value);
        var length = userInfo["users_req_bids"].Count + userInfo["users_bids"].Count;
        users_Bids_Username = new string[length];
        users_Bids_Amount = new string[length];
        var totalSum = int.Parse(userInfo["bids_req_sum"].Value) + int.Parse(userInfo["bids_sum"].Value);
        tot_BidsText.text = "$" + totalSum.ToString();
        tot_UsersText.text = length + " Players";

        if (contentPanel.childCount != 0) {
            for (int i = 0; i < contentPanel.childCount; i++) {
                Destroy(contentPanel.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < userInfo["users_req_bids"].Count; i++) {
            users_Bids_Username[i] = userInfo["users_req_bids"][i]["username"].Value;
            users_Bids_Amount[i] = userInfo["users_req_bids"][i]["amount"].Value;
            //  var bidreqSum = Int32.Parse(userInfo["bids_req_sum"].Value);
            var text = Instantiate(userText, contentPanel.transform);
            text.username = users_Bids_Username[i];
            text.userBid = users_Bids_Amount[i];
            text.IsEven = i % 2 == 0;
        }

        for (int i = 0; i < userInfo["users_bids"].Count; i++) {
            users_Bids_Username[i] = userInfo["users_bids"][i]["username"].Value;
            users_Bids_Amount[i] = userInfo["users_bids"][i]["amount"].Value;
            //  var bidreqSum = Int32.Parse(userInfo["bids_req_sum"].Value);
            var text = Instantiate(userText, contentPanel.transform);
            text.username = users_Bids_Username[i];
            text.userBid = users_Bids_Amount[i];
            text.IsEven = userInfo["users_req_bids"].Count + i % 2 == 0;
        }

        var cLayout = contentPanel.GetComponent<VerticalLayoutGroup>();
        int height = (int)userText.rectTransform.rect.height * userInfo["users_bids"].Count;
        int minHeight = (int)contentPanel.GetComponentInParent<RectTransform>().rect.height;
        if (height > minHeight) {
            cLayout.padding.bottom = height;
		} else {
            cLayout.padding.bottom = minHeight;
        }
    }
    #endregion


    #region ********* ROOM APIS ********
    public void LeaveRoom() {
    //    Application.ExternalCall("api_leave_room");
    }

    public void api_leave_room_res(string ResponseData) {
        if (roomJoined) {
            JSONNode userInfo = JSON.Parse((ResponseData).ToString());
            Debug.Log(userInfo.Value);
            var code = Int32.Parse(userInfo["code"].Value);
            var msg = userInfo["msg"].Value;
            if (code == 1) {
                ChangeText(msg, false);
            } else {
                ChangeText(msg, true);
            }
        } else {
            info_errorText.color = Color.red;
            info_errorText.text = "Room Not Joined";
        }
    }

    private void OnApplicationQuit() {
        LeaveRoom();
    }

    public void JoinRoom() {
        if (betText.text != "") {
            Application.ExternalCall("api_room_join_req", betText.text);
        }
    }

    public void api_room_join_req_res(string ResponseData) {
        JSONNode userInfo = JSON.Parse((ResponseData).ToString());
        var code = Int32.Parse(userInfo["code"].Value);
        var msg = userInfo["msg"].Value;
        if (code == 1) {
            ChangeText(msg, false);
        } else {
            ChangeText(msg, true);
        }
    }
    #endregion


    #region ******** < FCM RESPONSES > ********
    public void room_info(string Response) {
        JSONNode half_info = JSON.Parse(Response);
        JSONNode info = JSON.Parse(half_info["values"].Value);

        RoomData data = new RoomData();

		data.isEnded = int.Parse(info["room_info"]["is_ended"].Value);
		data.multipliyer = float.Parse(info["room_info"]["multiplier_per"].Value);
        data.id = info["room_info"]["id"].Value;
        data.startTimeString = info["room_info"]["start_time"].Value;
        data.currentTime = info["room_info"]["current_time"].Value;
        if (data.isEnded == 0) {
            payoutText.color = Color.white;
            roundOverText.color = Color.white;
            roundOverText.text = "Current Payout";
        }

        payoutText.text = $"{data.multipliyer.ToString("F2")}x";

        var length = info["room_users_info"].Count;
        data.users = new List<RoomUserData>(length);

        for (int i = 0; i < length; i++) {
            RoomUserData user = new RoomUserData();
            user.Amount = info["room_users_info"][i]["amount"].Value;
            //user.Name = info["room_users_info"][i]["name"].Value;
            user.LeaveTime = info["room_users_info"][i]["leave_time"].Value;
            user.multiplier_level = info["room_users_info"][i]["multiplier_level"].Value;

            data.users.Add(user);
        }

        gameControl.ProcessRoomData(data);
    }

    public void room_joined(string Response) {
     //   GetOnlineUsersBid();
        // fcmText.text = Response;
        JSONNode info = JSON.Parse(Response);
        var msg = info["body"].Value;
        ChangeText(msg, false);
        roomJoined = true;
        //mover.SetActive(true);
        gameControl.StartGame();
    //    StopVideo();
    }

    public void bet_lost(string Response) {
        // GetOnlineUsersBid();
        fcmText.text = Response;
        JSONNode info = JSON.Parse(Response);
        Debug.Log(info);
        var msg = info["body"].Value;
        ChangeText(msg, false);
        gameControl.EndGame();
		StartCoroutine(BetTime());

		payoutText.color = Color.red;
        roundOverText.color = Color.red;
        roundOverText.text = "Round Over";
    }

    IEnumerator BetTime() {
        yield return new WaitForSeconds(2f);
      //  PlayVideo();
        payoutText.color = Color.white;
        roundOverText.color = Color.white;
        payoutText.text = "Bet time";
        roundOverText.text = "";
    }

    IEnumerator Restart() {
        yield return new WaitForSeconds(2f);

        payoutText.color = Color.white;
        roundOverText.color = Color.white;
        roundOverText.text = "Current Payout";
        gameControl.StartGame();
    }
    #endregion


    public void _startGame() {
        gameControl.StartGame();
    }
}

#pragma warning restore 0618