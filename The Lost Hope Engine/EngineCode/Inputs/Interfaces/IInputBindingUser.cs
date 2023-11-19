using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHopeEngine.EngineCode.Inputs.Interfaces
{
    public interface IInputBindingUser
    {
        // Should be called when the user is first created
        public abstract void RegisterInputBindingsUser();

        // Should be implemented inside the user.
        // Get bindings from the manager, and setup
        public abstract void SetupInputBindings();
    }
}
