using Microsoft.AspNetCore.Mvc;
using Restorix.Models;
using Restorix.ViewModels;
using Restorix.Repositories.Abstract;

namespace Restorix.Controllers
{
    public class OrderManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderManagementController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int tableId)
        {
            var table = await _unitOfWork.Tables.GetByIdAsync(tableId);
            var products = await _unitOfWork.Products.GetAllAsync();
            var order = await _unitOfWork.Orders.GetActiveOrderForTableAsync(tableId);

            var viewModel = new OrderManagementViewModel
            {
                Table = table,
                Products = products.ToList(),
                Order = order
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(int tableId, int productId)
        {
            var order = await _unitOfWork.Orders.GetActiveOrderForTableAsync(tableId);
            var product = await _unitOfWork.Products.GetByIdAsync(productId);

            if (order == null)
            {
                order = new Order
                {
                    TableId = tableId,
                    CreatedAt = DateTime.UtcNow,
                    TotalAmount = 0,
                    Items = new List<OrderItem>()
                };
                await _unitOfWork.Orders.AddAsync(order);
            }

            var orderItem = new OrderItem
            {
                ProductId = productId,
                Quantity = 1,
                Price = product.Price
            };
            order.Items.Add(orderItem);
            order.TotalAmount += orderItem.Price * orderItem.Quantity;

            var table = await _unitOfWork.Tables.GetByIdAsync(tableId);
            table.IsOccupied = true;

            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index), new { tableId });
        }

        public async Task<IActionResult> DeleteProduct(int tableId, int productId)
        {
            var order = await _unitOfWork.Orders.GetActiveOrderForTableAsync(tableId);
            var orderItem = order.Items.FirstOrDefault(i => i.ProductId == productId);

            if (orderItem != null)
            {
                order.Items.Remove(orderItem);
                order.TotalAmount -= orderItem.Price * orderItem.Quantity;

                if (!order.Items.Any())
                {
                    var table = await _unitOfWork.Tables.GetByIdAsync(tableId);
                    table.IsOccupied = false;
                }

                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index), new { tableId });
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int tableId, int productId)
        {
            var order = await _unitOfWork.Orders.GetActiveOrderForTableAsync(tableId);
            if (order == null)
            {
                order = new Order
                {
                    TableId = tableId,
                    CreatedAt = DateTime.UtcNow,
                    TotalAmount = 0,
                    Items = new List<OrderItem>()
                };
                await _unitOfWork.Orders.AddAsync(order);

                var table = await _unitOfWork.Tables.GetByIdAsync(tableId);
                table.IsOccupied = true;


            }

            var orderItem = order.Items.FirstOrDefault(i => i.ProductId == productId);
            if (orderItem == null)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    return NotFound(); 
                }

                orderItem = new OrderItem
                {
                    ProductId = productId,
                    Quantity = 1, 
                    PaidQuantity = 0, 
                    Price = product.Price
                };
                order.Items.Add(orderItem);
            }
            else
            {
                orderItem.Quantity++;
            }

            order.TotalAmount += orderItem.Price;

            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index), new { tableId });
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int tableId, int productId)
        {
            var order = await _unitOfWork.Orders.GetActiveOrderForTableAsync(tableId);
            if (order == null)
            {
                return NotFound();
            }

            var orderItem = order.Items.FirstOrDefault(i => i.ProductId == productId);
            if (orderItem != null)
            {
                if (orderItem.Quantity > 1 && orderItem.Quantity > orderItem.PaidQuantity)
                {
                    if (orderItem.Quantity - 1 >= orderItem.PaidQuantity)
                    {
                        orderItem.Quantity--;
                        order.TotalAmount -= orderItem.Price;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ödenen miktarın altına inilemez.");
                    }
                }
                else
                {
                    order.Items.Remove(orderItem);
                    order.TotalAmount -= orderItem.Price;
                }

                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index), new { tableId });
        }


        public async Task<IActionResult> Payment(int tableId)
        {
            var table = await _unitOfWork.Tables.GetByIdAsync(tableId);
            var order = await _unitOfWork.Orders.GetActiveOrderForTableAsync(tableId);
            if (order == null || table == null)
            {
                return NotFound();
            }

            var viewModel = new PaymentViewModel
            {
                Table = table,
                Order = order
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model, string selectedItems)
        {
            var tableId = model.Table.Id;
            var paymentMethod = model.PaymentMethod;
            var order = await _unitOfWork.Orders.GetActiveOrderForTableAsync(tableId);
            if (order == null)
            {
                return NotFound();
            }
            if (selectedItems != null)
            {

            
            var selectedItemsDict = selectedItems.Split(',')
                .Select(item => item.Split(':'))
                .ToDictionary(
                    parts => parts[0].Split(',').Select(int.Parse).ToList(),
                    parts => int.Parse(parts[1])
                );

            foreach (var kvp in selectedItemsDict)
            {
                var itemIds = kvp.Key;
                var quantityToPay = kvp.Value;

                var items = order.Items.Where(i => itemIds.Contains(i.Id)).ToList();
                var remainingQuantity = quantityToPay;

                foreach (var item in items)
                {
                    var availableToPay = item.Quantity - item.PaidQuantity;
                    if (remainingQuantity >= availableToPay)
                    {
                        item.PaidQuantity += availableToPay;
                        remainingQuantity -= availableToPay;
                    }
                    else
                    {
                        item.PaidQuantity += remainingQuantity;
                        break;
                    }
                }
            }

            await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(Index), new { tableId });
        }

        public async Task<IActionResult> CloseOrder(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            var table = await _unitOfWork.Tables.GetByIdAsync(order.TableId);
            if (table == null)
            {
                return NotFound();
            }

            bool isFullyPaid = order.Items.All(item => item.Quantity == item.PaidQuantity);
            if (!isFullyPaid)
            {
                TempData["ErrorMessage"] = "Sipariş kapatılamaz. Tüm ürünler henüz ödenmemiş.";
                return RedirectToAction(nameof(Index), new { tableId = order.TableId });
            }

            order.IsCompleted = true;
            order.ClosedAt = DateTime.UtcNow;

            table.IsOccupied = false;
            table.CurrentOrderId = null;

            await _unitOfWork.CompleteAsync();

            TempData["SuccessMessage"] = "Sipariş başarıyla kapatıldı ve masa boşaltıldı.";
            return RedirectToAction("Index", "TableManagement");
        }

        public async Task<IActionResult> TransferOrder(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.Table = await _unitOfWork.Tables.GetByIdAsync(order.TableId);

            var availableTables = await _unitOfWork.Tables.GetAvailableTablesAsync();

            var viewModel = new TransferOrderViewModel
            {
                Order = order,
                AvailableTables = availableTables
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmTransfer(int orderId, int newTableId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            var newTable = await _unitOfWork.Tables.GetByIdAsync(newTableId);

            if (order == null || newTable == null)
            {
                return NotFound();
            }

            var oldTable = await _unitOfWork.Tables.GetByIdAsync(order.TableId);
            if (oldTable == null)
            {
                return NotFound();
            }

            oldTable.IsOccupied = false;
            oldTable.CurrentOrderId = null;

            newTable.IsOccupied = true;
            newTable.CurrentOrderId = order.Id;

            order.TableId = newTableId;

            await _unitOfWork.CompleteAsync();

            TempData["SuccessMessage"] = $"Sipariş başarıyla {oldTable.Name} masasından {newTable.Name} masasına transfer edildi.";

            return RedirectToAction("Index", "TableManagement");
        }


    }




}