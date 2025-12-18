using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Services;
using Pedidos360.ViewModels;

namespace Pedidos360.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private readonly PedidoService _pedidoService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PedidoController(PedidoService pedidoService, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _pedidoService = pedidoService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return View(pedidos);
        }

        // GET: /Pedidos/Create
        public IActionResult Create()
        {
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nombre");
            ViewBag.Productos = new SelectList(_context.Productos.Where(p => p.Activo), "Id", "Nombre");
            var vm = new CrearPedidoViewModel();
            vm.Lineas.Add(new LineaPedidoViewModel()); 
            return View(vm);
        }

        // POST: /Pedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearPedidoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nombre", vm.ClienteId);
                ViewBag.Productos = new SelectList(_context.Productos.Where(p => p.Activo), "Id", "Nombre");
                return View(vm);
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                var pedidoId = await _pedidoService.CrearPedidoAsync(vm, userId);
                return RedirectToAction("Details", new { id = pedidoId });
            }
            catch (Exception ex)
            {
                // log error
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nombre", vm.ClienteId);
                ViewBag.Productos = new SelectList(_context.Productos.Where(p => p.Activo), "Id", "Nombre");
                return View(vm);
            }
        }

        // GET: /Pedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            var vm = new EditarPedidoViewModel
            {
                Id = pedido.Id,
                ClienteNombre = pedido.Cliente?.Nombre,
                Fecha = pedido.Fecha,
                Subtotal = pedido.Subtotal,
                Impuestos = pedido.Impuestos,
                Total = pedido.Total,
                Estado = pedido.Estado
            };

            return View(vm);
        }

        // POST: /Pedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditarPedidoViewModel vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // volvemos a mostrar la vista con los datos actuales
                return View(vm);
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();

            // Solo permitimos cambiar Estado, lo demás se mantiene como auditoría
            pedido.Estado = vm.Estado;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = pedido.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();

            return View(pedido);
        }
    }
}
