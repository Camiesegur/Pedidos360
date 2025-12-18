using System.ComponentModel.DataAnnotations;

namespace Pedidos360.ViewModels
{
    public class EditarPedidoViewModel
    {
        public int Id { get; set; }

        public string ClienteNombre { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; }
    }
}
