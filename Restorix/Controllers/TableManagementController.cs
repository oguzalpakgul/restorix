using Microsoft.AspNetCore.Mvc;
using Restorix.Repositories.Abstract;

namespace Restorix.Controllers
{
    public class TableManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TableManagementController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var tables = await _unitOfWork.Tables.GetAllAsync();
            return View(tables);
        }
    }
}
