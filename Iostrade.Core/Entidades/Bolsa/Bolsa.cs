using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Entidades
{
    public class Bolsa
    {
        public long Id { get; set; }
        public string Nombre { get; set; }

        public TimeSpan Abre { get; set; }
        public TimeSpan Cierra { get; set; }
        public bool AllDay { get; set; }

        [ConcurrencyCheck]
        public int Version { get; set; }

        public bool EstaAbierto()
        {
            var tod = DateTimeOffset.Now.TimeOfDay;
            return tod > Abre && DateTimeOffset.Now.TimeOfDay < Cierra;

        }
    }
}
