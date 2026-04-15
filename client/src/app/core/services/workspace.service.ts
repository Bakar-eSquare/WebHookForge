import { Injectable }     from '@angular/core';
import { HttpClient }      from '@angular/common/http';
import { API }             from '../constants/api.constants';
import {
  AddMemberRequest, CreateWorkspaceRequest,
  UpdateWorkspaceRequest, Workspace
} from '../models/workspace.models';

@Injectable({ providedIn: 'root' })
export class WorkspaceService {
  constructor(private http: HttpClient) {}

  getAll()                                        { return this.http.get<Workspace[]>(API.workspaces.list()); }
  getById(id: string)                             { return this.http.get<Workspace>(API.workspaces.byId(id)); }
  create(req: CreateWorkspaceRequest)             { return this.http.post<Workspace>(API.workspaces.list(), req); }
  update(id: string, req: UpdateWorkspaceRequest) { return this.http.put<Workspace>(API.workspaces.byId(id), req); }
  delete(id: string)                              { return this.http.delete<void>(API.workspaces.byId(id)); }
  addMember(id: string, req: AddMemberRequest)    { return this.http.post<void>(API.workspaces.members(id), req); }
  removeMember(wsId: string, userId: string)      { return this.http.delete<void>(API.workspaces.member(wsId, userId)); }
}
