
using System;

[Serializable]
public class SurviveMission
{
    private const int MAX_SURVIVE_COUNT = 3;
    
    public int surviveCount = 0; 
    public int maxSurviveCount = 0;
    
    public bool IsAdmissionSurviveMission()
    {
        if(++surviveCount >= maxSurviveCount)
            return true;
        
        return false;
    }
    
}
