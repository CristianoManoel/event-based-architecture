import { Component, Inject, OnInit, Input, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from "@aspnet/signalr";

@Component({
  selector: 'app-chat-room',
  templateUrl: './chat-room.component.html',
  styleUrls: ['./chat-room.component.css']
})
export class ChatRoomComponent implements OnInit, AfterViewChecked {
  public message: string;
  public consumerSettigns: string;
  public messages: ChatMessage[];
  private hubConnection: signalR.HubConnection;
  private hasSubcribe: boolean;

  @ViewChild('scrollBottom')
  private myScrollContainer: ElementRef;

  @Input()
  public roomId: string;

  @Input()
  public topic: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.messages = new Array<ChatMessage>();
  }

  ngOnInit() {
    this.consumerSettigns = JSON.stringify({
      "BootstrapServers": "localhost:9092",
      "GroupId": "gid-chat",
      "EnableAutoCommit": true,
      "AutoOffSetReset": 0,
      "EnablePartionEof": true,
      "Topic": this.topic
    }, null, 2);
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  public subscribe() {
    var request = JSON.parse(this.consumerSettigns);

    this.http.post(this.baseUrl + `api/Chat/Subscribe?roomId=${this.roomId}`, request).subscribe(
      result => {
        this.listenChat();
      },
      error => {
        console.error(error);
      }
    );
  }

  public unSubscribe() {
    this.http.get(this.baseUrl + `api/Chat/UnSubscribe?roomId=${this.roomId}`).subscribe(
      result => {
        this.hasSubcribe = false;
        this.hubConnection.stop();
      },
      error => {
        console.error(error);
      }
    );
  }

  public clear() {
    this.messages = new Array<ChatMessage>();
  }

  private listenChat() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/signalr/chat', {
        // skipNegotiation: true,
        // transport: signalR.HttpTransportType.WebSockets
      })
      .build();

    this.hubConnection.start()
      .then(() => {
        console.log('Connection started');

        this.hubConnection.invoke("AddToGroup", this.roomId)
          .then(() => {
            this.hasSubcribe = true;
            this.hubConnection.on('chat', (msg: ChatMessage) => {
              console.log(msg);
              this.messages.push(msg);
            });

            this.hubConnection.on('chatError', (msg: ChatMessage) => {
              console.log(msg);
              msg.hasError = true;
              this.messages.push(msg);
            });
          })
          .catch(err => {
            console.log(err);
          });
      })
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch (err) { }
  }
}

interface ChatMessage {
  id: string;
  date: Date;
  message: string;
  roomId: string;
  hasError: boolean;
}

interface ConsumerSettigns {
  bootstrapServers: string;
  groupId: string;
  enableAutoCommit: string;
  autoOffSetReset: string;
  enablePartionEof: string;
  topic: string;
}