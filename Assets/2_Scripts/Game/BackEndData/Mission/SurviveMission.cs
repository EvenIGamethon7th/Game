using System;
using UnityEngine;

[Serializable]
public class SurviveMission
{
    public const int MAX_SURVIVE_COUNT = 3;
    
    public int surviveCount = 0; 
    public int maxWaveCount = 0;
    
    public void SetMaxWaveCount(int waveCount)
    {
        maxWaveCount = Mathf.Max(waveCount, maxWaveCount);
    }
    public bool IsAdmissionSurviveMission()
    {
        if(++surviveCount >= MAX_SURVIVE_COUNT)
            return true;
        
        return false;
    }
    
}
