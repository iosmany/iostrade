using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Entidades
{
    public abstract class DatosCambio
    {
        [MaxLength(20)]
        public string CreadoPor { get; set; }
        public DateTimeOffset Creado { get; set; }
        [MaxLength(20)]
        public string ModificadoPor { get; set; }
        public DateTimeOffset Modificado { get; set; }
    }

    public abstract class DatosTraza
    {
        [MaxLength(20)]
        public string CreadoPor { get; set; }
        public DateTimeOffset Creado { get; set; }
        [MaxLength(20)]
        public string ModificadoPor { get; set; }
        public DateTimeOffset Modificado { get; set; }

        [Column("Broker")]
        public long BrokerId { get; set; }
        public virtual Broker Broker { get; set; }
    }
}
