using Xunit;
using CuisineApi.Controllers;
using CuisineApi.Data;
using CuisineApi.Models;
using CuisineApi.DTOs;
using CuisineApi.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace CuisineApi.Tests
{
    public class CuisinesControllerTests
    {
        [Fact]
        public async Task Create_GetAll_GetById_Update_Delete_Workflow()
        {
            var ctx = DbContextFactory.CreateInMemoryContext("test_db_workflow");
            var controller = new CuisinesController(ctx);

            // Create
            var createDto = new CuisineCreateUpdateDto { Name = "Italian", Description = "Pasta and pizza" };
            var createActionResult = await controller.Create(createDto);
            var createdAt = Assert.IsType<CreatedAtActionResult>(createActionResult.Result);
            var created = Assert.IsType<CuisineReadDto>(createdAt.Value);
            Assert.Equal("Italian", created.Name);

            // GetAll
            var allResult = await controller.GetAll();
            var okAll = Assert.IsType<OkObjectResult>(allResult.Result);
            var list = Assert.IsType<System.Collections.Generic.List<CuisineReadDto>>(okAll.Value);
            Assert.Contains(list, x => x.Id == created.Id);

            // GetById
            var getResult = await controller.GetById(created.Id);
            var okGet = Assert.IsType<OkObjectResult>(getResult.Result);
            var fetched = Assert.IsType<CuisineReadDto>(okGet.Value);
            Assert.Equal("Italian", fetched.Name);

            // Update
            var updateDto = new CuisineCreateUpdateDto { Name = "Updated Italian", Description = "Updated." };
            var updateResult = await controller.Update(created.Id, updateDto);
            Assert.IsType<NoContentResult>(updateResult);

            var getAfterUpdate = await controller.GetById(created.Id);
            var okGet2 = Assert.IsType<OkObjectResult>(getAfterUpdate.Result);
            var fetched2 = Assert.IsType<CuisineReadDto>(okGet2.Value);
            Assert.Equal("Updated Italian", fetched2.Name);

            // Delete
            var delResult = await controller.Delete(created.Id);
            Assert.IsType<NoContentResult>(delResult);

            var getAfterDelete = await controller.GetById(created.Id);
            Assert.IsType<NotFoundResult>(getAfterDelete.Result);
        }
    }
}
