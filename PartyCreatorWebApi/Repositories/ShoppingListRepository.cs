using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Repositories.Contracts;

namespace PartyCreatorWebApi.Repositories
{
    public class ShoppingListRepository : IShoppingListRepository
    {
        private readonly DataContext _dataContext;

        public ShoppingListRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<ShoppingListItem>> GetShoppigList(int eventId)
        {
            return await _dataContext.ShoppingListItems.Where(x => x.EventId == eventId).ToListAsync();
        }

        public async Task<ShoppingListItem> GetShoppingListItemById(int id)
        {
            return await _dataContext.ShoppingListItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ShoppingListItem> NewShoppingListItem(ShoppingListItem request)
        {
            var result = await _dataContext.ShoppingListItems.AddAsync(request);
            await _dataContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ShoppingListItem> UpdateShoppingListItem(ShoppingListItem shoppingListItem)
        {
            var result = _dataContext.ShoppingListItems.Update(shoppingListItem);
            await _dataContext.SaveChangesAsync();
            return result.Entity;
        }
        public async Task<bool> RemoveShoppingListItem(int id)
        {
            var shoppingListItem = await _dataContext.ShoppingListItems.FindAsync(id);

            if (shoppingListItem != null)
            {
                _dataContext.ShoppingListItems.Remove(shoppingListItem);
                await _dataContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
