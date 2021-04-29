using System;
using System.Linq;
using System.Text;
using Iostrade.Core.Tareas;

namespace Iostrade.Core.Tareas
{
    /// <summary>
    /// Programador simple
    /// </summary>
    public class ProgramadorSimple : IProgramador
    {
        /// <summary>
        /// Initializes a new instance of the ProgramacionSimple class.
        /// </summary>
        public ProgramadorSimple()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ProgramacionSimple class.
        /// </summary>
        /// <param name="inicio">Fecha inicial. SI 01/01/0001 se considera la fecha de hoy y se tiene en cuenta la hora</param>
        /// <param name="siguiente"></param>
        public ProgramadorSimple(DateTime inicio, TimeSpan siguiente)
        {
            Inicio = inicio;
            this.Siguiente = siguiente;
        }

        DateTime inicio;
        public DateTime Inicio
        {
            get
            {
                return inicio;
            }
            set
            {
                if (inicio != value)
                {
                    if (value.Date == DateTime.MinValue.Date)
                        inicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, value.Hour, value.Minute, value.Second);
                    else
                        inicio = value;
                }
            }
        }

        TimeSpan siguiente;
        public TimeSpan Siguiente
        {
            get
            {
                return siguiente;
            }
            set
            {
                siguiente = value;
            }
        }

        public override string ToString()
        {
            if (Siguiente == TimeSpan.Zero)
                return String.Format("Inicio {0} y no hay repetición", Inicio);

            StringBuilder str = new StringBuilder();
            if (Siguiente.Days > 0)
            {
                str.Append(Siguiente.Days);
                str.Append(" días ");
            }
            if (Siguiente.Hours > 0)
            {
                str.Append(Siguiente.Hours);
                str.Append(" horas ");
            }
            if (Siguiente.Minutes > 0)
            {
                str.Append(Siguiente.Minutes);
                str.Append(" minutos ");
            }
            if (Siguiente.Seconds > 0)
            {
                str.Append(Siguiente.Seconds);
                str.Append(" segundos ");
            }
            if (Siguiente.Milliseconds > 0)
            {
                str.Append(Siguiente.Milliseconds);
                str.Append(" milisegundos ");
            }
            return String.Format("Inicio {0} y se repite cada {1}", Inicio, str.ToString());
        }

        #region IProgramacion Members

        public virtual DateTime PrimeraEjecucion(DateTime referencia)
        {
            // if la primera ejecución ya ha caducado y hay siguiente el inicio sera ahora mas siguiente
            if (inicio <= DateTime.Now && siguiente != TimeSpan.Zero)
                return DateTime.Now + siguiente;
            return Inicio;
        }

        public virtual DateTime SiguienteEjecucion(DateTime ultimaEjecucion)
        {
            if (Siguiente == TimeSpan.Zero)
                return DateTime.MinValue;
            return ultimaEjecucion + Siguiente;
        }

        #endregion
    }
}
