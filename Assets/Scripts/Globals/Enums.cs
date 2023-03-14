
namespace AQEngine.Globals
{
    public enum Direction
    {
        LEFT = -180,
        RIGHT = 0,
        FRONT = 90
    }

    public enum PowerUps
    {
        None,
        Sling = 1 << 1,
        Sword = 1 << 2,
        Shield = 1 << 3
    }
}
