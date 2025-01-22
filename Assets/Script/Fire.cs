public abstract class FireExpansion
{
    public direction nextDirectionWanted;
    public direction nextDirection;
    public bool[] haveAlreadyExpand = new bool[4];

    public enum direction
    {
        North,South,West,Est
    }
    
    public FireExpansion(direction dirWanted,direction nextDir,bool[] AlreadyExpand)
    {
        nextDirectionWanted = dirWanted;
        nextDirection = nextDir;
        haveAlreadyExpand = AlreadyExpand;
    }

}
