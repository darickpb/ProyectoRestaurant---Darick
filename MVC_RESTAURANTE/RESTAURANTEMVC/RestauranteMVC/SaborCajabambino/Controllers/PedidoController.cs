using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborCajabambino.Data;
using SaborCajabambino.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration; // Necesario para la conexión

namespace SaborCajabambino.Controllers
{
    public class PedidoController : Controller
    {
        private readonly RestauranteProgramacionIiContext _context;
        private readonly IConfiguration _configuration; // Para leer appsettings.json

        // Constructor modificado para recibir la configuración
        public PedidoController(RestauranteProgramacionIiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Pedido
        public async Task<IActionResult> Index()
        {
            var restauranteProgramacionIiContext = _context.Pedidos.Include(p => p.IdClienteNavigation).Include(p => p.IdEmpleadoNavigation).Include(p => p.IdMesaNavigation);
            return View(await restauranteProgramacionIiContext.ToListAsync());
        }

        // GET: Pedido/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.IdClienteNavigation)
                .Include(p => p.IdEmpleadoNavigation)
                .Include(p => p.IdMesaNavigation)
                .FirstOrDefaultAsync(m => m.IdPedido == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Pedido/Create
        public IActionResult Create()
        {
            var ultimoCliente = _context.Clientes
                .OrderByDescending(c => c.IdCliente)
                .FirstOrDefault();

            int? clienteSeleccionado = ultimoCliente?.IdCliente;

            ViewData["IdCliente"] = new SelectList(
                _context.Clientes.Select(c => new {
                    c.IdCliente,
                    NombreCompleto = c.Nombres + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno
                }),
                "IdCliente",
                "NombreCompleto",
                clienteSeleccionado
            );
            ViewData["IdEmpleado"] = new SelectList(
                _context.Empleados
                    .Where(e => e.Rol.ToLower() == "mesero")
                    .Select(e => new {
                        e.IdEmpleado,
                        e.NombreCompleto
                    }),
                "IdEmpleado",
                "NombreCompleto"
            );
            ViewData["IdMesa"] = new SelectList(_context.Mesas, "IdMesa", "IdMesa");
            return View();
        }

        // POST: Pedido/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPedido,Fecha,Hora,Estado,IdMesa,IdCliente,IdEmpleado,TipoPedido,DireccionEntrega,Comentarios")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCliente"] = new SelectList(
                _context.Clientes.Select(c => new {
                    c.IdCliente,
                    NombreCompleto = c.Nombres + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno
                }),
                "IdCliente",
                "NombreCompleto",
                pedido.IdCliente
            );
            ViewData["IdEmpleado"] = new SelectList(
                _context.Empleados
                    .Where(e => e.Rol.ToLower() == "mesero")
                    .Select(e => new {
                        e.IdEmpleado,
                        e.NombreCompleto
                    }),
                "IdEmpleado",
                "NombreCompleto",
                pedido.IdEmpleado
            );
            ViewData["IdMesa"] = new SelectList(_context.Mesas, "IdMesa", "IdMesa", pedido.IdMesa);
            return View(pedido);
        }

        // GET: Pedido/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", pedido.IdCliente);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", pedido.IdEmpleado);
            ViewData["IdMesa"] = new SelectList(_context.Mesas, "IdMesa", "IdMesa", pedido.IdMesa);
            return View(pedido);
        }

        // POST: Pedido/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPedido,Fecha,Hora,Estado,IdMesa,IdCliente,IdEmpleado,TipoPedido,DireccionEntrega,Comentarios")] Pedido pedido)
        {
            if (id != pedido.IdPedido)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.IdPedido))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", pedido.IdCliente);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", pedido.IdEmpleado);
            ViewData["IdMesa"] = new SelectList(_context.Mesas, "IdMesa", "IdMesa", pedido.IdMesa);
            return View(pedido);
        }

        // GET: Pedido/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.IdClienteNavigation)
                .Include(p => p.IdEmpleadoNavigation)
                .Include(p => p.IdMesaNavigation)
                .FirstOrDefaultAsync(m => m.IdPedido == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedido/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.IdPedido == id);
        }

        public async Task<IActionResult> Margen()
        {
            var sql = @"
            WITH ProductoCosto AS (
                SELECT
                    p.Id_Producto AS productoid,
                    p.EsPreparado, 
                    ISNULL(
                        SUM(CASE
                            WHEN p.EsPreparado = 1 THEN ISNULL(inv_ing.CostoPorUnidad, 0) * ISNULL(pg_ing.Cantidad, 0)
                            ELSE 0 
                        END),
                        0
                    ) AS CostoCalculadoIngredientes,
                    ISNULL(
                        MAX(CASE
                            WHEN p.EsPreparado = 0 THEN ISNULL(inv_direct.CostoPorUnidad, 0)
                            ELSE 0 
                        END),
                        0
                    ) AS CostoDirectoProducto
                FROM [GENERAL].[Producto] p
                LEFT JOIN [GENERAL].[ProductoIngrediente] pg_ing ON p.Id_Producto = pg_ing.Id_Producto
                LEFT JOIN [INVENTARIO].[Inventario] inv_ing ON pg_ing.Id_Item = inv_ing.Id_Item
                LEFT JOIN [INVENTARIO].[Inventario] inv_direct ON p.Nombre = inv_direct.ItemNombre
                GROUP BY p.Id_Producto, p.EsPreparado
            )
            SELECT
                (c.Nombres + ' ' + c.ApellidoPaterno + ' ' + c.ApellidoMaterno) AS Cliente,
                p.Nombre,
                e.NombreCompleto AS Empleado,
                m.Id_Mesa AS Mesa,
                (p.Precio * dp.Cantidad) AS PrecioVentaTotal,
                dp.Cantidad * (
                    CASE
                        WHEN pc.EsPreparado = 1 THEN pc.CostoCalculadoIngredientes
                        WHEN pc.EsPreparado = 0 THEN pc.CostoDirectoProducto
                        ELSE 0
                    END
                ) AS CostoTotal,
                (p.Precio * dp.Cantidad) - (
                    dp.Cantidad * (
                        CASE
                            WHEN pc.EsPreparado = 1 THEN pc.CostoCalculadoIngredientes
                            WHEN pc.EsPreparado = 0 THEN pc.CostoDirectoProducto
                            ELSE 0
                        END
                    )
                ) AS Margen
            FROM [TRANSACCION].[DetallePedido] dp
            INNER JOIN [GENERAL].[Producto] p ON dp.Id_Producto = p.Id_Producto
            LEFT JOIN [ProductoCosto] pc ON dp.Id_Producto = pc.productoid
            INNER JOIN [GENERAL].[Categoria] cat ON p.Id_Categoria = cat.Id_Categoria
            INNER JOIN [TRANSACCION].[Pedido] pe ON dp.Id_Pedido = pe.Id_Pedido
            INNER JOIN [CLIENTE].[Cliente] c ON c.Id_Cliente = pe.Id_Cliente
            INNER JOIN [PERSONAL].[Empleado] e ON e.Id_Empleado = pe.Id_Empleado
            INNER JOIN [GENERAL].[Mesa] m ON m.Id_Mesa = pe.Id_Mesa
            ";

            // CORRECCIÓN: Usar la cadena de conexión desde appsettings.json
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var lista = (await connection.QueryAsync<PedidoMargenViewModel>(sql)).ToList();
            return View(lista);
        }
    }
}