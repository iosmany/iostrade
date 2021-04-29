using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Entidades
{
    public class Usuario : DatosCambio
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string GoogleAccountId { get; set; }
        public string MicrosoftAccountId { get; set; }
        public string AppleAccountId { get; set; }
        public string LinkedingAccountId { get; set; }
        
        public virtual ICollection<UsuarioEstrategia> Estrategias { get; set; } = new HashSet<UsuarioEstrategia>();

        [ConcurrencyCheck]
        public int Version { get; set; }
    }
}
