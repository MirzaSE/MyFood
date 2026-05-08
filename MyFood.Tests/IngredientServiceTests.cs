using AutoMapper;
using Moq;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MyFood.Tests
{
    public class IngredientServiceTests
    {
        private readonly Mock<IIngredientRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IngredientService _service;

        public IngredientServiceTests()
        {
            _mockRepo = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new IngredientService(_mockRepo.Object, _mockMapper.Object);
        }

        // --- GetAllAsync Tests ---
        [Fact]
        public async Task GetAllAsync_ReturnsAllIngredients_WhenExist()
        {
            var entities = new List<IngredientEntity> { new IngredientEntity(), new IngredientEntity() }.AsQueryable();
            _mockRepo.Setup(repo => repo.GetAll()).Returns(entities);
            _mockMapper.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto> { new IngredientDto(), new IngredientDto() });
            
            var result = await _service.GetAllAsync(new QueryParameters { Page = 1, PageCount = 10 });
            
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
        {
            var entities = new List<IngredientEntity>().AsQueryable();
            _mockRepo.Setup(repo => repo.GetAll()).Returns(entities);
            _mockMapper.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());
            
            var result = await _service.GetAllAsync(new QueryParameters { Page = 1, PageCount = 10 });
            
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetAllAsync_PaginationWorks()
        {
            var entities = Enumerable.Range(1, 15).Select(i => new IngredientEntity { Id = i }).AsQueryable();
            _mockRepo.Setup(repo => repo.GetAll()).Returns(entities);
            _mockMapper.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                       .Returns((IEnumerable<IngredientEntity> src) => src.Select(e => new IngredientDto { Id = e.Id }));

            var result = await _service.GetAllAsync(new QueryParameters { Page = 2, PageCount = 10 });

            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Data.Count()); // The remaining 5 items on page 2
        }

        // --- GetByIdAsync Tests ---
        [Fact]
        public async Task GetByIdAsync_ReturnsIngredient_WhenFound()
        {
            _mockRepo.Setup(repo => repo.GetSingle(1)).Returns(new IngredientEntity { Id = 1 });
            _mockMapper.Setup(m => m.Map<IngredientDto>(It.IsAny<IngredientEntity>())).Returns(new IngredientDto { Id = 1 });

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(repo => repo.GetSingle(99)).Returns((IngredientEntity?)null!);
            var result = await _service.GetByIdAsync(99);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_HandlesNegativeIds()
        {
            var result = await _service.GetByIdAsync(-1);
            Assert.Null(result);
            _mockRepo.Verify(r => r.GetSingle(It.IsAny<int>()), Times.Never);
        }

        // --- CreateAsync Tests ---
        [Fact]
        public async Task CreateAsync_Success()
        {
            var createDto = new IngredientCreateDto { Name = "Tomato" };
            var entity = new IngredientEntity { Name = "Tomato" };
            
            _mockRepo.Setup(repo => repo.GetAll()).Returns(new List<IngredientEntity>().AsQueryable());
            _mockMapper.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
            _mockRepo.Setup(repo => repo.Add(It.IsAny<IngredientEntity>())); // Add doesn't return anything
            _mockMapper.Setup(m => m.Map<IngredientDto>(It.IsAny<IngredientEntity>())).Returns(new IngredientDto { Id = 1, Name = "Tomato" });

            var result = await _service.CreateAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal("Tomato", result.Name);
        }

        [Fact]
        public async Task CreateAsync_NullNameError()
        {
            var createDto = new IngredientCreateDto { Name = "" };
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_DuplicateError()
        {
            var createDto = new IngredientCreateDto { Name = "Tomato" };
            _mockRepo.Setup(repo => repo.GetAll()).Returns(new List<IngredientEntity> { new IngredientEntity { Name = "Tomato" } }.AsQueryable());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
        }

        // --- UpdateAsync Tests ---
        [Fact]
        public async Task UpdateAsync_Success()
        {
            var entity = new IngredientEntity { Id = 1, Name = "OldName" };
            var updateDto = new IngredientUpdateDto { Name = "NewName" };
            _mockRepo.Setup(r => r.GetSingle(1)).Returns(entity);
            _mockRepo.Setup(r => r.Update(entity)).Returns(entity);
            _mockMapper.Setup(m => m.Map<IngredientDto>(entity)).Returns(new IngredientDto { Id = 1, Name = "NewName" });

            var result = await _service.UpdateAsync(1, updateDto);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAsync_NotFound()
        {
            _mockRepo.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null!);
            var result = await _service.UpdateAsync(99, new IngredientUpdateDto());
            Assert.Null(result);
        }

        // --- DeleteAsync Tests ---
        [Fact]
        public async Task DeleteAsync_Success()
        {
            var entity = new IngredientEntity { Id = 1 };
            _mockRepo.Setup(r => r.GetSingle(1)).Returns(entity);
            
            var result = await _service.DeleteAsync(1);
            
            Assert.True(result);
            _mockRepo.Verify(r => r.Delete(entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NotFound()
        {
            _mockRepo.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null!);
            var result = await _service.DeleteAsync(99);
            Assert.False(result);
        }

        // --- SearchAsync Tests ---
        [Fact]
        public async Task SearchAsync_FindsMatches_CaseInsensitive()
        {
            var entities = new List<IngredientEntity> { new IngredientEntity { Name = "Tomato" }, new IngredientEntity { Name = "Potato" } }.AsQueryable();
            _mockRepo.Setup(r => r.GetAll()).Returns(entities);
            _mockMapper.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                       .Returns(new List<IngredientDto> { new IngredientDto { Name = "Tomato" } });

            var result = await _service.SearchAsync("tom");

            Assert.Single(result);
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmpty()
        {
            var entities = new List<IngredientEntity> { new IngredientEntity { Name = "Tomato" } }.AsQueryable();
            _mockRepo.Setup(r => r.GetAll()).Returns(entities);
            var result = await _service.SearchAsync("xyz");
            Assert.Empty(result);
        }
    }
}