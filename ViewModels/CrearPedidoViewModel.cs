using System.ComponentModel.DataAnnotations;

namespace Pedidos360.ViewModels
{
    public class CrearPedidoViewModel
    {
        [Required]
        public int ClienteId { get; set; }

        public List<LineaPedidoViewModel> Lineas { get; set; } = new();
    }
}
