import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { SpotifyService } from 'src/app/services/spotify.service';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { from } from 'rxjs';
import { NgToastService } from 'ng-angular-popup';
import { SongDto } from 'src/app/interfaces/song-dto';

@Component({
  selector: 'app-spotify',
  templateUrl: './spotify.component.html',
  styleUrls: ['./spotify.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
})
export class SpotifyComponent {
  @Input() eventId = '';
  @Input() eventTitle = '';
  searchControl = new FormControl();
  tracks: any[] = [];
  spotiToken: string | null = null;
  playlistTracks: SongDto[] = [];
  song: SongDto = {
    id: 0,
    eventId: 0,
    spotifyId: '',
    title: '',
    artist: '',
    image: '',
  };

  constructor(
    private spotifyService: SpotifyService,
    private toast: NgToastService
  ) {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((query) => this.spotifyService.searchTracks(query))
      )
      .subscribe((tracks) => {
        this.tracks = tracks.tracks.items;
      });
  }

  ngOnInit(): void {
    this.spotiToken = this.spotifyService.getToken();
    this.spotifyService.getPlaylist(parseInt(this.eventId)).subscribe((res) => {
      this.playlistTracks = res;
    });
  }

  addToPlaylist(track: any) {
    const song = {
      spotifyId: track.id,
      title: track.name,
      artist: track.artists[0].name,
      image: track.album.images[0].url,
      eventId: parseInt(this.eventId),
    };

    if (!this.playlistTracks.find((t) => t.spotifyId === song.spotifyId)) {
      this.playlistTracks.push(song as SongDto);
      this.spotifyService.addSong(song as SongDto).subscribe();
    } else {
      this.toast.error({
        detail: 'ERROR',
        summary: 'Utwór znajduje się już na playliście',
        duration: 3000,
      });
    }
  }

  removeFromPlaylist(index: number, songId: number) {
    this.playlistTracks.splice(index, 1);
    this.spotifyService.removeSong(songId).subscribe();
  }

  createPlaylist() {
    this.spotifyService.getUserId().then((res) => {
      const userId = res.id;
      const playlistName = this.eventTitle + ' - PartyCreator';
      const playlistDescription = 'Playlist created by PartyCreator';
      const playlistTracks = this.playlistTracks.map((t) => t.spotifyId);
      this.spotifyService
        .createPlaylist(playlistName, userId, playlistDescription)
        .then((res) => {
          const playlistId = res.id;
          this.spotifyService
            .addTracksToPlaylist(playlistId, playlistTracks)
            .then((res) => {
              this.toast.success({
                detail: 'SUCCESS',
                summary: 'Playlist została utworzona',
                duration: 3000,
              });
            });
        });
    });
  }
}
