namespace GlamMaster.Structs.Payloads;

public class FullPayloadToPlayer
{
    public Player FromPlayer;
    public Player ToPlayer;

    public string Payload;

    public FullPayloadToPlayer(Player FromPlayer, Player ToPlayer, string Payload)
    {
        this.FromPlayer = FromPlayer;
        this.ToPlayer = ToPlayer;
        this.Payload = Payload;
    }
}
