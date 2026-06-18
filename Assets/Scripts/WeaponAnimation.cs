[System.Serializable]
public struct WeaponAnimation
{
    public AnimationType Type;

   
    
    public AnimationLayer Hold;
    public AnimationLayer Reload;
    public AnimationLayer BoltOpen;
    public AnimationLayer BoltClose;
}

[System.Serializable]
public struct AnimationLayer
{
    public string BottomAnim;
    public string TopAnim;
}