using System;
using System.Linq;
using NLog;

namespace Iostrade.Core.Tareas
{
    /// <summary>
    /// Lee las tareas y las devuelve en formato "normal"
    /// </summary>
    public class ParserTareas
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Extrae la lista de tareas 
        /// </summary>
        /// <param name="codificado"></param>
        /// <returns></returns>
        public ProgramaTarea ExtraeTarea(string codificado)
        {
            try
            {
                var partes = codificado.Split(';');
                if (partes.Length == 2)
                    return ParseaEntrada(partes[0], partes[1]);
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error parseando {codificado}");
            }
            return null;
        }

        /// <summary>
        /// Parsea la entrada
        /// </summary>
        /// <param name="tarea"></param>
        /// <param name="inicio"></param>
        /// <param name="periocidad"></param>
        /// <returns></returns>
        private ProgramaTarea ParseaEntrada(string inicio, string periocidad)
        {
            var tarInicio = ExtraeInicio(inicio);
            if (tarInicio == DateTime.MaxValue)
                return null;
            return new ProgramaTarea(tarInicio, TimeSpan.Parse(periocidad));
        }

        /// <summary>
        /// Extrae el inicio de la tarea
        /// </summary>
        /// <param name="inicio"></param>
        /// <returns></returns>
        private DateTime ExtraeInicio(string inicio)
        {
            try
            {
                DateTime fechaInicio;
                var partes = inicio.Split('#');
                // Formato AHORA, VIERNES, 2013/02/16, .....
                if (partes.Length == 1)
                {
                    if (partes[0].ToUpper() == "AHORA")
                        return DateTime.Now.AddSeconds(5);
                    fechaInicio = ExtraeDia(partes[0]);
                    if (fechaInicio != DateTime.MaxValue)
                    {
                        if (fechaInicio < DateTime.Now)
                            fechaInicio = fechaInicio.AddDays(7);
                        return fechaInicio;
                    }
                    return DateTime.Parse(inicio).FechaUsuarioFechaServidor();
                }
                // Formato AHORA#09:00:00, LUNES#22:00:00, .....
                fechaInicio = ExtraeDia(partes[0]);
                if (fechaInicio == DateTime.MaxValue)
                    return fechaInicio;
                fechaInicio = fechaInicio.Add(TimeSpan.Parse(partes[1]));
                if (fechaInicio < DateTime.Now)
                    fechaInicio = partes[0].ToUpper() == "AHORA" ? fechaInicio.AddDays(1) : fechaInicio.AddDays(7);
                return fechaInicio;
            }
            catch(Exception ex)
            {
                log.Error(ex, $"Formato de fecha incorrecto en {inicio}");
                return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Extrae el día 
        /// </summary>
        /// <param name="dia"></param>
        /// <returns></returns>
        private DateTime ExtraeDia(string dia)
        {
            dia = dia.ToUpper();
            switch (dia)
            {
                case "AHORA":
                    return DateTime.Today.FechaUsuarioFechaServidor();
                case "DOMINGO":
                    return DateTime.Today.FechaUsuarioFechaServidor().AddDays((7 + (DayOfWeek.Sunday - DateTime.Today.DayOfWeek)) % 7);
                case "LUNES":
                    return DateTime.Today.FechaUsuarioFechaServidor().AddDays((7 + (DayOfWeek.Monday - DateTime.Today.DayOfWeek)) % 7);
                case "MARTES":
                    return DateTime.Today.FechaUsuarioFechaServidor().AddDays((7 + (DayOfWeek.Tuesday - DateTime.Today.DayOfWeek)) % 7);
                case "MIERCOLES":
                    return DateTime.Today.FechaUsuarioFechaServidor().AddDays((7 + (DayOfWeek.Wednesday - DateTime.Today.DayOfWeek)) % 7);
                case "JUEVES":
                    return DateTime.Today.FechaUsuarioFechaServidor().AddDays((7 + (DayOfWeek.Thursday - DateTime.Today.DayOfWeek)) % 7);
                case "VIERNES":
                    return DateTime.Today.FechaUsuarioFechaServidor().AddDays((7 + (DayOfWeek.Friday - DateTime.Today.DayOfWeek)) % 7);
                case "SABADO":
                    return DateTime.Today.FechaUsuarioFechaServidor().AddDays((7 + (DayOfWeek.Saturday - DateTime.Today.DayOfWeek)) % 7);
                default:
                    return DateTime.MaxValue;
            }
        }
    }

    public class ProgramaTarea
    {
        /// <summary>
        /// Initializes a new instance of the ProgramaTarea class.
        /// </summary>
        /// <param name="tarea"></param>
        /// <param name="inicio"></param>
        /// <param name="periocidad"></param>
        public ProgramaTarea(DateTime inicio, TimeSpan periocidad)
        {
            this.inicio = inicio;
            this.periocidad = periocidad;
        }

        private DateTime inicio;
        public DateTime Inicio
        {
            get { return inicio; }
        }
        private TimeSpan periocidad;
        public TimeSpan Periocidad
        {
            get { return periocidad; }
        }

        public IProgramador AProgramador()
        {
            return new ProgramadorPeriodico(Inicio, periocidad);
        }

        public IProgramador AProgramadorSimple()
        {
            return new ProgramadorSimple(Inicio, periocidad);
        }
    }
}
