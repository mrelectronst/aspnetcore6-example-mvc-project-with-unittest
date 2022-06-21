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
    }
}
