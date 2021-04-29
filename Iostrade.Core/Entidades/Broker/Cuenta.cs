using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Entidades
{
    class Cuenta
    {
        public long Id { get; set; }
        [Column("Usuario")]
        public long UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
