using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Entidades
{
    public class Broker : DatosCambio
    {
        public long Id { get; set; }
        public string Nombre { get; set; }

        [ConcurrencyCheck]
        public int Version { get; set; }
    }
}
