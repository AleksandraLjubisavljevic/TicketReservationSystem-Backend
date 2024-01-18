using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface ITokenVersionStore
    {
        int GetTokenVersion(string userId);
        void IncrementTokenVersion(string userId);
    }
}
