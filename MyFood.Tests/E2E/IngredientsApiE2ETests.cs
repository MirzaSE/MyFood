using System.Net;
using System.Net.Http.Json;
using MyFood.Application.Dtos;

namespace MyFood.Tests.E2E
{
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);
            var response = await Client.GetAsync("/api/v1/ingredients");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ValidIngredient_ReturnsCreated()
        {
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);
            var dto = new IngredientCreateDto
            {
                Name = "TestSalt",
                Unit = "g",
                CaloriesPerUnit = 0,
                Protein = 0,
                Carbs = 0,
                Fat = 0
            };

            var response = await Client.PostAsJsonAsync("/api/v1/ingredients", dto);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ExistingIngredient_ReturnsOk()
        {
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);
            var dto = new IngredientCreateDto { Name = "TestSugar", Unit = "g" };
            var createResponse = await Client.PostAsJsonAsync("/api/v1/ingredients", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<IngredientDto>();

            var response = await Client.GetAsync($"/api/v1/ingredients/{created!.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Update_ExistingIngredient_ReturnsOk()
        {
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);
            var dto = new IngredientCreateDto { Name = "TestPepper", Unit = "g" };
            var createResponse = await Client.PostAsJsonAsync("/api/v1/ingredients", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<IngredientDto>();

            var updateDto = new IngredientUpdateDto { Name = "UpdatedPepper" };
            var response = await Client.PutAsJsonAsync($"/api/v1/ingredients/{created!.Id}", updateDto);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ExistingIngredient_ReturnsNoContent()
        {
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);
            var dto = new IngredientCreateDto { Name = "TestOil", Unit = "ml" };
            var createResponse = await Client.PostAsJsonAsync("/api/v1/ingredients", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<IngredientDto>();

            var response = await Client.DeleteAsync($"/api/v1/ingredients/{created!.Id}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}