using AspNetCoreMVCxUnitTest.Web.Controllers;
using AspNetCoreMVCxUnitTest.Web.Database.DTOs;
using AspNetCoreMVCxUnitTest.Web.Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreMVCxUnitTest.Test.Controllers
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<ProductDTO>> _mock;

        private readonly ProductController _productController;

        private List<ProductDTO> products;

        public ProductControllerTest()
        {
            _mock = new Mock<IRepository<ProductDTO>>();

            _productController = new ProductController(_mock.Object);

            products = new List<ProductDTO>() {
            new ProductDTO
            {
                Id = new Guid("37d2310b-bcdf-4220-b19d-047256e640b5"),
                Name = "Pencil",
                Price = 60,
                Stock = 5
            },
            new ProductDTO
            {
                Id = new Guid("e06167ab-13ad-4acf-8deb-0ec1c08d51a2"),
                Name = "Book",
                Price = 50,
                Stock = 10
            },
            new ProductDTO
            {
                Id = new Guid("7a9be9e9-2b63-4fc5-8126-f937d6888a05"),
                Name = "Laptop",
                Price = 100,
                Stock = 2
            }};
        }

        [Fact]
        public async void Index_ActionExecute_ReturnView()
        {
            var result = await _productController.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecute_ReturnProductList()
        {
            _mock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            var result = await _productController.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(viewResult.Model);

            Assert.Equal<int>(3, productList.Count());
        }

        [Fact]
        public async void Detail_ProductIsNull_ReturnIndex()
        {
            ProductDTO product = null;
            _mock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(product);

            var result = await _productController.Details(new Guid());

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData("37d2310b-bcdf-4220-b19d-047256e640b5")]
        public async void Detail_ValidId_ReturnProduct(Guid Id)
        {
            _mock.Setup(x => x.GetByIdAsync(Id)).ReturnsAsync(products.First(y =>
            y.Id == Id));

            var result = await _productController.Details(Id);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<ProductDTO>(viewResult.Model);

            Assert.Equal(Id, resultProduct.Id);
        }

        [Fact]
        public void Create_ActionExecute_ReturnView()
        {
            var result = _productController.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Create_InValidModelState_ReturnView()
        {
            _productController.ModelState.AddModelError("Name", "Required Name");

            var result = await _productController.Create(products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<ProductDTO>(viewResult.Model);
        }

        [Fact]
        public async void Create_ValidModelState_ReturnRedirectToIndex()
        {
            var result = await _productController.Create(products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Create_ValidModelState_CreateMethodExecute()
        {
            ProductDTO product = null;

            _mock.Setup(x => x.CreateAsync(It.IsAny<ProductDTO>())).Callback<ProductDTO>(x =>
                product = x);

            var result = await _productController.Create(products.First());

            _mock.Verify(x => x.CreateAsync(It.IsAny<ProductDTO>()), Times.Once);

            Assert.Equal(products.First().Id, product.Id);
        }

        [Fact]
        public async void Create_InValidModelState_NeverCreateMethodExecute()
        {
            _productController.ModelState.AddModelError("Name", "");

            var result = await _productController.Create(products.First());

            _mock.Verify(x => x.CreateAsync(It.IsAny<ProductDTO>()), Times.Never);
        }

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToAction()
        {
            var result = await _productController.Edit(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Edit_ProductIsNull_ReturnNotFound()
        {
            ProductDTO product = null;

            _mock.Setup(x => x.GetByIdAsync(products.First().Id)).ReturnsAsync(product);

            var result = await _productController.Edit(products.First().Id);

            var redirect = Assert.IsAssignableFrom<NotFoundResult>(result);

            Assert.Equal(404, redirect.StatusCode);
        }

        [Fact]
        public async void Edit_ActionExecute_ReturnView()
        {
            _mock.Setup(x => x.GetByIdAsync(products.First().Id)).ReturnsAsync(products.First);

            var result = await _productController.Edit(products.First().Id);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<ProductDTO>(viewResult.Model);

            Assert.Equal(products.First().Id, resultProduct.Id);
        }

        [Theory]
        [InlineData("37d2310b-bcdf-4220-b19d-047256e640b5")]
        public async void Edit_IdIsNotEqualProduct_ReturnNotFound(Guid Id)
        {
            var result = await _productController.Edit(new Guid(), products.First(x => x.Id == Id));

            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData("37d2310b-bcdf-4220-b19d-047256e640b5")]
        public async void Edit_InValidModel_ReturnView(Guid Id)
        {
            _productController.ModelState.AddModelError("Name", "");

            var result = await _productController.Edit(Id, products.First(x => x.Id == Id));

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultModel = Assert.IsAssignableFrom<ProductDTO>(viewResult.Model);

            Assert.Equal(products.First(), resultModel);
        }

        [Theory]
        [InlineData("37d2310b-bcdf-4220-b19d-047256e640b5")]
        public async void Edit_ValidModelState_ReturnRedirectToAction(Guid Id)
        {
            var result = await _productController.Edit(Id, products.First(x => x.Id == Id));

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData("37d2310b-bcdf-4220-b19d-047256e640b5")]
        public async void Edit_ActionExecute_ReturnRedirectToAction(Guid Id)
        {
            var product = products.First(x => x.Id == Id);

            _mock.Setup(x => x.UpdateAsync(It.IsAny<ProductDTO>())).Callback<ProductDTO>(x =>
                        product = x);


            var result = await _productController.Edit(Id, products.First(x => x.Id == Id));

            _mock.Verify(x => x.UpdateAsync(It.IsAny<ProductDTO>()), Times.Once());
        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _productController.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData("37d2310b-bcdf-4220-b19d-047256e640b5")]
        public async void Delete_ProductIsNull_ReturnNotFound(Guid Id)
        {
            ProductDTO product = null;

            _mock.Setup(x => x.GetByIdAsync(products.First().Id)).ReturnsAsync(product);

            var result = await _productController.Delete(products.First().Id);

            var redirect = Assert.IsAssignableFrom<NotFoundResult>(result);

            Assert.Equal(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData("37d2310b-bcdf-4220-b19d-047256e640b5")]
        public async void Delete_ValidModel_ReturnView(Guid Id)
        {
            _mock.Setup(x => x.GetByIdAsync(Id)).ReturnsAsync(products.First);

            var result = await _productController.Edit(products.First().Id);

            var viewResult = Assert.IsType<ViewResult>(result);

            var productResult = Assert.IsAssignableFrom<ProductDTO>(viewResult.Model);

            Assert.Equal(products.First(), productResult);
        }
    }
}
