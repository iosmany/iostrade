using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Entidades
{
    public class Operacion : DatosTraza
    {
        public long Id { get; set; }
        /// <summary>
        /// Id de operacion en el broker
        /// </summary>
        public string BrokerOpId { get; set; }

        public EstadoOperacion Estado { get; set; }

        public decimal Unidades { get; set; }
        public decimal Entrada { get; set; }
        public decimal Salida { get; set; }

        [Column("Stoploss")]
        public long? StopLossId { get; set; }
        public virtual Operacion StopLoss { get; set; }
        [Column("Takeprofit")]
        public long? TakeProfitId { get; set; }
        public virtual Operacion TakeProfit { get; set; }

        /// <summary>
        /// Importe monetario de perdida (se ejecute o no el stoploss si lo hay)
        /// </summary>
        public decimal Riesgo { get; set; } 
        /// <summary>
        /// % de riesgo de acuerdo al valor de la cuenta
        /// </summary>
        public decimal RiesgoPorcentaje { get; set; }
        public decimal Beneficio { get; set; }
        public decimal PosibleBeneficio { get; set; }

        public TipoOperacion TipoOperacion { get; set; }

        public long MercadoId { get; set; }
        public virtual Bolsa Mercado { get; set; }

        public Clasificacion ClasificacionOperacion { get; set; }

        [ConcurrencyCheck]
        public int Version { get; set; }
    }
}
