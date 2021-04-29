using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Entidades
{
    public class Instrumento : DatosTraza
    {
        public long Id { get; set; }
        /// <summary>Identificacion que da el broker al instrumento</summary>
        public string BrokerInstId { get; set; }
        [Column("Bolsa")]
        public long BolsaId { get; set; }
        public virtual Bolsa Bolsa { get; set; }
        public ClasificacionActivo Clasificacion { get; set; }
        [ConcurrencyCheck]
        public int Version { get; set; }
    }

    public enum ClasificacionActivo { FOREX, CRYPTO, ACTIVOS, FUTUROS, MATERIASPRIMAS }
}
