using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }

        [Range(0, 100)]
        public decimal ImpuestoPorc { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Url]
        public string ImagenUrl { get; set; }

        public bool Activo { get; set; } = true;
    }
}
