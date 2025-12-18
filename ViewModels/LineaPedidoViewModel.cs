using System.ComponentModel.DataAnnotations;

namespace Pedidos360.ViewModels
{
    public class LineaPedidoViewModel
    {
        [Required]
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }
}
