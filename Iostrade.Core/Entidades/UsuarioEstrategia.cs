using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Entidades
{
    public class UsuarioEstrategia : DatosTraza
    {
        public long Id { get; set; }

        public long UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        public long EstrategiaId { get; set; }
        public virtual Estrategia Estrategia { get; set; }

        public long BolsaId { get; set; }
        public virtual Bolsa Bolsa { get; set; }

        public DateTimeOffset? Activa { get; set; }
    }
}
