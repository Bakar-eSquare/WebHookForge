export interface Workspace {
  id:        string;   // GUID
  name:      string;
  slug:      string;
  ownerId:   string;
  createdAt: string;
  members:   WorkspaceMember[];
}

export interface WorkspaceMember {
  userId:      string;
  displayName: string;
  email:       string;
  role:        'Viewer' | 'Editor' | 'Admin';
}

export interface CreateWorkspaceRequest { name: string; }
export interface UpdateWorkspaceRequest { name: string; }
export interface AddMemberRequest       { email: string; role: string; }
