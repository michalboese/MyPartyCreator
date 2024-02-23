using Microsoft.EntityFrameworkCore;
using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<InviteList> InviteLists { get; set; }
        public DbSet<GuestList> GuestLists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Survey> Surveys { get; set;}
        public DbSet<SurveyVote> SurveyVotes { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }
        public DbSet<UserContact> UserContacts { get; set; }


    }
}
