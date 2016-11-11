using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinnestTuring {
    public sealed class AcceptingState : State {
        public AcceptingState() : base(TuringMachine.STATE_ACCEPT) {}
    }
}
