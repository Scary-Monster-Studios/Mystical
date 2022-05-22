using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Text;
using Atavism;
using UnityEditor;
using UnityEngine;

namespace Atavims
{


    public class CookieAwareWebClient : WebClient
    {
        public CookieAwareWebClient()
        {
            CookieContainer = new CookieContainer();
            this.ResponseCookies = new CookieCollection();
        }

        public CookieContainer CookieContainer { get; private set; }
        public CookieCollection ResponseCookies { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest) base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = (HttpWebResponse) base.GetWebResponse(request);
            this.ResponseCookies = response.Cookies;
            return response;
        }
    }

    public class MenuItems : ScriptableObject
    {
        [MenuItem(@"Window/Atavism/Atavism Welcome ", priority = -800)]
        public static void EditorWindow()
        {
            AtavismWelcome.Init();
        }
    }

    [InitializeOnLoad]
    public class AtavismWelcome : EditorWindow
    {
        public static AtavismWelcome Window;
        static string sessionName = "Atavims.showedReadme";
        string currentFileDownload = null;
        private string _login = "";
        private string _pass = "";
        private string _invoice = "";
        private CookieAwareWebClient client = null;

        static AtavismWelcome()
        {
            EditorApplication.delayCall += AutoSelectReadme;

        }

        static void AutoSelectReadme()
        {
            if (!SessionState.GetBool(sessionName, false))
            {
                if (!EditorPrefs.GetBool("Dragonsan_Welcome"))
                {
                    Init();
                }

                SessionState.SetBool(sessionName, true);
            }
        }

        public static void Init()
        {
            if (AtavismWelcome.Window == null)
            {
                Window = CreateInstance<AtavismWelcome>();
                Window.ShowUtility();
            }
        }

        string login(string login, string pass, string invoice)
        {
            string url = "https://apanel.atavismonline.com/login/demo.php";
            WebClient wc = new WebClient();
            var parameters = new NameValueCollection();
            parameters.Add("action", "login");
            parameters.Add("email", login);
            parameters.Add("password", pass);
            parameters.Add("invoice", invoice);
            parameters.Add("version", AtavismClient.Version);
            var data = wc.UploadValues(url, "POST", parameters);
            var responseString = UnicodeEncoding.UTF8.GetString(data);
            return responseString;
        }


        void DownloadFile(string url, string folderpath, string filename)
        {
            client = new CookieAwareWebClient();
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadFileProgress);
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/96.0.4664.110 Safari/537.36");
            client.Headers.Add("cache-control", "no-cache");
            Uri _url = new Uri(url);
            currentFileDownload = folderpath + "/" + filename;
            client.DownloadFileAsync(_url, currentFileDownload);
        }

        void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            EditorUtility.ClearProgressBar();
            AllDone();
        }

        void AllDone()
        {
            AssetDatabase.ImportPackage(currentFileDownload, true);
        }

        void DownloadFileProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            var RecevedBytes = e.BytesReceived;
            var TotalBytes = e.TotalBytesToReceive;
            string bytes = RecevedBytes + " B";
            if (RecevedBytes > 1024)
                bytes = ((float) RecevedBytes / 1024).ToString("0.000") + " kB ";
            if (RecevedBytes > 1024 * 1024)
                bytes = ((float) RecevedBytes / (1024 * 1024)).ToString("0.000") + " MB ";
            if (RecevedBytes > 1024 * 1024 * 1024)
                bytes = ((float) RecevedBytes / (1024 * 1024 * 1024)).ToString("0.000") + " GB ";

            string TBytes = TotalBytes + " B";
            if (TotalBytes > 1024)
                TBytes = ((float) TotalBytes / 1024).ToString("0.000") + " kB ";
            if (TotalBytes > 1024 * 1024)
                TBytes = ((float) TotalBytes / (1024 * 1024)).ToString("0.000") + " MB ";
            if (TotalBytes > 1024 * 1024 * 1024)
                TBytes = ((float) TotalBytes / (1024 * 1024 * 1024)).ToString("0.000") + " GB ";

            if (EditorUtility.DisplayCancelableProgressBar("Downloading...", "Downloading Demo data " + bytes + "/" + TBytes, ((float) RecevedBytes / (float) TotalBytes)))
            {
                client.CancelAsync();
            }
        }

        private void OnGUI()
        {
            if (Window != null)
            {
                Init();
            }
            Window.titleContent = new GUIContent("Welcome in Atavism MMO Framework " + AtavismClient.Version);
            Window.minSize = new Vector2(600, 400);
            Window.maxSize = new Vector2(600, 400);
            GUIStyle DescriptionStyle = new GUIStyle(GUI.skin.label);
            GUIStyle NameStyle = new GUIStyle(GUI.skin.label);
            GUIStyle LoginStyle = new GUIStyle(GUI.skin.label);
            GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button);
            GUIStyle ButtonRegStyle = new GUIStyle(GUI.skin.button);


            DescriptionStyle.wordWrap = true;
            DescriptionStyle.fontSize = 13;
            DescriptionStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1f);

            NameStyle.fontSize = 15;
            NameStyle.normal.textColor = new Color(0f, 0.74f, 1f, 1f);
            NameStyle.wordWrap = true;

            ButtonRegStyle.hover.textColor  = new Color(0f, 0.74f, 1f, 1f);
            ButtonRegStyle.fontSize = 14;
         //   ButtonRegStyle.fixedWidth = 100;
         //   ButtonRegStyle.fixedHeight = 25;
            
            ButtonStyle.hover.textColor  = new Color(0f, 0.74f, 1f, 1f);
            ButtonStyle.fontSize = 14;
            ButtonStyle.fixedWidth = 300;
            ButtonStyle.fixedHeight = 25;
            
            LoginStyle.fontSize = 14;
            LoginStyle.fixedWidth = 80;
            LoginStyle.fixedHeight = 25;
            GUILayout.BeginVertical();
            GUILayout.Label("Welcome", NameStyle);
           GUILayout.Space(4);
            GUILayout.Label("Please follow the How To Start with Atavism Guide to set your all Atavism components.", DescriptionStyle);
            if (GUILayout.Button("How To Start",ButtonRegStyle))
            {
                Application.OpenURL("https://unity.wiki.atavismonline.com/project/how-to-start-with-atavism/");
            }
            GUILayout.Label("You have installed the Atavism Core version that doesn't contain examples.", DescriptionStyle);

            GUILayout.Space(4);
            GUILayout.Label("If you haven't registered your purchase on our website, please create an account on", DescriptionStyle);
            if (GUILayout.Button("Create Account",ButtonRegStyle))
            {
                Application.OpenURL("https://www.atavismonline.com/component/users/?view=registration");
            }

            GUILayout.Label("and register your license using your Unity Invoice or Unity Order Number.", DescriptionStyle);
            if (GUILayout.Button("Register License",ButtonRegStyle))
            {
                Application.OpenURL("https://apanel.atavismonline.com/licences/addunitylicence");
            }

            GUILayout.Space(4);
            GUILayout.Label("If you want to import examples, provide credentials that you used for registration on our website and click Login & Download & Import Example Data button.", DescriptionStyle);
            GUILayout.Space(2);

            GUI.color = Color.white;
            //login
            GUILayout.BeginHorizontal();
            GUILayout.Space(Screen.width / 2 - 300 / 2 - 80 - 4);
            GUILayout.Label("Login", LoginStyle);
            _login = GUILayout.TextField(_login, ButtonStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(Screen.width / 2 - 300 / 2 - 80 - 4);
            GUILayout.Label("Password", LoginStyle);
            _pass = GUILayout.PasswordField(_pass, '*', ButtonStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(Screen.width / 2 - 300 / 2);
            if (GUILayout.Button("Login & Download & Import Example Data", ButtonStyle))
            {
                var url = login(_login, _pass, _invoice);
                url = url.Trim();
                if (!string.IsNullOrEmpty(url))
                {
                    DownloadFile(url, "Assets/Dragonsan", "AtavismExamples_" + AtavismClient.Version + ".unitypackage");
                }
                else
                {
                    EditorUtility.DisplayDialog("Login", "Provided credentials are incorrect", "Ok");
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("Example package is around 5GB+ and if for some reason your download won't be successful, you can download it manually from apanel.", DescriptionStyle);
            if (GUILayout.Button("Download",ButtonRegStyle))
            {
                Application.OpenURL("https://apanel.atavismonline.com/account/downloads?release="+AtavismClient.Version);
            }

            GUILayout.Space(7);
            var shownect = GUILayout.Toggle(EditorPrefs.GetBool("Dragonsan_Welcome"), "Don't show again");
            EditorPrefs.SetBool("Dragonsan_Welcome", shownect);
        }

    }
}