using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Models;

namespace Pedidos360.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        // Estadísticas para el dashboard
        var totalProductos = await _context.Productos.CountAsync();
        var totalClientes = await _context.Clientes.CountAsync();
        var totalPedidos = await _context.Pedidos.CountAsync();

        ViewData["TotalProductos"] = totalProductos;
        ViewData["TotalClientes"] = totalClientes;
        ViewData["TotalPedidos"] = totalPedidos;

        return View();
    }

}
