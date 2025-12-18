using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        public ICollection<Producto> Productos { get; set; }
    }
}
