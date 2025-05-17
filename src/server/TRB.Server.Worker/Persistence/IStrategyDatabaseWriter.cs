using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Worker.Models;

namespace TRB.Server.Worker.Persistence
{
    public interface IStrategyDatabaseWriter
    {
        Task<bool> SaveAsync(List<StrategyAnalysisResultEntity> results);
    }
}
