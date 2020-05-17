namespace DemoJSON
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Security.Cryptography;
	using UnityEngine;

	public static class Utilities
	{
		/// <summary>
		/// Gets a Random Item from the list. Not 'that' random though...
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="_list"></param>
		/// <returns></returns>
		public static T GetRandomItem<T>(this List<T> _list)
		{
			if (_list.Count == 0)
				return default;
			return _list[UnityEngine.Random.Range(0, _list.Count-1)];
		}

		/// <summary>
		/// Returns the MD5 from a file in StreamingAssets
		/// </summary>
		/// <param name="_fileName"></param>
		/// <returns></returns>
		public static string GetFileMD5(string _fileName)
		{
			using (var md5 = MD5.Create())
			{
				string _fullPath = Path.Combine(Application.streamingAssetsPath, _fileName);
				if (!File.Exists(_fullPath))
					return string.Empty;

				using (var stream = File.OpenRead(_fullPath))
				{
					var hash = md5.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
				}
			}
		}

		public static bool CheckFileHasChanged(string _fileName, string _lastMD5)
		{
			string _fullPath = Path.Combine(Application.streamingAssetsPath, _fileName);
			if (!File.Exists(_fullPath))
			{
				Debug.LogError($"File {_fileName} doesn't exists. \nfullPath: {_fullPath}");
				return false;
			}
			return !_lastMD5.Equals(Utilities.GetFileMD5(_fileName));
		}

		public static string GetFileContent(string _fileName, out string _md5)
		{
			string _fullPath = Path.Combine(Application.streamingAssetsPath, _fileName);
			if (!File.Exists(_fullPath))
			{
				Debug.LogError($"File {_fileName} doesn't exists. \nfullPath: {_fullPath}");
				_md5 = string.Empty;
				return string.Empty;
			}

			_md5 = Utilities.GetFileMD5(_fileName);

			return File.ReadAllText(_fullPath);
		}

		public static void DelayedActionAfterEndOfFrame(this MonoBehaviour mono, Action _action)
		{
			mono.StartCoroutine(mono.DelayedCoroutineAfterEndOfFrame(_action));
		}

		public static void DelayedActionAfterSeconds(this MonoBehaviour mono, float _time, Action _action)
		{
			mono.StartCoroutine(mono.DelayedCoroutineAfterSeconds(_time, _action));
		}

		public static void DelayedActionAfterSecondsRealtime(this MonoBehaviour mono, float _time, Action _action)
		{
			mono.StartCoroutine(mono.DelayedCoroutineAfterSecondsRealtime(_time, _action));
		}

		public static IEnumerator DelayedCoroutineAfterEndOfFrame(this MonoBehaviour mono, Action _action)
		{
			yield return new WaitForEndOfFrame();
			_action?.Invoke();
		}

		public static IEnumerator DelayedCoroutineAfterSeconds(this MonoBehaviour mono, float _time, Action _action)
		{
			yield return new WaitForSeconds(_time);
			_action?.Invoke();
		}

		public static IEnumerator DelayedCoroutineAfterSecondsRealtime(this MonoBehaviour mono, float _time, Action _action)
		{
			yield return new WaitForSecondsRealtime(_time);
			_action?.Invoke();
		}
	}

}