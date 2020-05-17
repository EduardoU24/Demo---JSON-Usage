namespace DemoJSON
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public class TableController : MonoBehaviour
	{
		[Header("Last Data")]
		public string lastFileMD5;
		public string lastFileName;
		public DemoTableData loadedData;

		[Header("Watch Intervals")]
		[SerializeField]
		private bool watchFileChangesOnStart = false;
		[SerializeField]
		private float watchFileChangesIntervalTime = 4f;
		private Coroutine _watchFileChangesCoroutine;

		[Header("UI Building")]
		public TextMeshProUGUI title;
		public Button button;
		public RectTransform contentHolder;
		public TableRowUIItem tableRowUIItemPrefab;
		public List<TableRowUIItem> tableRowBuiltUIItems = new List<TableRowUIItem>();

		[Header("TestFilePaths - Remove This")]
		public List<string> testJsonFilePaths;

		void Start()
		{
			if(watchFileChangesOnStart)
			{
				_watchFileChangesCoroutine = StartCoroutine(IEStart());
			}
		}

		void OnDisable()
		{
			if(_watchFileChangesCoroutine != null)
				StopCoroutine(_watchFileChangesCoroutine);
		}

		IEnumerator IEStart()
		{
			while(true)
			{
				yield return new WaitForSeconds(watchFileChangesIntervalTime);

				if(Utilities.CheckFileHasChanged(lastFileName, lastFileMD5))
				{
					Debug.Log($"File has changed, reloading...");
					FetchLastFile();
				}
			}
		}

		[ContextMenu("FetchRandomFile")]
		public void FetchRandomFile()
		{
			string _filePath = testJsonFilePaths.GetRandomItem();
			loadedData = FetchDataFromFile(_filePath, out string _md5, out bool _changed);
			lastFileMD5 = _md5;
			lastFileName = _filePath;

			if(_changed)
				Build();
		}

		[ContextMenu("FetchLastFile")]
		public void FetchLastFile()
		{
			loadedData = FetchDataFromFile(lastFileName, out string _md5, out bool _changed);
			lastFileMD5 = _md5;
			Debug.Log($"Changed: {_changed}");
			if(_changed)
				Build();
		}

		[ContextMenu("Build")]
		public void Build()
		{
			Debug.Log($"Building...");
			if (!Application.isPlaying)
			{
				Debug.Log("Not builing on editor...");
				return;
			}

			button.interactable = false;

			CleanBuiltItems();

			title.text = loadedData.Title;

			this.DelayedActionAfterEndOfFrame(() => {

				TableRowUIItem _headerRow = Instantiate(tableRowUIItemPrefab, contentHolder);
				List<DemoTableColumnData> _headerColumns = new List<DemoTableColumnData>();

				foreach (string _header in loadedData.ColumnHeaders)
				{
					_headerColumns.Add(new DemoTableColumnData()
					{
						key = "header",
						value = _header
					});
				}

				_headerRow.Build(_headerColumns, true);
				tableRowBuiltUIItems.Add(_headerRow);


				foreach (DemoTableRowData _column in loadedData.Data)
				{
					TableRowUIItem _row = Instantiate(tableRowUIItemPrefab, contentHolder);
					_row.Build(_column.items, false);

					tableRowBuiltUIItems.Add(_row);
				}

				button.interactable = true;

			});
		}

		[ContextMenu("CleanBuiltItems")]
		public void CleanBuiltItems()
		{
			foreach(var _item in tableRowBuiltUIItems)
			{
				_item.gameObject.SetActive(false);
				Destroy(_item, 0.1f);
			}
			tableRowBuiltUIItems.Clear();
		}






		public DemoTableData FetchDataFromFile(string _filePath, out string _md5, out bool _changed)
		{
			if (!Utilities.CheckFileHasChanged(_filePath, lastFileMD5))
			{
				Debug.Log($"Data hasn't changed on {_filePath}. Nothing to do here");
				_md5 = lastFileMD5;
				_changed = false;
				return loadedData;
			}
			_changed = true;

			string _fileContent = Utilities.GetFileContent(_filePath, out string _newMD5);
			_md5 = _newMD5;

			// I leave this because of native implementation... but well... you added a dictionary and I didn't noticed it before.
			//DemoTableData _newData = JsonUtility.FromJson<DemoTableData>(_fileContent);

			DemoTableData _newData = new DemoTableData()
			{
				ColumnHeaders = new List<string>(),
				Data = new List<DemoTableRowData>()
			};

			JSONObject jsonObject = new JSONObject(_fileContent);

			_newData.Title = jsonObject["Title"].str;

			jsonObject["ColumnHeaders"].list.ForEach((_item) =>
			{
				_newData.ColumnHeaders.Add(_item.str);
			});

			foreach(JSONObject _data in jsonObject["Data"].list)
			{
				DemoTableRowData _newRow = new DemoTableRowData()
				{
					items = new List<DemoTableColumnData>()
				};

				for (int i = 0; i < _data.list.Count; i++)
				{
					DemoTableColumnData _item = new DemoTableColumnData()
					{
						key = _data.keys[i],
						value = _data.list[i].str
					};
					_newRow.items.Add(_item);
				}

				_newData.Data.Add(_newRow);
			}

			return _newData;
		}
	}
}