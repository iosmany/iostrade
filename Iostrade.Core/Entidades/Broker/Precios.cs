using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Entidades
{
    public class Precio : DatosTraza
    {
        public long Id { get; set; }
        /// <summary>
        /// Identificador de precio obtenido desde el broker
        /// </summary>
        public string BrokerCotizacionId { get; set; }

        public decimal Cotizacion { get; set; }
        
        /// <summary>
        /// Activo referido ETHUSD, BTCUSD, MSFT, TSLA
        /// </summary>
        public long InstrumentoId { get; set; }
        public virtual Instrumento Instrumento { get; set; }

        public decimal Venta { get; set; }
        public decimal Compra { get; set; }

        [NotMapped]
        public decimal Spread => Compra - Venta;

        [ConcurrencyCheck]
        public int Version { get; set; }
    }
}
