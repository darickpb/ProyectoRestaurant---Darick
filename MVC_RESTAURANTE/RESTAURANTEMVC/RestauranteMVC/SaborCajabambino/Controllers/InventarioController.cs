﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborCajabambino.Data;
using SaborCajabambino.Models;

namespace SaborCajabambino.Controllers
{
    public class InventarioController : Controller
    {
        private readonly RestauranteProgramacionIiContext _context;

        public IActionResult inventario_prueba()
        {
            return View();
        }
        public InventarioController(RestauranteProgramacionIiContext context)
        {
            _context = context;
        }

        // GET: Inventario
        public async Task<IActionResult> Index()
        {
            var restauranteProgramacionIiContext = _context.Inventarios.Include(i => i.IdItemCategoriaNavigation);
            return View(await restauranteProgramacionIiContext.ToListAsync());
        }

        // GET: Inventario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .Include(i => i.IdItemCategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdItem == id);
            if (inventario == null)
            {
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_Details", inventario);
            }

            return View(inventario);
        }

        // GET: Inventario/Create
        public IActionResult Create()
        {
            ViewData["IdItemCategoria"] = new SelectList(_context.ItemCategoria, "IdItemCategoria", "Categoria");
            return View();
        }

        // POST: Inventario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdItem,ItemNombre,IdItemCategoria,UnidadMedida,Stock,CostoPorUnidad,FechaDeExpiracion,NivelReorden,CantidadReorden,NecesitaReorden")] Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // 
            ViewData["IdItemCategoria"] = new SelectList(_context.ItemCategoria, "IdItemCategoria", "Categoria", inventario.IdItemCategoria);
            return View(inventario);
        }

        // GET: Inventario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ModelState.Remove("IdItemCategoriaNavigation");
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
        .Include(i => i.IdItemCategoriaNavigation)
        .FirstOrDefaultAsync(m => m.IdItem == id);
            if (inventario == null)
            {
                return NotFound();
            }
            // Modificar para mostrar el nombre de la categoría en lugar del ID
            //esto cambio
            ViewData["IdItemCategoria"] = new SelectList(_context.ItemCategoria, "IdItemCategoria", "Categoria", inventario.IdItemCategoria);
            return View(inventario);
        }

        // POST: Inventario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdItem,ItemNombre,IdItemCategoria,UnidadMedida,Stock,CostoPorUnidad,FechaDeExpiracion,NivelReorden,CantidadReorden,NecesitaReorden")] Inventario inventario)
        {
            ModelState.Remove("IdItemCategoriaNavigation");
            if (id != inventario.IdItem)
            {
                return NotFound();
            }           

            if (ModelState.IsValid)
             {
                 try
                 {   
                     _context.Update(inventario);
                     await _context.SaveChangesAsync();
                 }
                 catch (DbUpdateConcurrencyException)
                 {
                     if (!InventarioExists(inventario.IdItem))
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
            //esto cambio
            ViewData["IdItemCategoria"] = new SelectList(_context.ItemCategoria, "IdItemCategoria", "Categoria", inventario.IdItemCategoria);
            return View(inventario);

        }

        // GET: Inventario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventarios
                .Include(i => i.IdItemCategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdItem == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // POST: Inventario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario != null)
            {
                _context.Inventarios.Remove(inventario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Inventario/Buscar
        [HttpGet]
        public async Task<IActionResult> Buscar(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                var todos = await _context.Inventarios
                    .Include(i => i.IdItemCategoriaNavigation)
                    .ToListAsync();
                return Json(todos);
            }

            var inventarios = await _context.Inventarios
                .Include(i => i.IdItemCategoriaNavigation)
                .Where(i => i.ItemNombre.Contains(searchTerm) ||
                            i.UnidadMedida.Contains(searchTerm) ||
                            (i.IdItemCategoriaNavigation != null && 
                             i.IdItemCategoriaNavigation.Categoria.Contains(searchTerm)))
                .ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(inventarios);
            }

            return View("Index", inventarios);
        }

        private bool InventarioExists(int id)
        {
            return _context.Inventarios.Any(e => e.IdItem == id);
        }
    }
}
