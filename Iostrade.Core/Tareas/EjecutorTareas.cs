using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Iostrade.Core.Tareas
{
    /// <summary>
    /// Programador de tareas
    /// </summary>
    public class EjecutorTareas : IDisposable
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        Timer timer;
        object bloqueo = new object();
        Container container;
        IBloqueoTarea bloqueoTarea;

        public EjecutorTareas(Container container=null, IBloqueoTarea bloqueoTarea=null)
        {
            this.container = container;
            this.bloqueoTarea = bloqueoTarea ?? new BloqueoTareaNeutro();
        }

        /// <summary>
        /// Inicia el servicio
        /// </summary>
        public void IniciarServicio()
        {
            lock (bloqueo)
            {
                if (funcionando)
                    return;
                log.Info("Iniciando programación de tareas");
                funcionando = true;                
            }
            EscaneaTareas();
        }

        /// <summary>
        /// Para el servicio sin abortar las tareas que se estan ejecutando
        /// </summary>
        public void PararServicio()
        {
            PararServicio(false);
        }

        /// <summary>
        /// Para el servicio abortando tareas si se le indica
        /// </summary>
        /// <param name="abortar"></param>
        public void PararServicio(bool abortar)
        {
            lock (bloqueo)
            {
                if (!funcionando)
                    return;
                log.Info("Parando programación de tareas");
                funcionando = false;

                // Paramos timer
                if (timer != null)
                {
                    log.Debug("Liberando timer del programador de tareas");
                    timer.Dispose();
                    timer = null;
                }

                if (!abortar)
                    return;

                log.Info("Abortando programación de tareas");

                foreach (TareaProgramada prg in tareasProgramadas)
                {
                    try
                    {
                        log.Debug($"Abortando {prg.ID}");
                        if (prg.Funcionando)
                            prg.CancellationTokenSource.Cancel();
                    }
                    catch
                    {
                        // Ignoramos errores
                    }
                }               
            }
        }

        public void EliminarTareas()
        {
            lock (bloqueo)
            {
                log.Info("Eliminando tareas del programador");
                if (funcionando)
                    PararServicio(true);
                foreach (TareaProgramada prg in tareasProgramadas)
                {
                    if (prg.Tarea is IDisposable dsp) dsp.Dispose();
                }
                tareasProgramadas.Clear();
            }
        }

        private bool funcionando;
        public bool Funcionando
        {
            get
            {
                return funcionando;
            }
        }

        /// <summary>
        /// Programa una tarea
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tarea"></param>
        /// <param name="tarea"></param>
        /// <param name="tipoTarea"></param>
        /// <param name="procesosIncompatibles">Coleccion de IDs de tareas incompatibles</param>
        void ProgramaTarea(string id, ITarea tarea, Type tipoTarea, IProgramador programacion, IEnumerable<string> procesosIncompatibles = null)
        {
            if ((tarea == null && tipoTarea==null) || programacion == null)
                throw new ArgumentException("La tarea y la programación deben especificarse");

            if (tarea != null && tipoTarea != null)
                throw new ArgumentException("Solo se puede especificar la tarea o su tipo");

            if(tipoTarea !=null && container==null)
                throw new ArgumentException($"El contenedor es nulo y la tarea tipo {tipoTarea.Name} lo necesita");

            if(tipoTarea!=null && container.GetRegistration(tipoTarea) == null)
                throw new ArgumentException($"El contenedor no conoce a {tipoTarea.Name}");

            if(string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"La tarea debe tener un Id unico");

            log.Info($"Programando tarea {id} con el programador {programacion.GetType()}");

            DateTime primeraEjecucion = programacion.PrimeraEjecucion(DateTime.Now);
            if (primeraEjecucion < DateTime.Now.AddSeconds(-5))
            {
                log.Warn($"La tarea {id} ha caducado antes de iniciarse");
                return;
            }
            log.Info($"Servicio periodico registrado {id} en fecha {primeraEjecucion:g}");
            TareaProgramada prog = new TareaProgramada();
            prog.ID = id;
            prog.Programacion = programacion;
            prog.Tarea = tarea;
            prog.TipoTarea = tipoTarea;
            prog.SiguienteInicio = primeraEjecucion;
            if (procesosIncompatibles != null)
                prog.ProcesosIncompatibles.AddRange(procesosIncompatibles);
            lock (bloqueo)
            {
                if (tareasProgramadas.Where(x => x.ID == id).FirstOrDefault() != null)
                {
                    log.Warn($"La tarea {id} está duplicada");
                    return;
                }
                tareasProgramadas.Add(prog);
            }
            EscaneaTareas();
        }

        /// <summary>
        /// Programa una tarea
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tarea"></param>
        /// <param name="programacion"></param>
        /// <param name="procesosIncompatibles">Coleccion de IDs de tareas incompatibles</param>
        public void ProgramaTarea(string id, ITarea tarea, IProgramador programacion, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, tarea, null, programacion, procesosIncompatibles);
        }

        /// <summary>
        /// Programa una tarea
        /// </summary>
        /// <param name="tarea"></param>
        /// <param name="cuando"></param>
        /// <param name="repeticion"></param>
        public void ProgramaTarea(string id, ITarea tarea, DateTime cuando, TimeSpan repeticion, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, tarea, new ProgramadorSimple(cuando, repeticion), procesosIncompatibles);
        }

        /// <summary>
        /// Programa una tarea para que se ejecute una sola vez
        /// </summary>
        /// <param name="tarea"></param>
        /// <param name="cuando"></param>
        public void ProgramaTarea(string id, ITarea tarea, DateTime cuando, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, tarea, cuando, TimeSpan.Zero, procesosIncompatibles);
        }

        /// <summary>
        /// Programa una tarea
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipoTarea"></param>
        /// <param name="programacion"></param>
        /// <param name="procesosIncompatibles">Coleccion de IDs de tareas incompatibles</param>
        public void ProgramaTarea(string id, Type tipoTarea, IProgramador programacion, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, null, tipoTarea, programacion, procesosIncompatibles);
        }

        /// <summary>
        /// Programa una tarea
        /// </summary>
        /// <param name="tipoTarea"></param>
        /// <param name="cuando"></param>
        /// <param name="repeticion"></param>
        public void ProgramaTarea(string id, Type tipoTarea, DateTime cuando, TimeSpan repeticion, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, tipoTarea, new ProgramadorSimple(cuando, repeticion), procesosIncompatibles);
        }

        /// <summary>
        /// Programa una tarea para que se ejecute una sola vez
        /// </summary>
        /// <param name="tipoTarea"></param>
        /// <param name="cuando"></param>
        public void ProgramaTarea(string id, Type tipoTarea, DateTime cuando, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, tipoTarea, cuando, TimeSpan.Zero, procesosIncompatibles);
        }

        /// <summary>
        /// Programa una tarea
        /// </summary>
        /// <param name="id"></param>        
        /// <param name="programacion"></param>
        /// <param name="procesosIncompatibles">Coleccion de IDs de tareas incompatibles</param>
        public void ProgramaTarea<T>(string id, IProgramador programacion, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, null, typeof(T), programacion, procesosIncompatibles);
        }

        /// <summary>
        /// Programa una tarea
        /// </summary>
        /// <param name="tipoTarea"></param>
        /// <param name="cuando"></param>
        /// <param name="repeticion"></param>
        public void ProgramaTarea<T>(string id, DateTime cuando, TimeSpan repeticion, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, typeof(T), new ProgramadorSimple(cuando, repeticion), procesosIncompatibles);
        }

        /// <summary>
        /// Programa una tarea para que se ejecute una sola vez
        /// </summary>
        /// <param name="tipoTarea"></param>
        /// <param name="cuando"></param>
        public void ProgramaTarea<T>(string id, DateTime cuando, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, typeof(T), cuando, TimeSpan.Zero, procesosIncompatibles);
        }

        /// <summary>
        /// Programa una tarea
        /// </summary>
        /// <param name="id"></param>
        /// <param name="funTarea"></param>
        /// <param name="programacion"></param>
        /// <param name="descripcion"></param>
        public void ProgramaTarea(string id, Func<CancellationToken, Task> funTarea, IProgramador programacion, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, new TareaFuncion(funTarea), programacion, procesosIncompatibles);
        }
       
        /// <summary>
        /// Programa una tarea
        /// </summary>
        /// <param name="id"></param>
        /// <param name="funTarea"></param>
        /// <param name="cuando"></param>
        /// <param name="repeticion"></param>
        /// <param name="descripcion"></param>
        public void ProgramaTarea(string id, Func<CancellationToken, Task> funTarea, DateTime cuando, TimeSpan repeticion, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, new TareaFuncion(funTarea), cuando, repeticion, procesosIncompatibles);
        }
       
        /// <summary>
        /// Programa una tarea para que se ejecute una sola vez
        /// </summary>
        /// <param name="id"></param>
        /// <param name="funTarea"></param>
        /// <param name="cuando"></param>
        /// <param name="descripcion"></param>
        public void ProgramaTarea(string id, Func<CancellationToken, Task> funTarea, DateTime cuando, IEnumerable<string> procesosIncompatibles = null)
        {
            ProgramaTarea(id, new TareaFuncion(funTarea), cuando, procesosIncompatibles);
        }
        

        public void CancelaTarea(string id)
        {            
            log.Info($"Cancelando tarea con ID {id}");

            lock (bloqueo)
            {
                var prg = tareasProgramadas.FirstOrDefault(x => x.ID == id);
                if (prg != null)
                {
                    if (prg.Funcionando)
                        prg.CancellationTokenSource.Cancel();
                    DesprogramaTarea(prg);                    
                }
            }
        }

        /// <summary>
        /// Tareas programadas
        /// </summary>
        public IReadOnlyCollection<ITarea> Tareas
        {
            get
            {
                lock (bloqueo)
                {                    
                    return tareasProgramadas.Select(x => x.Tarea).ToList();
                }
            }
        }

        List<TareaProgramada> tareasProgramadas = new List<TareaProgramada>();

        /// <summary>
        /// Escanea las tareas y si hay programa el timer para la proxima en el tiempo
        /// </summary>
        private void EscaneaTareas()
        {
            // ¿Esta parado el servicio?
            if (!funcionando)
                return;

            log.Debug("Escaneando tareas en el programador");
            lock (bloqueo)
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
                DateTime actuales = DateTime.Now;
                var siguiente = new TareaProgramada();    // Para simplificar la busqueda
                siguiente.SiguienteInicio = DateTime.MaxValue;
                foreach (TareaProgramada cls in tareasProgramadas)
                {
                    if (cls.SiguienteInicio <= actuales && !cls.Funcionando)
                    {                        
                        log.Warn($"Llegamos tarde a la tarea {cls.ID} y la lanzamos ahora");
                        Task.Run(()=>EjecutaPrograma(cls));   // ¿Llegamos tarde? arrancamos ahora
                    }
                    else
                    {
                        if (cls.SiguienteInicio < siguiente.SiguienteInicio && !cls.Funcionando)
                            siguiente = cls;
                    }
                }
                if (siguiente.SiguienteInicio < DateTime.MaxValue)      // Si hemos encontrado algo, programamos el timer
                {
                    TimeSpan arranque = siguiente.SiguienteInicio - DateTime.Now;
                    if (arranque < TimeSpan.Zero)
                        arranque = TimeSpan.FromMilliseconds(50);       // Se ejecuta dentro de 50 ms
                    timer = new Timer(new TimerCallback(LanzadoTimer), siguiente, arranque, new TimeSpan(-1));
                }

                if (timer == null)
                    log.Info("No hay mas servicios a programar");
                else
                    log.Info($"Proximo servicio nombre {siguiente.ID} programado para {siguiente.SiguienteInicio}");
            }
        }

        private void EjecutaPrograma(TareaProgramada cls)
        {
            lock (bloqueo)
            {
                
                log.Info($"Lanzando programa {cls.ID}");
                log.Debug("Comprobando si es incompatible con otra tarea en funcionamiento");

                if (cls.ProcesosIncompatibles.Count > 0)
                {
                    bool incompatibles = tareasProgramadas.Any(q => q.Funcionando && cls.ProcesosIncompatibles.Contains(q.ID));
                    if (incompatibles)
                    {
                        log.Warn($"Tarea {cls.ID} ha sido pospuesta");
                        cls.SiguienteInicio = DateTime.Now.AddMinutes(1);    // Posponemos un minuto
                        Task.Run(()=>EscaneaTareas());						 // Buscamos otras posibles tareas a lanzar o simplemente reprogramamos
                        return;
                    }
                }
                
                var cs = cls.CancellationTokenSource;
                var tareaActual= cls.Task = Task.Run(async () =>
                  {
                      var bloqueado = await BlindaAsync(() => bloqueoTarea.BloqueaTareaAsync(cls.ID)).ConfigureAwait(false);
                      try
                      {
                          await HazEjecutaTarea(cls, bloqueado).ConfigureAwait(false);
                      }
                      finally
                      {
                          if (bloqueado)
                              await BlindaAsync(() => bloqueoTarea.DesBloqueaTareaAsync(cls.ID)).ConfigureAwait(false);
                      }
                  }, cls.CancellationTokenSource.Token);
                // Cuando acabe, liberamos memoria
                cls.Task.ContinueWith((t) => {
                    // Liberamos el cancellation token
                    cs.Dispose();
                    // Liberamos recursos
                    tareaActual.Dispose();
                });
            }
        }

        /// <summary>
        /// Hace la ejecucion de la tarea
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="bloqueado"></param>
        /// <returns></returns>
        private async Task HazEjecutaTarea(TareaProgramada cls, bool bloqueado)
        {
            if (!bloqueado)
                log.Info("La tarea no se ha podido bloquear, simulamos su ejecución por coherencia");
            var scope = AseguraTarea(cls);
            try
            {
                OnInicioTarea(cls.ID, cls.Tarea);

                try
                {
                    log.Info($"Ejecutando {cls.ID}. Tarea bloqueada: {bloqueado}");

                    if (cls.Tarea != null)
                    {
                        if (bloqueado)
                            await cls.Tarea.Ejecutar(cls.CancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    else
                        log.Error($"La tarea con ID {cls.ID} no se ha creado correctamente");

                    log.Info($"Fin de ejecucion {cls.ID}");

                    // Avisamos del fin de la tarea
                    FinServicio(cls, null);
                }
                catch (Exception ex)
                {
                    log.Error(ex, $"Error al ejecutar la tarea {cls.ID}");
                    FinServicio(cls, ex);
                }
            }
            finally
            {
                if (scope != null)
                    scope.Dispose();
            }
        }

        Scope AseguraTarea(TareaProgramada cls)
        {
            if (cls.AutoCrear)
            {
                try
                {
                    var scope = AsyncScopedLifestyle.BeginScope(container);
                    cls.Tarea = container.GetInstance(cls.TipoTarea) as ITarea;
                    return scope;
                }
                catch(Exception ex)
                {
                    log.Error(ex, $"Error creando la tarea con ID {cls.ID}");
                }                
            }
            return null;
        }

        /// <summary>
        /// El timer lanza el evento y ejecutamos el servicio programado
        /// </summary>
        /// <param name="estado">Servicio a lanzar</param>
        void LanzadoTimer(object estado)
        {
            EjecutaPrograma((TareaProgramada)estado);
            EscaneaTareas();						// Buscamos otros posibles servicios a lanzar o simplemente reprogramamos
        }


        /// <summary>
        /// Programa finalizado
        /// </summary>
        /// <param name="prg"></param>
        void FinServicio(TareaProgramada prg, Exception ex)
        {
            lock (bloqueo)
            {                
                log.Info($"Finalizando tarea {prg.ID}");

                // Avisamos que ha terminado
                OnFinTarea(prg.ID, prg.Tarea, ex);
                // Si es Auto borramos la tarea en si. No importa hacer dispose porque las tareas se registras como scoped
                if (prg.AutoCrear)
                    prg.Tarea = null;

                prg.Task = null;
                prg.SiguienteInicio = prg.Programacion.SiguienteEjecucion(DateTime.Now);
                if (prg.SiguienteInicio == DateTime.MinValue)						// Si no es periodico, lo borramos
                {                    
                    log.Info($"Tarea {prg.ID} eliminada");
                    DesprogramaTarea(prg);
                }
                else
                {
                    log.Info($"Tarea {prg.ID} reprogramado para {prg.SiguienteInicio.FechaServidorFechaUsuario()}");
                }
                // Liberamos, la proxima llamada creara otro
                prg.CancellationTokenSource = null;
            }
            EscaneaTareas();
        }

        /// <summary>
        /// Elimina una tarea de la lista y si implementa IDisposable lo ejecuta sobre la tarea
        /// </summary>
        /// <param name="prg"></param>
        private void DesprogramaTarea(TareaProgramada prg)
        {
            IDisposable dsp = prg.Tarea as IDisposable;
            if (dsp != null)
                dsp.Dispose();
            tareasProgramadas.Remove(prg);
        } 
        
        /// <summary>
        /// Utilidad para simplificar llamadas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orden"></param>
        /// <returns></returns>
        async Task<T> BlindaAsync<T>(Func<Task<T>> ordenAsync)
        {
            try
            {
                return await ordenAsync();
            }
            catch(Exception ex)
            {
                log.Error(ex, "Error ejecutando la funcion.");
                return default;
            }
        }

        /// <summary>
        /// Utilidad para simplificar llamadas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orden"></param>
        /// <returns></returns>
        async Task BlindaAsync(Func<Task> ordenAsync)
        {
            try
            {
                await ordenAsync();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error ejecutando la orden.");                
            }
        }

        public event InicioTarea InicioTarea;
        public event FinTarea FinTarea;

        #region OnInicioTarea
        /// <summary>
        /// Triggers the InicioTarea event.
        /// </summary>
        void OnInicioTarea(string iDTarea, ITarea tarea)
        {
            if (InicioTarea != null)
                try
                {
                    InicioTarea(this, new InicioTareaEventArgs(iDTarea, tarea));
                }
                catch
                {
                }
        }
        #endregion

        #region OnFinTarea
        /// <summary>
        /// Triggers the FinTarea event.
        /// </summary>
        void OnFinTarea(string iDTarea, ITarea tarea, Exception ex)
        {
            if (FinTarea != null)
                try
                {
                    FinTarea(this, new FinTareaEventArgs(iDTarea, tarea, ex));
                }
                catch
                {
                }
        }
        #endregion        

        #region Programa

        internal class TareaProgramada
        {
            public string ID { get; set; }
            public ITarea Tarea { get; set; }
            public IProgramador Programacion { get; set; }
            public Task Task { get; set; }
            public Type TipoTarea { get; set; }
            public bool AutoCrear => TipoTarea != null;
            CancellationTokenSource cancellationTokenSource;
            public CancellationTokenSource CancellationTokenSource
            {
                get
                {
                    if (cancellationTokenSource == null)
                        cancellationTokenSource = new CancellationTokenSource();
                    return cancellationTokenSource;
                }
                set
                {
                    cancellationTokenSource = value;
                }
            }
            public DateTime SiguienteInicio { get; set; }
            public bool Funcionando
            {
                get
                {
                    if (Task == null)
                        return false;                    
                    return !Task.IsCompleted;
                }
            }
            List<string> procesosIncompatibles = new List<string>();
            public List<string> ProcesosIncompatibles
            {
                get
                {
                    return procesosIncompatibles;
                }
            }
        }

        #endregion

        #region IDisposable Members
        bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;
            EliminarTareas();
            (bloqueoTarea as IDisposable)?.Dispose();
            disposed = true;
        }

        #endregion
    }

    public delegate void InicioTarea(object sender, InicioTareaEventArgs ea);
    public delegate void FinTarea(object sender, FinTareaEventArgs ea);


    public class InicioTareaEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ProgresionTareaEventArgs class.
        /// </summary>
        /// <param name="iDTarea"></param>
        /// <param name="tarea"></param>
        public InicioTareaEventArgs(string iDTarea, ITarea tarea)
        {
            this.iDTarea = iDTarea;
            this.tarea = tarea;
        }

        private string iDTarea;
        public string IDTarea
        {
            get
            {
                return iDTarea;
            }
        }

        private ITarea tarea;
        public ITarea Tarea
        {
            get
            {
                return tarea;
            }
        }
    }

    public class FinTareaEventArgs : InicioTareaEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the FinTareaEventArgs class.
        /// </summary>
        /// <param name="excepcion"></param>
        public FinTareaEventArgs(string iDTarea, ITarea tarea, Exception excepcion) : base(iDTarea, tarea)
        {
            this.excepcion = excepcion;
        }

        private Exception excepcion;
        public Exception Excepcion
        {
            get
            {
                return excepcion;
            }
        }
    }

}

