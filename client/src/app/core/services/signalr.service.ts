import { Injectable, OnDestroy } from '@angular/core';
import { Subject }               from 'rxjs';
import * as signalR              from '@microsoft/signalr';
import { environment }           from '../../../environments/environment';
import { AuthService }           from './auth.service';
import { IncomingRequest }       from '../models/request.models';

/**
 * SignalR service — manages a single hub connection for the live request feed.
 * Implements OnDestroy to stop the connection when the service is destroyed,
 * preventing memory leaks.
 *
 * Usage:
 *   signalrService.subscribe(endpointId);
 *   signalrService.requests$.subscribe(req => ...);
 *   // On component destroy: signalrService.unsubscribe(endpointId);
 */
@Injectable({ providedIn: 'root' })
export class SignalrService implements OnDestroy {

  private _hub!: signalR.HubConnection;

  /** Emits every new IncomingRequest pushed from the server. */
  readonly requests$ = new Subject<IncomingRequest>();

  private _connected = false;

  constructor(private auth: AuthService) {
    this.buildConnection();
  }

  /** Start the connection and subscribe to the given endpoint's group. */
  async subscribe(endpointId: string): Promise<void> {
    if (!this._connected) await this.start();
    await this._hub.invoke('Subscribe', endpointId);
  }

  /** Leave the endpoint's group (called when navigating away). */
  async unsubscribe(endpointId: string): Promise<void> {
    if (this._connected) {
      await this._hub.invoke('Unsubscribe', endpointId).catch(() => {});
    }
  }

  ngOnDestroy(): void {
    this._hub.stop().catch(() => {});
  }

  // ── Private ──────────────────────────────────────────────────

  private buildConnection(): void {
    this._hub = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl, {
        // Pass access token via query string (required for SignalR WebSocket upgrade)
        accessTokenFactory: () => this.auth.getAccessToken() ?? ''
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    this._hub.on('NewRequest', (req: IncomingRequest) => this.requests$.next(req));

    this._hub.onreconnected(() => this._connected = true);
    this._hub.onclose(()     => this._connected = false);
  }

  private async start(): Promise<void> {
    try {
      await this._hub.start();
      this._connected = true;
    } catch (err) {
      console.error('SignalR connection failed:', err);
    }
  }
}
