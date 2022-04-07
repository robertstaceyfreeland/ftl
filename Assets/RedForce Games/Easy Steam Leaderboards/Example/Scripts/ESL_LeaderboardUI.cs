using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EasySteamLeaderboard;

public class ESL_LeaderboardUI : MonoBehaviour
{

	//ase
	public GameObject EntriesContainer;
	public GameObject LBEntryPrefab;
	public InputField Fetch_IDField;
	public InputField Upload_IDField;
	public InputField Upload_ScoreField;
	public ESL_LeaderboardEntryUI yourEntryUI;

	//enum
	public enum LeaderboardFilter
	{
		Global,
		Friends
	}

	//vars
	LeaderboardFilter currentFilter;
	List<GameObject> entriesObjs = new List<GameObject>();
	ESL_Leaderboard lbCache;

	private void Start()
	{
		StartCoroutine(GetGscLeaderBoard());
	}

	void OnEnable()
	{
		ESL_LeaderboardFilterSelector.onFilterSelected += ESL_LeaderboardFilterSelector_onFilterSelected;
	}

	void OnDisable()
	{
		ESL_LeaderboardFilterSelector.onFilterSelected -= ESL_LeaderboardFilterSelector_onFilterSelected;
	}

	IEnumerator GetGscLeaderBoard()
	{
		yield return new WaitForSeconds(3);

		FetchLeaderboard("GSC_LeaderBoard");
	}

	void ESL_LeaderboardFilterSelector_onFilterSelected(LeaderboardFilter filter)
	{
		currentFilter = filter;

		//if cached then repopulate
		if (lbCache != null)
			PopulateEntriedBasedOnFilter();
			
	}

	void PopulateEntriedBasedOnFilter()
	{
		StopAllCoroutines();
		if (currentFilter == LeaderboardFilter.Global)
			StartCoroutine(PopulateEntries(lbCache.GlobalEntries));
		else if (currentFilter == LeaderboardFilter.Friends)
			StartCoroutine(PopulateEntries(lbCache.FriendsEntries));
	}

	IEnumerator PopulateEntries(List<ESL_LeaderboardEntry> entries)
	{
		//reset current ui
		ResetUI();

		//populate your entry if it exists
		yourEntryUI.Initialize(lbCache.SteamUserEntry);

		for (int i = 0; i < entries.Count; i++)
		{
			//instantiate prefab
			GameObject entry = Instantiate(LBEntryPrefab) as GameObject;

			//if gloabl entries show global rank
			if (currentFilter == LeaderboardFilter.Global)
				entry.GetComponent<ESL_LeaderboardEntryUI>().Initialize(entries[i]);
			else if (currentFilter == LeaderboardFilter.Friends) //if friends, then show local rank among friends
				entry.GetComponent<ESL_LeaderboardEntryUI>().Initialize(entries[i].PlayerName, (i + 1), entries[i].Score);

			//set transform to container
			entry.transform.SetParent(EntriesContainer.transform);
			entry.transform.localScale = Vector3.one;

			//local obj cache
			entriesObjs.Add(entry);

			yield return null;
		}
	}

	void ResetUI()
	{
		for (int i = 0; i < entriesObjs.Count; i++)
		{
			Destroy(entriesObjs[i]);
		}

		entriesObjs.Clear();

		//reset your entry ui
		yourEntryUI.Reset();
	}

	void FetchLeaderboardWithID(string lbid, int startRange, int endRange)
	{
		EasySteamLeaderboards.Instance.GetLeaderboard(lbid, (result) =>
			{
				//check if leaderboard successfully fetched
				if (result.resultCode == ESL_ResultCode.Success)
				{
					lbCache = result;
					PopulateEntriedBasedOnFilter();
				}
				else
				{
					Debug.Log("Failed Fetching: " + result.resultCode.ToString());
					StopAllCoroutines();
					ResetUI();
				}
			}, startRange, endRange); //fetch top 20 entries
	}

	//ID fetched from input field directly
	public void FetchLeaderboard()
	{
		string lbid = Fetch_IDField.text; //get id from input field from user
		FetchLeaderboardWithID(lbid, 1, 20);
	}
	public void FetchLeaderboard(string pLeaderBoard)
	{
		FetchLeaderboardWithID(pLeaderBoard, 1, 20);
	}

	//id and score got from input field directly
	public void UploadScoreToLeaderboard()
	{
		int score = 0;
		string lbid = "GSC_LeaderBoard";
		if (!Upload_ScoreField.text.Equals(""))
			score = int.Parse(Upload_ScoreField.text);

		EasySteamLeaderboards.Instance.UploadScoreToLeaderboard(lbid, score, (result) =>
			{
				//check if leaderboard successfully fetched
				if (result.resultCode == ESL_ResultCode.Success)
				{
					Debug.Log("Succesfully Uploaded!");

					//refresh lbid
					FetchLeaderboardWithID(lbid, 1, 20);
				}
				else
				{
					Debug.Log("Failed Uploading: " + result.resultCode.ToString());
					StopAllCoroutines();
					ResetUI();
				}
			});
	}
}
