using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Repositories
{
    public class ReceiptItemRepository : IReceiptItemRepository
    {
        private readonly DataContext _dataContext;

        public ReceiptItemRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<ReceiptItem>> GetReceiptItems(int eventId)
        {
            return await _dataContext.ReceiptItems.Where(x => x.EventId == eventId).ToListAsync();
        }

        public async Task<ReceiptItem> GetReceiptItemById(int id)
        {
            return await _dataContext.ReceiptItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ReceiptItem> AddReceiptItem(ReceiptItem receiptItem)
        {
            var result = await _dataContext.ReceiptItems.AddAsync(receiptItem);
            await _dataContext.SaveChangesAsync();
            return result.Entity;
        }

       /* public async Task<ReceiptItem> UpdateReceiptItem(ReceiptItem receiptItem)
        {
            var result = _dataContext.ReceiptItems.Update(receiptItem);
            await _dataContext.SaveChangesAsync();
            return result.Entity;
        } */

        public async Task<ReceiptItem> RemoveReceiptItem(int id)
        {
            var receiptItem = await _dataContext.ReceiptItems.FindAsync(id);

            if (receiptItem != null)
            {
                _dataContext.ReceiptItems.Remove(receiptItem);
                await _dataContext.SaveChangesAsync();
                return receiptItem;
            }

            return null;
        }
        // trzeba wyjebac
        public async Task<bool> IsUserEventOrganizer(int userId, int eventId)
        {
            var isOrganizer = await _dataContext.Events.AnyAsync(e => e.Id == eventId && e.CreatorId == userId);
            return isOrganizer;
        }

 
    }
}