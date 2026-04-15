export interface Endpoint {
  id:           string;   // GUID
  workspaceId:  string;
  token:        string;
  name:         string;
  description?: string;
  isActive:     boolean;
  createdAt:    string;
  webhookUrl:   string;
  requestCount: number;
}

export interface CreateEndpointRequest { name: string; description?: string; }
export interface UpdateEndpointRequest { name: string; description?: string; isActive: boolean; }
