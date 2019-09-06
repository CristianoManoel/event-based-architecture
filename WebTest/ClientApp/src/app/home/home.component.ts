import { Component, Inject, OnInit, Input, ViewChildren, QueryList } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ChatRoomComponent } from '../chat-room/chat-room.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  @ViewChildren(ChatRoomComponent)
  rooms: QueryList<ChatRoomComponent>

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

  public subscribeAll() {
    var array = this.rooms.toArray();
    for(var i in array)
      array[i].subscribe();
  }

  public unSubscribeAll() {
    var array = this.rooms.toArray();
    for(var i in array)
      array[i].unSubscribe();
  }

  public clearAll() {
    var array = this.rooms.toArray();
    for(var i in array)
      array[i].clear();
  }
}
