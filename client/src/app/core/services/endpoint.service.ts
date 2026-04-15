import { Injectable }     from '@angular/core';
import { HttpClient }      from '@angular/common/http';
import { API }             from '../constants/api.constants';
import { CreateEndpointRequest, Endpoint, UpdateEndpointRequest } from '../models/endpoint.models';

@Injectable({ providedIn: 'root' })
export class EndpointService {
  constructor(private http: HttpClient) {}

  getByWorkspace(wsId: string)                           { return this.http.get<Endpoint[]>(API.workspaces.endpoints(wsId)); }
  getById(id: string)                                    { return this.http.get<Endpoint>(API.endpoints.byId(id)); }
  create(wsId: string, req: CreateEndpointRequest)       { return this.http.post<Endpoint>(API.workspaces.endpoints(wsId), req); }
  update(id: string, req: UpdateEndpointRequest)         { return this.http.put<Endpoint>(API.endpoints.byId(id), req); }
  delete(id: string)                                     { return this.http.delete<void>(API.endpoints.byId(id)); }
  regenerateToken(id: string)                            { return this.http.post<Endpoint>(API.endpoints.regenerateToken(id), {}); }
}
