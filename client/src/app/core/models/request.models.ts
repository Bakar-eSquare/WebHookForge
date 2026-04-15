export interface IncomingRequest {
  id:          string;   // GUID
  endpointId:  string;
  method:      string;
  path?:       string;
  queryString?: string;
  headers?:    Record<string, string>;
  body?:       string;
  contentType?: string;
  ipAddress?:  string;
  sizeBytes:   number;
  receivedAt:  string;
}

export interface PagedResult<T> {
  items:          T[];
  totalCount:     number;
  page:           number;
  pageSize:       number;
  totalPages:     number;
  hasNextPage:    boolean;
  hasPreviousPage: boolean;
}
