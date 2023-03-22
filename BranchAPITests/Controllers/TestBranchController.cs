using BranchAPI.Controllers;
using BranchAPI.Models;
using BranchAPI.Services.Interfaces;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace BranchAPITests.Controllers
{
    public class TestBranchController
    {
        private readonly Mock<IBranchService> _branchServiceMock;
        private readonly Mock<IPlaceService> _placeServiceMock;
        private readonly Mock<ILogger<BranchController>> _logger;
        public TestBranchController()
        {
            _branchServiceMock = new Mock<IBranchService>();
            _placeServiceMock = new Mock<IPlaceService>(); 
            _logger = new Mock<ILogger<BranchController>>();
        }

        [Fact]
        public async Task GetAllBranchesTest_Success()
        {
            //Arrange
            _branchServiceMock.Setup(x => x.GetAllAsync("SELECT * FROM c")).ReturnsAsync(GetBranchesData());
            var branchController = new BranchController(_placeServiceMock.Object, _branchServiceMock.Object, _logger.Object);

            //Act
            var result = (ObjectResult)await branchController.GetAllBranches();

            //Assert
            result.StatusCode.Should().Be(200);
        }

        private List<Branch> GetBranchesData()
        {
            return new List<Branch>() { new Branch() {
                id = "9f6eadbc-ff42-40cf-bfe1-6cfcecae8459",
                BranchCode = "MUM-001",
                BranchName = "MUMBAI BRANCH 1",
                Contact = "12345645",
                Email= "mum01@tourism.com",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Website = "www.tourism.com",
                Places = new List<Place>(){
                    new Place()
                    {
                        PlaceId = "1",
                        PlaceName = "ANDAMAN",
                        TariffAmount = 50000
                    }
                }
            } };
        }
    }
}
