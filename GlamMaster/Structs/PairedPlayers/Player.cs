namespace GlamMaster.Structs;

public class Player
{
    public string playerName; // The paired player first and last name (Ex. : "Some Player")
    public string homeWorld; // The paired player world

    public Player(string playerName, string homeWorld)
    {
        this.playerName = playerName;
        this.homeWorld = homeWorld;
    }
}
