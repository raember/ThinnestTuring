using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinnestTuring {
    public sealed class DroppingState :State {
        public DroppingState() : base(TuringMachine.STATE_FAIL){}
    }
}
