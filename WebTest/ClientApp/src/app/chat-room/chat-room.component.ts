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
  private loading: boolean;

  @ViewChild('scrollBottom')
  private myScrollContainer: ElementRef;

  @Input()
  public roomId: string;

  @Input()
  public groupId: string;

  @Input()
  public topic: string;

  @Input()
  public topicRepublish: string;

  @Input()
  public delay: number;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.messages = new Array<ChatMessage>();
  }

  ngOnInit() {
    let obj: ConsumerSettigns = {
      bootstrapServers: "localhost:9092",
      groupId: this.groupId,
      enableAutoCommit: true,
      autoOffSetReset: 0,
      enablePartionEof: true,
      topic: this.topic,
      topicRepublish: this.topicRepublish,
      delay: this.delay
    };

    if (!this.topicRepublish)
      delete obj.topicRepublish;

    if (!this.delay)
      delete obj.delay;

    this.consumerSettigns = JSON.stringify(obj, null, 2);
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  public subscribe() {
    this.loading = true;
    var request = JSON.parse(this.consumerSettigns);

    this.http.post(this.baseUrl + `api/Chat/Subscribe?roomId=${this.roomId}`, request).subscribe(
      result => {
        this.listenChat();
      },
      error => {
        console.error(error);
        this.loading = false;
      }
    );
  }

  public unSubscribe() {
    this.loading = true;
    
    this.http.get(this.baseUrl + `api/Chat/UnSubscribe?roomId=${this.roomId}`).subscribe(
      result => {
        this.hasSubcribe = false;
        this.hubConnection.stop();
        this.loading = false;
      },
      error => {
        console.error(error);
        this.loading = false;
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
            this.loading = false;

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
            this.loading = false;
          });
      })
      .catch(err => {
        this.loading = false;
        console.log('Error while starting connection: ' + err);
      });
  }

  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch (err) { }
  }
}

interface ChatMessage {
  id: string;
  sendDate: Date;
  receiveDate: Date;
  message: string;
  roomId: string;
  hasError: boolean;
}

interface ConsumerSettigns {
  bootstrapServers: string;
  groupId: string;
  enableAutoCommit: boolean;
  autoOffSetReset: number;
  enablePartionEof: boolean;
  topic: string;
  topicRepublish: string;
  delay: number;
}