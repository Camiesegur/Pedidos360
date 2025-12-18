using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    public class PedidoDetalle
    {
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        [Required]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        public decimal PrecioUnit { get; set; }
        public decimal Descuento { get; set; }
        public decimal ImpuestoPorc { get; set; }
        public decimal TotalLinea { get; set; }
    }
}
