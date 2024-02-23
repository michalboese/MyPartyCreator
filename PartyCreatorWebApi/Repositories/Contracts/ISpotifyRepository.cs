using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Repositories.Contracts
{
    public interface ISpotifyRepository
    {
        Task<string> GetAccessToken(string code);
        Task<Song> AddSong(Song song);
        Task<List<Song>> GetSongsFromEvent(int eventId);
        Task<Song> DeleteSong(int id);
        Task<List<Song>> DeleteAllSongs(int eventId);
    }
}
