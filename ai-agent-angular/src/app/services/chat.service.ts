import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ChatRequest {
  message: string;
}

export interface ChatResponse {  
  reply?: string;
  response?: string;
  [key: string]: any;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private apiUrl = 'https://localhost:7033/AiAgent/chat'; 
  
  constructor(private http: HttpClient) { }

  sendMessage(message: string): Observable<ChatResponse> {
    const request: ChatRequest = { message };
    return this.http.post<ChatResponse>(this.apiUrl, request);
  }
}
