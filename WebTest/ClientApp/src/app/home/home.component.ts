import { Component, Inject, OnInit, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  public message: string;
  public publisherSettigns: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {

  }

  ngOnInit(): void {
    this.publisherSettigns = JSON.stringify({
      "BootstrapServers": "localhost:9092",
      "Topic": "Chat"
    }, null, 2);
  }

  public send() {
    this.sendMessage(this.message);
  }

  private sendMessage(_message: string) {
    var request = {
      settings: JSON.parse(this.publisherSettigns),
      message: {
        message: _message
      }
    };

    this.http.post<Boolean>(this.baseUrl + `api/Chat/Send`, request).subscribe(
      result => {

      },
      error => {
        console.error(error);
      });
  }
}
