using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Iostrade.Core.Tareas;

namespace Iostrade.Core.Tareas
{
    /// <summary>
    /// Convierte una funcion en una tarea
    /// </summary>
    public class TareaFuncion : ITarea
    {        
        public TareaFuncion(Func<CancellationToken, Task> funcion)
        {
            this.funcion = funcion;
        }

        Func<CancellationToken, Task> funcion;

        #region ITarea Members

        public Task Ejecutar(CancellationToken token)
        {
            return funcion(token);
        }

        #endregion
    }
}
