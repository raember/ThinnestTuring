namespace ThinnestTuring
{
    public sealed class AcceptingState : State
    {
        public AcceptingState() : base(TuringMachine.STATE_ACCEPT, "accepting,"){}
    }
}