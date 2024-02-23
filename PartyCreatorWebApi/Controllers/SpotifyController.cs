using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyCreatorWebApi.Repositories.Contracts;
using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyRepository _spotifyRepository;

        public SpotifyController(ISpotifyRepository spotifyRepository)
        {
            _spotifyRepository = spotifyRepository;
        }

        [HttpPost("getAccessToken"), Authorize]
        public async Task<ActionResult<string>> GetAccessToken([FromBody]SpotifyDto code)
        {
            var result = await _spotifyRepository.GetAccessToken(code.Code);
            LoginResponseDto loginResponseDto = new LoginResponseDto
            {
                Token = result
            };
            return Ok(loginResponseDto);
        }

        [HttpPost("addSong"), Authorize]
        public async Task<ActionResult<Song>> AddSong([FromBody]Song song)
        {
            var result = await _spotifyRepository.AddSong(song);
            return Ok(result);
        }

        [HttpGet("getSongsFromEvent/{eventId}"), Authorize]
        public async Task<ActionResult<List<Song>>> GetSongsFromEvent(int eventId)
        {
            var result = await _spotifyRepository.GetSongsFromEvent(eventId);
            return Ok(result);
        }

        [HttpDelete("deleteSong/{id}"), Authorize]
        public async Task<ActionResult<Song>> DeleteSong(int id)
        {
            var result = await _spotifyRepository.DeleteSong(id);
            return Ok(result);
        }
    }
}
