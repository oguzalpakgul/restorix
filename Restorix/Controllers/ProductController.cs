using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restorix.Models;
using Restorix.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

namespace Restorix.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Products.GetAllAsync(includeProperties: "Category");
            ViewBag.Products = products;
            return View();
        }
        public async Task<IActionResult> Create()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,CategoryId,ImageFile")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImagePath = "/images/" + uniqueFileName;
                }

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(await _unitOfWork.Categories.GetAllAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _unitOfWork.Categories.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,CategoryId,ImageFile")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    if (product.ImageFile != null && product.ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        if (!string.IsNullOrEmpty(existingProduct.ImagePath))
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(fileStream);
                        }

                        existingProduct.ImagePath = $"/images/{uniqueFileName}";
                    }
                    else
                    {
                        existingProduct.ImagePath = product.ImagePath ?? existingProduct.ImagePath;
                    }

                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryId = product.CategoryId;

                    _unitOfWork.Products.Update(existingProduct);
                    await _unitOfWork.CompleteAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(await _unitOfWork.Categories.GetAllAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }


        private async Task<bool> ProductExists(int id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id) != null;
        }
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
