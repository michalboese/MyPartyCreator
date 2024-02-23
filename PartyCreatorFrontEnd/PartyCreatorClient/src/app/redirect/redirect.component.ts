import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SpotifyService } from '../services/spotify.service';

@Component({
  selector: 'app-redirect',
  templateUrl: './redirect.component.html',
  styleUrls: ['./redirect.component.css'],
  standalone: true,
  imports: [],
})
export class RedirectComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private spotify: SpotifyService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const code = params['code'];
      this.spotify.getAccessToken(code).subscribe((response) => {
        this.spotify.storeToken(response.token);
        this.router.navigate(['wydarzenia']);
      });
    });
  }
}
