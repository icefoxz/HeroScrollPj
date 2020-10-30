using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using cn.mob.unity3d.sdkporter;

namespace cn.SMSSDK.Unity
{
#if UNITY_IPHONE
	[CustomEditor(typeof(SMSSDK))]
	[ExecuteInEditMode]
	public class SMSSDKConfigEditor : Editor {
		string appKey = "";
		string appSecret = "";

		void Awake()
		{
			Prepare ();
		}
			
		public override void OnInspectorGUI()
		{
			EditorGUILayout.Space ();
			appKey = EditorGUILayout.TextField ("MobAppKey", appKey);
			appSecret = EditorGUILayout.TextField ("MobAppSecret", appSecret);
			Save ();
		}
			
		private void Prepare()
		{
			try
			{
				var files = System.IO.Directory.GetFiles(Application.dataPath , "MOB.keypds", System.IO.SearchOption.AllDirectories);
				string filePath = files [0];
				FileInfo projectFileInfo = new FileInfo( filePath );
				if(projectFileInfo.Exists)
				{
					StreamReader sReader = projectFileInfo.OpenText();
					string contents = sReader.ReadToEnd();
					sReader.Close();
					sReader.Dispose();
					Hashtable datastore = (Hashtable)MiniJSON.jsonDecode( contents );
					appKey = (string)datastore["MobAppKey"];
					appSecret = (string)datastore["MobAppSecret"];
				}
				else
				{
					Debug.LogWarning ("MOB.keypds no find");
				}
			}
			catch(Exception e) 
			{
				if(appKey.Length == 0)
				{
					appKey = "30f84d47e6298";
					appSecret = "c4c995dd348612437192739aeecb8e60";
				}
				Debug.LogException (e);
			}
		}
			
		private void Save()
		{
			try
			{
				var files = System.IO.Directory.GetFiles(Application.dataPath , "MOB.keypds", System.IO.SearchOption.AllDirectories);
				string filePath = files [0];
				if(File.Exists(filePath))
				{
					Hashtable datastore = new Hashtable();
					datastore["MobAppKey"] = appKey;
					datastore["MobAppSecret"] = appSecret;
					var json = MiniJSON.jsonEncode(datastore);
					StreamWriter sWriter = new StreamWriter(filePath);
					sWriter.WriteLine(json);
					sWriter.Close();
					sWriter.Dispose();
				}
				else
				{
					Debug.LogWarning ("MOB.keypds no find");
				}
			}
			catch(Exception e) 
			{
				Debug.LogWarning ("error");
				Debug.LogException (e);
			}
		}
	}
#endif
}