import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from './services/chat.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'ai-agent-angular';
  message = '';
  response = '';
  isLoading = false;
  errorMessage = '';

  constructor(private chatService: ChatService) { }

  sendMessage() {
    if (!this.message.trim()) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.response = '';

    this.chatService.sendMessage(this.message).subscribe({
      next: (data) => {
        this.response = data.reply || JSON.stringify(data);
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'An error occurred while calling the API';
        this.isLoading = false;
        console.error('API Error:', error);
      }
    });
  }
}

