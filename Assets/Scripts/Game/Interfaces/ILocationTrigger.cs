namespace Foxberry
{
    public interface ILocationTrigger
    {
        string LocationName { get; }

        void TriggerLocation();
    }
}
