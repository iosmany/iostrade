using System;
using System.Threading.Tasks;

namespace Iostrade.Core.Tareas
{
    /// <summary>
    /// Implementa el interface pero realmente no hace nada
    /// </summary>
    class BloqueoTareaNeutro : IBloqueoTarea
    {
        public Task<bool> BloqueaTareaAsync(string tareaId)
        {
            return Task.FromResult(true);
        }

        public Task DesBloqueaTareaAsync(string tareaId)
        {
            return Task.CompletedTask;
        }
    }
}
