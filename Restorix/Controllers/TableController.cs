using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restorix.Models;
using Restorix.Repositories.Abstract;

namespace Restorix.Controllers
{
    public class TablesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TablesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var tables = await _unitOfWork.Tables.GetAllAsync();
            return View(tables);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IsOccupied,CurrentOrderId")] Table table)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Tables.AddAsync(table);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(table);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var table = await _unitOfWork.Tables.GetByIdAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            return View(table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsOccupied,CurrentOrderId")] Table table)
        {
            if (id != table.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Tables.Update(table);
                    await _unitOfWork.CompleteAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TableExists(table.Id))
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
            return View(table);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var table = await _unitOfWork.Tables.GetByIdAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            return View(table);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var table = await _unitOfWork.Tables.GetByIdAsync(id);
            _unitOfWork.Tables.Remove(table);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TableExists(int id)
        {
            return await _unitOfWork.Tables.GetByIdAsync(id) != null;
        }
    }

}
