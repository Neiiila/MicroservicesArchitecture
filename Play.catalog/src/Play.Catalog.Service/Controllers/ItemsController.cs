using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> repository;
        // private readonly ReadOnlyCollection<Item> items = [
        //     new Item { Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreatedDate = DateTimeOffset.UtcNow },
        //     new Item { Id = Guid.NewGuid(), Name = "Iron Sword", Price = 20, CreatedDate = DateTimeOffset.UtcNow },
        //     new Item { Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 18, CreatedDate = DateTimeOffset.UtcNow }
        // ];

        public ItemsController(IRepository<Item> repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = (await repository.GetAsync())
                        .Select(item => item.AsDto());

            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await repository.GetAsync(id);
            if (item is null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        // ActionResult to returns a type of results ( NotFound, Okey, etc) or a special type <Type>
        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await repository.CreateAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {

            var existingItem = await repository.GetAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            await repository.UpdateAsync(new Item
            {
                Id = id,
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price,
                CreatedDate = existingItem.CreatedDate
            });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var existingItem = await repository.GetAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            await repository.DeleteAsync(id);
            return NoContent();
        }
    }
}