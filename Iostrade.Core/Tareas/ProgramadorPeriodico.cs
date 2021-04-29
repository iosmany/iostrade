using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iostrade.Core.Tareas;

namespace Iostrade.Core.Tareas
{
    /// <summary>
    /// Programador que intenta mantener el ritmo de inicio. Si la fecha de inicio ha pasado y hay repetición intenta seguirla manteniendo el horario
    /// </summary>
    public class ProgramadorPeriodico : IProgramador
    {
        /// <summary>
        /// Initializes a new instance of the ProgramacionSimple class.
        /// </summary>
        public ProgramadorPeriodico()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ProgramacionSimple class.
        /// </summary>
        /// <param name="inicio">Fecha inicial. SI 01/01/0001 se considera la fecha de hoy y se tiene en cuenta la hora</param>
        /// <param name="siguiente"></param>
        public ProgramadorPeriodico(DateTime inicio, TimeSpan siguiente)
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

        private DateTime ultimoInicio;
        public DateTime UltimoInicio
        {
            get
            {
                return ultimoInicio;
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

        /// <summary>
        /// Calcula la siguiente ejecucion si es posible
        /// </summary>
        /// <param name="ahora"></param>
        /// <returns></returns>
        DateTime CalculaSiguienteEjecucion(DateTime ahora)
        {
            if (siguiente == TimeSpan.Zero)
                return ahora;

            DateTime act = ahora;
            while (ahora < DateTime.Now.AddSeconds(-1))
                ahora = ahora + siguiente;

            return ahora;
        }

        #region IProgramador Members

        public virtual DateTime PrimeraEjecucion(DateTime referencia)
        {
            // if la primera ejecución ya ha caducado y hay siguiente el inicio sera cuando deberia ejecutarse
            // segun siguiente
            if (inicio <= DateTime.Now && siguiente != TimeSpan.Zero)
                ultimoInicio = CalculaSiguienteEjecucion(inicio);
            else
                ultimoInicio = Inicio;
            return ultimoInicio;
        }

        public virtual DateTime SiguienteEjecucion(DateTime ultimaEjecucion)
        {
            if (Siguiente == TimeSpan.Zero)
                return DateTime.MinValue;

            ultimoInicio = ultimoInicio + Siguiente;
            return this.ultimoInicio;
        }

        #endregion
    }
}
