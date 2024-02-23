using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Repositories.Contracts
{
    public interface IShoppingListRepository
    {
        Task<ShoppingListItem> NewShoppingListItem(ShoppingListItem request);
        Task<List<ShoppingListItem>> GetShoppigList(int eventId);
        Task<ShoppingListItem> GetShoppingListItemById(int id);
        Task<ShoppingListItem> UpdateShoppingListItem(ShoppingListItem shoppingListItem);
        Task<bool> RemoveShoppingListItem(int id); 

    }
}
