using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EasySteamLeaderboard;

public class PostHighScore : MonoBehaviour
{
	public void ResetUI()
	{
		//Do Nothing
	}

	void FetchLeaderboardWithID(string lbid, int startRange, int endRange)
	{
		EasySteamLeaderboards.Instance.GetLeaderboard(lbid, (result) =>
		{
			
		}, startRange, endRange); //fetch top 20 entries
	}

	public void UploadScoreToLeaderboard(int pScore)
	{
		int _Score = 0;
		string lbid = "GSC_LeaderBoard";
		_Score = pScore;

		EasySteamLeaderboards.Instance.UploadScoreToLeaderboard(lbid, _Score, (result) =>
		{
			//check if leaderboard successfully fetched
			if (result.resultCode == ESL_ResultCode.Success)
			{
				Debug.Log("Succesfully Uploaded!");

				//refresh lbid
				FetchLeaderboardWithID(lbid, 1, 1);
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
