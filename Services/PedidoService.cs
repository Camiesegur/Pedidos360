using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Models;
using Pedidos360.ViewModels;

namespace Pedidos360.Services
{
    public class PedidoService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PedidoService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> CrearPedidoAsync(CrearPedidoViewModel vm, string userId)
        {
            // Cargar productos involucrados
            var productoIds = vm.Lineas.Select(l => l.ProductoId).ToList();
            var productos = await _context.Productos
                .Where(p => productoIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            decimal subtotal = 0;
            decimal impuestos = 0;

            var pedido = new Pedido
            {
                ClienteId = vm.ClienteId,
                UsuarioId = userId,
                Fecha = DateTime.UtcNow,
                Estado = "Confirmado",
                Detalles = new List<PedidoDetalle>()
            };

            foreach (var linea in vm.Lineas)
            {
                var producto = productos[linea.ProductoId];

                if (producto.Stock < linea.Cantidad)
                    throw new InvalidOperationException($"Sin stock suficiente para {producto.Nombre}");

                decimal precioUnit = producto.Precio;
                decimal impPorc = producto.ImpuestoPorc;
                decimal baseLinea = precioUnit * linea.Cantidad;
                decimal impLinea = baseLinea * (impPorc / 100m);
                decimal totalLinea = baseLinea + impLinea;

                subtotal += baseLinea;
                impuestos += impLinea;

                // Descontar stock
                producto.Stock -= linea.Cantidad;

                pedido.Detalles.Add(new PedidoDetalle
                {
                    ProductoId = producto.Id,
                    Cantidad = linea.Cantidad,
                    PrecioUnit = precioUnit,
                    Descuento = 0,
                    ImpuestoPorc = impPorc,
                    TotalLinea = totalLinea
                });
            }

            pedido.Subtotal = subtotal;
            pedido.Impuestos = impuestos;
            pedido.Total = subtotal + impuestos;

            // Transacción sencilla
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return pedido.Id;
        }
    }
}
