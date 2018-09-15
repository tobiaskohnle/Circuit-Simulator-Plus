namespace CircuitSimulatorPlus
{
    public interface IClickable
    {
        Hitbox Hitbox
        {
            get;
        }
        bool IsSelected
        {
            get; set;
        }
    }
}
