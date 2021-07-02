using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _sellerService.FindAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            try
            {
                await _sellerService.InsertAsync(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (System.ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var obj = await _sellerService.FindByIdAsync(id.Value);
                if (obj != null)
                    return View(obj);
            }
            return RedirectToAction(nameof(Error), new { message = "Id not found or not provided (" + id.Value + ")" });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                var obj = await _sellerService.FindByIdAsync(id.Value);
                if (obj != null)
                    return View(obj);
            }
            return RedirectToAction(nameof(Error), new { message = "Id not found or not provided (" + id.Value + ")" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Delete(int id)
        {
            await _sellerService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                var obj = await _sellerService.FindByIdAsync(id.Value);
                if (obj != null)
                {
                    List<Department> departments = await _departmentService.FindAllAsync();
                    SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments };
                    return View(viewModel);
                }
            }
            return RedirectToAction(nameof(Error), new { message = "Id not found or not provided (" + id.Value + ")" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            if (id == seller.Id)
            {
                try
                {
                    await _sellerService.UpdateAsync(seller);
                    return RedirectToAction(nameof(Index));
                } 
                catch (System.ApplicationException e)
                {
                    return RedirectToAction(nameof(Error), new { message = e.Message});
                }
            }
            return RedirectToAction(nameof(Error), new { message = "Ids '" + id + "' and '" + seller.Id + "' mismatch" });
        }
        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
    }
}
