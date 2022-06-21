using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspNetCoreMVCxUnitTest.Web.Database;
using AspNetCoreMVCxUnitTest.Web.Database.DTOs;
using AspNetCoreMVCxUnitTest.Web.Database.Repositories;

namespace AspNetCoreMVCxUnitTest.Web.Controllers
{
    public class ProductController : Controller
    {
        public IRepository<ProductDTO> _repository { get; }

        public ProductController(IRepository<ProductDTO> repository)
        {
            _repository = repository;
        }

        // GET: ProductDTOes
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAllAsync().ConfigureAwait(false));
        }

        // GET: ProductDTOes/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var productDTO = await _repository.GetByIdAsync(id).ConfigureAwait(false);

            if (productDTO == null)
            {
                return NotFound();
            }

            return View(productDTO);
        }

        // GET: ProductDTOes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductDTOes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock")] ProductDTO productDTO)
        {
            if (ModelState.IsValid)
            {
                productDTO.Id = Guid.NewGuid();
                await _repository.CreateAsync(productDTO).ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }
            return View(productDTO);
        }

        // GET: ProductDTOes/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var productDTO = await _repository.GetByIdAsync(id).ConfigureAwait(false);
            if (productDTO == null)
            {
                return NotFound();
            }
            return View(productDTO);
        }

        // POST: ProductDTOes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Price,Stock")] ProductDTO productDTO)
        {
            if (id != productDTO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(productDTO).ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductDTOExistsAsync(productDTO.Id))
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
            return View(productDTO);
        }

        // GET: ProductDTOes/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _repository.GetByIdAsync(id) == null)
            {
                return NotFound();
            }

            var productDTO = await _repository.GetByIdAsync(id).ConfigureAwait(false);

            if (productDTO == null)
            {
                return NotFound();
            }

            return View(productDTO);
        }

        // POST: ProductDTOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (await _repository.GetByIdAsync(id) == null)
            {
                return Problem("Entity set 'WebAppDbContext.Products'  is null.");
            }
            var productDTO = await _repository.GetByIdAsync(id).ConfigureAwait(false);
            if (productDTO != null)
            {
                await _repository.DeleteAsync(productDTO).ConfigureAwait(false);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductDTOExistsAsync(Guid id)
        {
            var product = _repository.GetByIdAsync(id).Result;

            return product != null ? true : false;
        }
    }
}
