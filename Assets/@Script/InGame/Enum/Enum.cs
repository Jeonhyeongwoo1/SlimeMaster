namespace SlimeMaster.InGame.Enum
{
    public enum GameState
    {
        None = -1,
        Ready,
        Start,
        End
    }

    public enum GameEventType
    {
        None = -1,
        GameOver,
        SpawnObject = 100,
    }
}