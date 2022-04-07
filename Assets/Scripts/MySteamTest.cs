using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class MySteamTest : MonoBehaviour
{
    private SteamLeaderboard_t m_SteamLeaderboard;
    private CallResult<LeaderboardScoreUploaded_t> OnLeaderboardScoreUploadedCallResult;
    private CallResult<LeaderboardFindResult_t> OnLeaderboardFindResultCallResult;


    private void Awake()
    {
        //OnLeaderboardFindResultCallResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
    }
    void Start()
    {


        OnLeaderboardFindResultCallResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);

        SteamAPICall_t handle = SteamUserStats.FindLeaderboard("GSC_LeaderBoard");
        //OnLeaderboardFindResultCallResult.Set(handle);

        SteamUserStats.UploadLeaderboardScore(m_SteamLeaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, 10, null, 0);

        //SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore(m_SteamLeaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, 10, null, 0);
        //OnLeaderboardScoreUploadedCallResult.Set(handle);
        //print("SteamUserStats.UploadLeaderboardScore(" + m_SteamLeaderboard + ", " + ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate + ", " + 10 + ", " + null + ", " + 0 + ") : " + handle);
    }

    void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        Debug.Log("[" + LeaderboardFindResult_t.k_iCallback + " - LeaderboardFindResult] - " + pCallback.m_hSteamLeaderboard + " -- " + pCallback.m_bLeaderboardFound);

        if (pCallback.m_bLeaderboardFound != 0)
        {
            m_SteamLeaderboard = pCallback.m_hSteamLeaderboard;
        }
    }
}
