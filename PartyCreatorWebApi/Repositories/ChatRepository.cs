using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Repositories.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace PartyCreatorWebApi.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly DataContext _dataContext;
        public readonly string key = "b14ca5898a4e4133bbce2ea2315a1916";
        public ChatRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<ChatMessageDto> CreateChatMessage(ChatMessage chatMessage)
        {
            var result = await _dataContext.ChatMessages.AddAsync(chatMessage);
            await _dataContext.SaveChangesAsync();
            var message = new ChatMessageDto
            {
                Id = chatMessage.Id,
                EventId = chatMessage.EventId,
                DateTime = chatMessage.DateTime,
                UserId = chatMessage.UserId,
                Message = chatMessage.Message,
                FirstName = _dataContext.Users.Where(u => u.Id == result.Entity.UserId).Select(f => f.FirstName).FirstOrDefault(),
                LastName = _dataContext.Users.Where(u => u.Id == result.Entity.UserId).Select(f => f.LastName).FirstOrDefault(),
            };
            return message;
        }

        public async Task<List<ChatMessageDto>> GetAllMessages(int eventId)
        {
            var result = await _dataContext.ChatMessages.Where(m => m.EventId == eventId)
                .Select(n=> new ChatMessageDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    FirstName = _dataContext.Users.Where(u => u.Id == n.UserId).Select(f => f.FirstName).FirstOrDefault(),
                    LastName = _dataContext.Users.Where(u => u.Id == n.UserId).Select(f => f.LastName).FirstOrDefault(),
                    EventId = n.EventId,
                    Message = n.Message,
                    DateTime = n.DateTime
                }).ToListAsync();
            return result;
        }

        public async Task<List<ChatMessage>> DeleteAllFromEvent(int eventId)
        {
            var messages = await _dataContext.ChatMessages.Where(x => x.EventId == eventId).ToListAsync();
            foreach (var message in messages)
            {
                _dataContext.ChatMessages.Remove(message);
            }
            await _dataContext.SaveChangesAsync();
            return messages;
        }

        public string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
