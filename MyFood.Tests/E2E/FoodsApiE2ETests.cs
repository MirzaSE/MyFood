using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class FoodsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetAllFoods_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Act
            var response = await Client.GetAsync("/api/v1/foods");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllFoods_WithAuthentication_ReturnsOkWithFoods()
        {
            // Arrange
            var token = await RegisterAndLogin("fooduser1", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/foods?page=1&pageCount=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"), "Should contain X-Pagination header");
            
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            
            Assert.True(root.TryGetProperty("value", out _));
            Assert.True(root.TryGetProperty("links", out _));
        }

        [Fact]
        public async Task GetAllFoods_WithPagination_ReturnsPaginationMetadata()
        {
            // Arrange
            var token = await RegisterAndLogin("fooduser2", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/foods?page=1&pageCount=5");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("X-Pagination", out var paginationHeaders));
            
            var paginationJson = paginationHeaders?.First();
            Assert.NotNull(paginationJson);
            
            using var jsonDoc = JsonDocument.Parse(paginationJson);
            var pagination = jsonDoc.RootElement;
            Assert.True(pagination.TryGetProperty("currentPage", out var currentPage));
            Assert.True(pagination.TryGetProperty("pageSize", out var pageSize));
            Assert.Equal(1, currentPage.GetInt32());
            Assert.Equal(5, pageSize.GetInt32());
        }

        [Fact]
        public async Task GetSingleFood_WithValidId_ReturnsOk()
        {
            // Arrange
            var token = await RegisterAndLogin("fooduser3", "Test@123");
            SetAuthorizationToken(token);

            // Get all foods first to find a valid ID
            var allFoodsResponse = await Client.GetAsync("/api/v1/foods?page=1&pageCount=1");
            var allFoodsBody = await allFoodsResponse.Content.ReadAsStringAsync();
            using var allFoodsDoc = JsonDocument.Parse(allFoodsBody);
            var foods = allFoodsDoc.RootElement.GetProperty("value");
            
            if (foods.GetArrayLength() > 0)
            {
                var firstFood = foods[0];
                var foodId = firstFood.GetProperty("id").GetInt32();

                // Act
                var response = await Client.GetAsync($"/api/v1/foods/{foodId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var responseBody = await response.Content.ReadAsStringAsync();
                using var foodDoc = JsonDocument.Parse(responseBody);
                var food = foodDoc.RootElement;
                
                Assert.True(food.TryGetProperty("id", out var id));
                Assert.Equal(foodId, id.GetInt32());
            }
        }

        [Fact]
        public async Task GetSingleFood_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin("fooduser4", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/foods/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSingleFood_WithNegativeId_ReturnsBadRequest()
        {
            // Arrange
            var token = await RegisterAndLogin("fooduser5", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/foods/-1");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchFoods_WithValidSearchTerm_ReturnsMatches()
        {
            // Arrange
            var token = await RegisterAndLogin("fooduser6", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/foods/search?name=test&page=1&pageCount=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"), "Should contain X-Pagination header");
            
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            
            Assert.True(root.TryGetProperty("value", out var foods));
            Assert.True(root.TryGetProperty("links", out _));
        }

        [Fact]
        public async Task SearchFoods_WithEmptySearchTerm_ReturnsResults()
        {
            // Arrange
            var token = await RegisterAndLogin("fooduser7", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/foods/search?name=&page=1&pageCount=10");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            // TODO: Chek and validate response if message is ok.
        }

        [Fact]
        public async Task GetAllFoods_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            SetAuthorizationToken("invalid.jwt.token");

            // Act
            var response = await Client.GetAsync("/api/v1/foods");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllFoods_ExpiredToken_ReturnsUnauthorized()
        {
            // Arrange
            // Using an obviously expired/malformed token
            SetAuthorizationToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");

            // Act
            var response = await Client.GetAsync("/api/v1/foods");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task FoodsApi_HandlesConcurrentRequests()
        {
            // Arrange
            var token = await RegisterAndLogin("concurrentuser", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var tasks = new List<Task<HttpResponseMessage>>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(Client.GetAsync("/api/v1/foods?page=1&pageCount=10"));
            }

            var responses = await Task.WhenAll(tasks);

            // Assert
            Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));

            foreach (var response in responses)
            {
                response.Dispose();
            }
        }

        [Fact]
        public async Task GetAllFoods_WithLargePageCount_ReturnsCappedPageCount()
        {
            // Arrange
            var token = await RegisterAndLogin("pageuser", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/foods?page=1&pageCount=1000");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("X-Pagination", out var paginationHeaders));
            
            var paginationJson = paginationHeaders?.First();
            Assert.NotNull(paginationJson);
            using var jsonDoc = JsonDocument.Parse(paginationJson);
            var pagination = jsonDoc.RootElement;
            
            var pageSize = pagination.GetProperty("pageSize").GetInt32();
            // Max page count should be capped at 50 based on QueryParameters
            Assert.True(pageSize <= 50, $"Expected page size <= 50, but got {pageSize}");
        }
    }
}
