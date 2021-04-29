using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Entidades
{
    public class Estrategia : DatosCambio
    {
        public long Id { get; set; }
        public Guid ProcesoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    public class EjecucionProgramada : DatosTraza
    {
        public long Id { get; set; }

        [Column("UsuarioEstrategia")]
        public long UsuarioEstrategiaId { get; set; }
        public virtual UsuarioEstrategia UsuarioEstrategia { get; set; }

        public string Programacion { get; set; }
        public DateTimeOffset? Cerrada { get; set; }

        [ConcurrencyCheck]
        public int Version { get; set; }
    }

    public class EjecucionEstrategiaHistorial : DatosTraza
    {
        public long Id { get; set; }
        public long EjecucionProgramadaId { get; set; }
        public long EstrategiaId { get; set; }
        public EstadoEjecucion EstadoEjecucion { get; set; }
    }

    public enum EstadoEjecucion { Ok, Ko }
}
