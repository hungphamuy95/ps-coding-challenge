using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services.Interfaces
{
    public interface IStateService
    {
        Task<StateResponseModel> GetState(string playerId);
    }
}
