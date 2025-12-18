using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [Required]
        public string UsuarioId { get; set; }  // Identity User Id
        public IdentityUser Usuario { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }

        [StringLength(20)]
        public string Estado { get; set; } = "Confirmado";

        public ICollection<PedidoDetalle> Detalles { get; set; }
    }
}
