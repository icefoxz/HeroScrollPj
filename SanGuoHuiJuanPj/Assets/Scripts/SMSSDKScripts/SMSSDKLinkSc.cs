using UnityEngine;
using UnityEngine.UI;

namespace cn.SMSSDK.Unity
{
    public class SMSSDKLinkSc : MonoBehaviour, SMSSDKHandler
    {
        [HideInInspector]
        public SMSSDK smssdk;
        public UserInfo userInfo;

        public string appKey = "30f84d47e6298";
        public string appSerect = "c4c995dd348612437192739aeecb8e60";
        [HideInInspector]
        public bool isWarn = false;

        [SerializeField]
        Button showResPageBtn;

        private void Awake()
        {
            isCanCallBack = true;

            print("print: [SMSSDK]  ===>>>  Awake");
            smssdk = gameObject.GetComponent<SMSSDK>();

            showResPageBtn.onClick.AddListener(TryShowRegisterPage);
        }

        private void Start()
        {
            userInfo = new UserInfo();
            smssdk.init(appKey, appSerect, isWarn);
            smssdk.setHandler(this);
            //Debug.Log("[SMSSDK]  ===>>>  Start");
        }

        //验证模板
        private string tempCode = null; //1319972

        //尝试弹出短信验证界面
        private void TryShowRegisterPage()
        {
            // 模板号可以为空
            smssdk.showRegisterPage(CodeType.TextCode, tempCode);
        }

        //[SerializeField]
        //InputField textField;

        private string phoneNum = "";
        private string zoneNums = "86";

        //尝试提交用户信息
        private void TrySubmitUserInfo()
        {
            //phoneNum = textField.text;

            userInfo.avatar = "www.mob.com";
            userInfo.phoneNumber = phoneNum;
            userInfo.zone = zoneNums;
            userInfo.nickName = "David";
            userInfo.uid = "1234567890";
            smssdk.submitUserInfo(userInfo);
        }

        //尝试获取好友列表
        private void TryGetFriends()
        {
            smssdk.getFriends();
        }

        //尝试获取版本信息
        private void TryGetVersionNumber()
        {
            smssdk.getVersion();
        }

        private string result = null;   //回调消息

        private bool isCanCallBack;  //是否可以接受回调响应
        private void ReplyCanCallBack()
        {
            isCanCallBack = true;
        }

        public void onComplete(int action, object resp)
        {
            ActionType act = (ActionType)action;
            //Debug.Log("action :" + action);
            //ShowDebugText("action :" + action);
            if (resp != null)
            {
                result = resp.ToString();
            }
            if (act == ActionType.GetCode)
            {
                string responseString = (string)resp;
                //Debug.Log("isSmart :" + responseString);
            }
            else if (act == ActionType.GetVersion)
            {
                string version = (string)resp;
                //Debug.Log("version :" + version);
                print("Demo*version*********" + version);

            }
            else if (act == ActionType.GetSupportedCountries)
            {
                string responseString = (string)resp;
                //Debug.Log("zoneString :" + responseString);

            }
            else if (act == ActionType.GetFriends)
            {
                string responseString = (string)resp;
                //Debug.Log("friendsString :" + responseString);
            }
            else if (act == ActionType.CommitCode)
            {
                //短信验证码成功
                string responseString = (string)resp;
                //Debug.Log("commitCodeString :" + responseString);
                //start场景短信验证绑定手机
                if (isCanCallBack)
                {
                    isCanCallBack = false;
                    StartSceneToServerCS.instance.SMSVerifiedSuccessedFun(responseString);
                    Invoke("ReplyCanCallBack", 3f);
                }
            }
            else if (act == ActionType.SubmitUserInfo)
            {
                string responseString = (string)resp;
                //Debug.Log("submitString :" + responseString);
            }
            else if (act == ActionType.ShowRegisterView)
            {
                string responseString = (string)resp;
                //Debug.Log("showRegisterView :" + responseString);
            }
            else if (act == ActionType.ShowContractFriendsView)
            {
                string responseString = (string)resp;
                //Debug.Log("showContractFriendsView :" + responseString);
            }
        }

        public void onError(int action, object resp)
        {
            //Debug.Log("action :" + action);
            //Debug.Log("Error :" + resp);
            result = resp.ToString();
            print("OnError ******resp" + resp);
        }
    }
}