import { Injectable }     from '@angular/core';
import { HttpClient }      from '@angular/common/http';
import { API }             from '../constants/api.constants';
import { MockRule, ReorderRulesRequest, SaveMockRuleRequest } from '../models/mock-rule.models';

@Injectable({ providedIn: 'root' })
export class MockRuleService {
  constructor(private http: HttpClient) {}

  getByEndpoint(endpointId: string)                           { return this.http.get<MockRule[]>(API.endpoints.rules(endpointId)); }
  getById(id: string)                                         { return this.http.get<MockRule>(API.rules.byId(id)); }
  create(endpointId: string, req: SaveMockRuleRequest)        { return this.http.post<MockRule>(API.endpoints.rules(endpointId), req); }
  update(id: string, req: SaveMockRuleRequest)                { return this.http.put<MockRule>(API.rules.byId(id), req); }
  delete(id: string)                                          { return this.http.delete<void>(API.rules.byId(id)); }
  toggle(id: string)                                          { return this.http.patch<MockRule>(API.rules.toggle(id), {}); }
  reorder(endpointId: string, req: ReorderRulesRequest)       { return this.http.put<void>(API.endpoints.reorder(endpointId), req); }
}
