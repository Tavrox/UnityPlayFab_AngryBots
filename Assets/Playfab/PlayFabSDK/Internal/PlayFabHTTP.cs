﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.Internal;

namespace PlayFab.Internal
{
	public class PlayFabHTTP : SingletonMonoBehaviour<PlayFabHTTP>  {
		
		public delegate void HTTPCallback(string response, string error);
		
		
		/// <summary>
		/// Sends a POST HTTP request
		/// </summary>
		public static void Post (string url, string data, string authType, string authKey, HTTPCallback callback)
		{
			PlayFabHTTP.instance.InstPost(url, data, authType, authKey, callback);
		}
		
		private void InstPost (string url, string data, string authType, string authKey, HTTPCallback callback)
		{
			StartCoroutine(MakeRequest (url, data, authType, authKey, callback));
		}
		
		private IEnumerator MakeRequest (string url, string data, string authType, string authKey, HTTPCallback callback)
		{
			byte[] bData = System.Text.Encoding.UTF8.GetBytes(data);

#if UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_2 || UNITY_4_0 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			// Using hashtable for compatibility with Unity < 4.5
			Hashtable headers = new Hashtable ();
#else
			Dictionary<string, string> headers = new Dictionary<string,string>();
#endif
			headers.Add("Content-Type", "application/json");
			if(authType != null)
				headers.Add(authType, authKey);
			headers.Add("X-ReportErrorAsSuccess", "true");
			headers.Add("X-PlayFabSDK", PlayFabVersion.getVersionString ());
			WWW www = new WWW(url, bData, headers);
			yield return www;
			
			if(!String.IsNullOrEmpty(www.error))
			{
				Debug.LogError(www.error);
				callback(null, www.error);
			}
			else
			{
				string response = www.text;
				callback(response, null);
			}
			
		}
	}
}
