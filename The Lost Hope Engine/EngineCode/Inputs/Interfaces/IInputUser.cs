using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHopeEngine.EngineCode.Inputs.Interfaces
{
    public interface IInputUser
    {
        public abstract void UserInputsUpdated(List<InputBinding> bindings);
        public abstract List<string> RegisterUser();
    }
}
