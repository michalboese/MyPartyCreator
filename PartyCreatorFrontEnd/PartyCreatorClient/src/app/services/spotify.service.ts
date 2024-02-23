import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { LoginResponseDto } from '../interfaces/login-response-dto';
import { SongDto } from '../interfaces/song-dto';

@Injectable({
  providedIn: 'root',
})
export class SpotifyService {
  private baseUrl = environment.apiUrl + 'Spotify/';

  constructor(private http: HttpClient) {}
  headers = new HttpHeaders({
    'Content-Type': 'application/json',
  });

  getAccessToken(code: string) {
    return this.http.post<LoginResponseDto>(`${this.baseUrl}getAccessToken`, {
      code,
    });
  }

  storeToken(token: string) {
    localStorage.setItem('spotifyToken', token);
  }

  getToken() {
    return localStorage.getItem('spotifyToken');
  }

  async searchTracks(query: string) {
    const result = await fetch(
      'https://api.spotify.com/v1/search?q=' + query + '&type=track&limit=5',
      {
        method: 'GET',
        headers: {
          Authorization: 'Bearer ' + this.getToken(),
        },
      }
    );
    return await result.json();
  }

  async getUserId() {
    const result = await fetch('https://api.spotify.com/v1/me', {
      method: 'GET',
      headers: {
        Authorization: 'Bearer ' + this.getToken(),
      },
    });
    return await result.json();
  }

  async createPlaylist(name: string, userId: string, description: string) {
    const result = await fetch(
      'https://api.spotify.com/v1/users/' + userId + '/playlists',
      {
        method: 'POST',
        headers: {
          Authorization: 'Bearer ' + this.getToken(),
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          name: name,
          description: description,
          public: false,
        }),
      }
    );
    return await result.json();
  }

  async addTracksToPlaylist(playlistId: string, uris: string[]) {
    const result = await fetch(
      'https://api.spotify.com/v1/playlists/' + playlistId + '/tracks',
      {
        method: 'POST',
        headers: {
          Authorization: 'Bearer ' + this.getToken(),
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          uris: uris.map((uri) => 'spotify:track:' + uri),
        }),
      }
    );
    return await result.json();
  }

  addSong(song: SongDto) {
    return this.http.post<SongDto>(`${this.baseUrl}addSong`, song);
  }

  getPlaylist(eventId: number) {
    return this.http.get<SongDto[]>(
      `${this.baseUrl}getSongsFromEvent/${eventId}`
    );
  }

  removeSong(songId: number) {
    return this.http.delete(`${this.baseUrl}deleteSong/${songId}`);
  }
}
