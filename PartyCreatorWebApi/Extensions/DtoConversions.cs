using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Repositories.Contracts;
using System.Runtime.CompilerServices;

namespace PartyCreatorWebApi.Extensions
{
    public class DtoConversions
    {

        public static UserDto UserToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Birthday = user.Birthday,
                Description = user.Description,
                Image = user.Image
            };
        }

        public static List<UserContactDto> UserToUserContactDto(List<User> users)
        {
            return (from user in users select new UserContactDto
            {
                UserId = user.Id,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email
            }).ToList();
        }

        public static EventUserDto EventToDto(Event _event, User _user)
        {
           
            return new EventUserDto
            {
                Id = _event.Id,
                CreatorId = _event.CreatorId,
                CreatorFirstName = _user.FirstName,
                CreatorLastName = _user.LastName,
                Title = _event.Title,
                Description = _event.Description,
                DateTime = _event.DateTime,
                City = _event.City,
                Address = _event.Address,
                Country = _event.Country,
                Color = _event.Color,
                PlaylistTitle = _event.PlaylistTitle,
                ShoppingListTitle = _event.ShoppingListTitle,
                ReceiptTitle = _event.ReceiptTitle
            };
        }

        public static SurveyVoteDto SurveyVoteToDto(SurveyVote _surveyVote, User _user)
        {
            return new SurveyVoteDto
            {
                SurveyId = _surveyVote.SurveyId,
                ChoiceId = _surveyVote.ChoiceId,
                Id = _surveyVote.Id,
                UserId = _surveyVote.UserId,
                FirstName = _user.FirstName,
                LastName = _user.LastName
            };
        }
    }
}
