using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Tareas
{
    /// <summary>
    ///  Permite bloquear una tarea en el sistema, util si queremos usar una tarea a la vez en todo el cluster
    /// </summary>
    public interface IBloqueoTarea
    {
        Task<bool> BloqueaTareaAsync(string tareaId);
        Task DesBloqueaTareaAsync(string tareaId);
    }
}
