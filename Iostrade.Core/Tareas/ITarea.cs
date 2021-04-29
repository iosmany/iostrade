using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Iostrade.Core.Tareas
{
    /// <summary>
    /// Tarea que se ejecuta cada cierto tiempo
    /// </summary>
    public interface ITarea
    {
        Task Ejecutar(CancellationToken cancellationToken);
    }
}
