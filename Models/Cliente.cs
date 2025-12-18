using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Nombre { get; set; }

        [Required, StringLength(20)]
        public string Cedula { get; set; }

        [Required, EmailAddress]
        public string Correo { get; set; }

        [Phone]
        public string Telefono { get; set; }

        [StringLength(250)]
        public string Direccion { get; set; }

        public ICollection<Pedido> Pedidos { get; set; }
    }
}
