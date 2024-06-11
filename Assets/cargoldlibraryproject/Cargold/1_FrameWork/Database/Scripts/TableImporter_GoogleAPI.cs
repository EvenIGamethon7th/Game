using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System.Text;
using static Cargold.LibraryRemocon.FrameWorkData.DatabaseData;
using Sirenix.OdinInspector;

namespace Cargold.DB.TableImporter
{
    // GoogleSheetsToUnity 에셋에서 가져옴
    public class TableImporter_GoogleAPI
    {
        public const string GetGoogleApiIdStr = "127476093341-jsmgu7iuo25dpudqu2i3alnin27p8kdc.apps.googleusercontent.com";
        public const string GetGoogleApiPasswordStr = "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";
        public const string PortStr = "http://127.0.0.1:{0}";

        private const string requestStr = 
            "https://accounts.google.com/o/oauth2/v2/auth?client_id={0}&redirect_uri={1}&response_type=code&scope={2}&access_type=offline&prompt=consent&";
        private static string authToken = "";
        private static int remoconID;

        private static HttpListener httpListener;
        private static string htmlResponseContent = @"
<h1>(Cargold Table Importer)</h1>
<h3>Google access permit.
Go back your unity project.<h3>";

        private static object notifyAuthTokenLock = new object();
        private static bool shouldNotifyAuthTokenReceived = false;
        private static Action<string> onCompleteDel;

        private static TableImporterData GetData => TableImporter_C.GetData;

#if UNITY_EDITOR
        public static void GoogleAccessVaild_Func(int _remoconID)
        {
            remoconID = _remoconID;

            if (httpListener != null)
            {
                httpListener.Abort();
                httpListener = null;
            }
            onCompleteDel = null;

            httpListener = new HttpListener();
            TableImporterData.TableImporterRemocon _remoconData = GetData.GetRemoconData_Func(_remoconID);
            string _portStr = string.Format(TableImporter_GoogleAPI.PortStr, _remoconData.GetPort);
            httpListener.Prefixes.Add(_portStr + "/");
            httpListener.Start();
            httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), httpListener);

            onCompleteDel += GetAuthComplete;

            string _requestStr = string.Format(requestStr
                , Uri.EscapeDataString(TableImporter_GoogleAPI.GetGoogleApiIdStr)
                , Uri.EscapeDataString(_portStr)
                , Uri.EscapeDataString("https://www.googleapis.com/auth/spreadsheets")
                );

            Application.OpenURL(_requestStr);
        }

        static void GetAuthComplete(string _authToken)
        {
            TableImporterData.TableImporterRemocon _remoconData = GetData.GetRemoconData_Func(remoconID);
            string _portStr = string.Format(TableImporter_GoogleAPI.PortStr, _remoconData.GetPort);

            WWWForm _wwwForm = new WWWForm();
            _wwwForm.AddField("code", _authToken);
            _wwwForm.AddField("client_id", TableImporter_GoogleAPI.GetGoogleApiIdStr);
            _wwwForm.AddField("client_secret", TableImporter_GoogleAPI.GetGoogleApiPasswordStr);
            _wwwForm.AddField("redirect_uri", _portStr);
            _wwwForm.AddField("grant_type", "authorization_code");
            _wwwForm.AddField("scope", "");

            Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(GetToken(_wwwForm), LibraryRemocon.Instance);
        }

        static IEnumerator GetToken(WWWForm _wwwForm)
        {
            using (UnityWebRequest _request = UnityWebRequest.Post("https://accounts.google.com/o/oauth2/token", _wwwForm))
            {
                yield return _request.SendWebRequest();

                Debug_C.Log_Func("구글 접근 권한 요청 완료");

                GoogleRequestData _googleRequestData = JsonUtility.FromJson<GoogleRequestData>(_request.downloadHandler.text);
                TableImporterData.TableImporterRemocon _remoconData = GetData.GetRemoconData_Func(remoconID);
                _remoconData.SetAccessToken_Func(_googleRequestData.access_token);
                
                Editor_C.SetSaveAsset_Func(LibraryRemocon.Instance);

                TableImporter_C.CheckGoogleAccessVaildDone_Func(remoconID);
            }
        }

        static void ListenerCallback(IAsyncResult result)
        {
            if (httpListener != null)
            {
                try
                {
                    HttpListenerContext context = httpListener.EndGetContext(result);
                    HandleListenerContextResponse(context);
                    ProcessListenerContext(context);

                    context.Response.Close();
                    httpListener.BeginGetContext(ListenerCallback, httpListener); // EndGetContext above ends the async listener, so we need to start it up again to continue listening.
                }
                catch (ObjectDisposedException)
                {
                    // Intentionally ignoring this exception because it will be thrown when we stop listening.
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message + " : " + exception.StackTrace); // Just in case...
                }
            }
        }

        static void ProcessListenerContext(HttpListenerContext context)
        {
            // Attempt to pull out the URI fragment as a part of the query string.
            string uriFragment = context.Request.QueryString["code"];
            if (uriFragment != null)
            { // If it worked, that means we're being passed the auth token from Instagram, so pull it out and notify that we received it.
                string authToken = uriFragment.Replace("access_token=", "");
                NotifyAuthTokenReceived(authToken);
            }
        }

        /// <summary>
        /// Child classes should call this once the auth token has been successfully retrieved.</summary>
        static void NotifyAuthTokenReceived(string _authToken)
        {
            lock (notifyAuthTokenLock)
            {
                // We're not directly calling _onComplete() here because we're still on HttpListener's async thread.
                // We need _onComplete() to be called on the main thread, so we store the auth token and set a flag
                // that will tell us when we should call _onComplete() in the Update() method, which always executes
                // on the main thread.
                TableImporter_GoogleAPI.authToken = _authToken;
                shouldNotifyAuthTokenReceived = true;

                //Coroutine_C.StartCoroutine_Func(CheckForTokenRecieve());
                Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(CheckForTokenRecieve(), LibraryRemocon.Instance);
            }
        } 


        //Background Processes....
        static IEnumerator CheckForTokenRecieve()
        {
            lock (notifyAuthTokenLock)
            {
                // using a lock here because we'll be modifying _shouldNotifyAuthTokenReceived on both the main thread and on HttpListener's async thread.
                if (shouldNotifyAuthTokenReceived)
                {
                    if (onCompleteDel != null)
                    {
                        onCompleteDel(authToken);
                    }
                    shouldNotifyAuthTokenReceived = false;
                }
                else
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Some HTML response content was passed in to the StartListening() method, and this is where we display it to the user.</summary>
        static void HandleListenerContextResponse(HttpListenerContext context)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(htmlResponseContent);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }

        ///// <summary>
        ///// checks if time has expired far enough that a new auth token needs to be issued
        ///// </summary>
        ///// <returns></returns>
        //public static IEnumerator CheckForRefreshOfToken()
        //{
        //    TableImporterData.TableImporterRemocon _remoconData = GetData.GetRemoconData_Func(remoconID);
        //    GoogleRequestData _googleRequestData = _remoconData.GetGoogleRequestData;

        //    if (DateTime.Now > _googleRequestData.nextRefreshTime)
        //    {
        //        WWWForm _www = new WWWForm();
        //        _www.AddField("client_id", TableImporter_GoogleAPI.GetGoogleApiIdStr);
        //        _www.AddField("client_secret", TableImporter_GoogleAPI.GetGoogleApiPasswordStr);
        //        _www.AddField("refresh_token", _googleRequestData.refresh_token);
        //        _www.AddField("grant_type", "refresh_token");
        //        _www.AddField("scope", "");

        //        using (UnityWebRequest request = UnityWebRequest.Post("https://www.googleapis.com/oauth2/v4/token", _www))
        //        {
        //            yield return request.SendWebRequest();

        //            GoogleRequestData _newGdr = JsonUtility.FromJson<GoogleRequestData>(request.downloadHandler.text);
        //            _googleRequestData.access_token = _newGdr.access_token;
        //            _googleRequestData.nextRefreshTime = DateTime.Now.AddDays(_newGdr.expires_in);

        //            _remoconData.SetGoogleRequestData_Func(_googleRequestData);

        //            Editor_C.SetSaveAsset_Func(LibraryRemocon.Instance);
        //        }
        //    }
        //}
#endif
    }

    [System.Serializable]
    public struct GoogleRequestData
    {
        public string access_token;
        public string refresh_token;
        public string token_type;
        public int expires_in;
        public DateTime nextRefreshTime;

        [ShowInInspector] public string nextRefreshTimeStr => this.nextRefreshTime.ToString();
    }
}