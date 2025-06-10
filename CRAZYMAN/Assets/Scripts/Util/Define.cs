public class Define
{
    public enum UIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
    }

    public enum Scene
    {
        Unknown,
        Dev,
        Game
    }

    public enum Sound
    {
        Bgm,
        Merge,
        Throw,
        UseItem,
        GetItem,
        PlayerWalk,
        PlayerRun,
        PlayerDie,
        EnemyWalk,
        EnemyRun,
        EnemyDie,
        DoorInteract,
        Flashlight,
        UseCamera,
        UsePill,
        swallow,
        GateOpen,
    }

    public enum Item
    {
        LevelUp,
        Delete,
        Revive
    }
}