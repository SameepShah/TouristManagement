using BranchAPI.Controllers;
using BranchAPI.Models;
using BranchAPI.Services;
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

        [Fact]
        public async Task AddBranchTest_Success() {
            //Arrange
            var branchData = GetBranchForAddSuccess();
            var branchController = new BranchController(_placeServiceMock.Object, _branchServiceMock.Object, _logger.Object);
            
            //Act
            _branchServiceMock.Setup(x => x.AddBranchAsync(branchData)).ReturnsAsync(MockBranchAddResponseSuccess);
            var result = (ObjectResult) await branchController.AddBranch(branchData);

            //Assert
            result.StatusCode.Should().Be(200);

        }

        [Fact]
        public async Task AddBranchTest_Validation()
        {
            //Arrange
            var branchData = GetBranchForAddValidation();
            var branchController = new BranchController(_placeServiceMock.Object, _branchServiceMock.Object, _logger.Object);

            //Act
            _branchServiceMock.Setup(x => x.AddBranchAsync(branchData)).ReturnsAsync(MockBranchAddResponseValidation);
            var result = (ObjectResult)await branchController.AddBranch(branchData);

            //Assert
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task EditBranchTest_Success()
        {
            //Arrange
            var branchUpdateData = GetBranchForUpdateSuccess();
            var branchController = new BranchController(_placeServiceMock.Object, _branchServiceMock.Object, _logger.Object);

            //Act
            _branchServiceMock.Setup(x => x.UpdateBranchAsync(branchUpdateData)).ReturnsAsync(MockBranchUpdateResponseSuccess);
            var result = (ObjectResult)await branchController.EditBranch(branchUpdateData);

            //Assert
            result.StatusCode.Should().Be(200);

        }


        [Fact]
        public async Task EditBranchTest_Validation()
        {
            //Arrange
            var branchUpdateData = GetBranchForUpdateValidation();
            var branchController = new BranchController(_placeServiceMock.Object, _branchServiceMock.Object, _logger.Object);

            //Act
            _branchServiceMock.Setup(x => x.UpdateBranchAsync(branchUpdateData)).ReturnsAsync(MockBranchUpdateResponseValidation);
            var result = (ObjectResult)await branchController.EditBranch(branchUpdateData);

            //Assert
            result.StatusCode.Should().Be(400);

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

        private Branch GetBranchForAddSuccess()
        { 
            return new Branch(){
                BranchCode = "MUM-002",
                BranchName = "MUMBAI BRANCH 2",
                Contact = "12345645",
                Email = "mum02@tourism.com",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Website = "www.tourism.com",
                Places = new List<Place>(){
                    new Place()
                    {
                        PlaceId = "1",
                        PlaceName = "ANDAMAN",
                        TariffAmount = 60000
                    }
                }
            };

        }

        private Branch GetBranchForAddValidation()
        {
            return new Branch()
            {
                BranchCode = "MUM-002",
                BranchName = "MUMBAI BRANCH 2",
                Contact = "12345645",
                Email = "mum02@tourism.com",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Website = "www.tourism.com",
                Places = new List<Place>(){
                    new Place()
                    {
                        PlaceId = "1",
                        PlaceName = "ANDAMAN",
                        TariffAmount = 30000
                    }
                }
            };

        }

        private UpdateBranch GetBranchForUpdateSuccess()
        {
            return new UpdateBranch()
            {
                Id = "009c943a-a09f-4b66-a12b-78390eb23fa0",
                BranchCode = "MUM-002",
                Places = new List<Place>(){
                    new Place()
                    {
                        PlaceId = "1",
                        PlaceName = "ANDAMAN",
                        TariffAmount = 60000
                    }
                }
            };
        }

        private UpdateBranch GetBranchForUpdateValidation()
        {
            return new UpdateBranch()
            {
                Id = "009c943a-a09f-4b66-a12b-78390eb23fa0",
                BranchCode = "MUM-002",
                Places = new List<Place>(){
                    new Place()
                    {
                        PlaceId = "1",
                        PlaceName = "ANDAMAN",
                        TariffAmount = 30000
                    }
                }
            };
        }

        private BranchAddResponse MockBranchAddResponseSuccess()
        {
            return new BranchAddResponse()
            {
                BranchId = "009c943a-a09f-4b66-a12b-78390eb23fa0",
                StatusCode = System.Net.HttpStatusCode.OK,
                ErrorMessage = ""
            };
        }

        private BranchAddResponse MockBranchAddResponseValidation()
        {
            return new BranchAddResponse()
            {
                BranchId = "009c943a-a09f-4b66-a12b-78390eb23fa0",
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                ErrorMessage = ""
            };
        }

        private BranchEditResponse MockBranchUpdateResponseSuccess()
        {
            return new BranchEditResponse()
            {
                BranchId = "009c943a-a09f-4b66-a12b-78390eb23fa0",
                StatusCode = System.Net.HttpStatusCode.OK,
                ErrorMessage = ""
            };
        }

        private BranchEditResponse MockBranchUpdateResponseValidation()
        {
            return new BranchEditResponse()
            {
                BranchId = "009c943a-a09f-4b66-a12b-78390eb23fa0",
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                ErrorMessage = ""
            };
        }

    }
}
