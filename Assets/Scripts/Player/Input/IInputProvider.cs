namespace Player.Input
{
    public interface IInputProvider
    {
        float HorizontalAxis { get; }
        bool IsJumpPressed { get; }
        bool IsAttackPressed { get; }
        bool IsSecondaryAttackPressed { get; }
        bool IsSlidePressed { get; }
        bool IsLiftPressed { get; }
    }
}