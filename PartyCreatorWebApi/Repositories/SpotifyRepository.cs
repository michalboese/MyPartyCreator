using Newtonsoft.Json;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Repositories.Contracts;

namespace PartyCreatorWebApi.Repositories
{
    public class SpotifyRepository : ISpotifyRepository
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        private readonly DataContext _dataContext;

        public SpotifyRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Song> AddSong(Song song)
        {
            var result = await _dataContext.Songs.AddAsync(song);
            await _dataContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Song> DeleteSong(int id)
        {
            var result = await _dataContext.Songs.FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                _dataContext.Songs.Remove(result);
                await _dataContext.SaveChangesAsync();
            }
            return null;
        }

        public async Task<List<Song>> DeleteAllSongs(int eventId)
        {
            var songs = await _dataContext.Songs.Where(x=>x.EventId == eventId).ToListAsync();
            foreach (var song in songs)
            {
                _dataContext.Songs.Remove(song);
            }
            await _dataContext.SaveChangesAsync();
            return songs;
        }

        public async Task<string> GetAccessToken(string code)
        {
            var client = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", "http://localhost:4200/redirect"),
                new KeyValuePair<string, string>("client_id", "81810883fb3b47bb83ffa68101537c05"),
                new KeyValuePair<string, string>("client_secret", "d5380ae4782748ff92520e95cdd1e7f4")
            });

            var response = await client.PostAsync("https://accounts.spotify.com/api/token", requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<SpotifyRepository>(responseContent);

            return tokenResponse.AccessToken;
        }

        public Task<List<Song>> GetSongsFromEvent(int eventId)
        {
            var result = _dataContext.Songs.Where(x => x.EventId == eventId).ToListAsync();
            return result;
        }
    }
}
