using System;

namespace Iostrade.Core.Tareas
{
    /// <summary>
    /// Permite ajustar la programación de una tarea
    /// </summary>
    public interface IProgramador
    {
        /// <summary>
        /// Primera ejecución de la tarea, si < a la referencia se ejecuta inmediatamente, 
        /// si DateTime.MinValue no se ejecuta
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        DateTime PrimeraEjecucion(DateTime referencia);
        /// <summary>
        /// Siguiente ejecución de la tarea, si no hay mas DateTime.MinValue
        /// </summary>
        /// <param name="ultimaEjecucion"></param>
        /// <returns></returns>
        DateTime SiguienteEjecucion(DateTime ultimaEjecucion);
    }
}
