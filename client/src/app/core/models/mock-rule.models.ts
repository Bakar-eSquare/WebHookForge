export interface MockRule {
  id:                  string;   // GUID
  endpointId:          string;
  name:                string;
  priority:            number;
  matchMethod?:        string;
  matchPath?:          string;
  matchBodyExpression?: string;
  responseStatus:      number;
  responseBody?:       string;
  responseHeaders?:    Record<string, string>;
  delayMs:             number;
  isActive:            boolean;
  createdAt:           string;
}

export interface SaveMockRuleRequest {
  name:                string;
  priority:            number;
  matchMethod?:        string;
  matchPath?:          string;
  matchBodyExpression?: string;
  responseStatus:      number;
  responseBody?:       string;
  responseHeaders?:    Record<string, string>;
  delayMs:             number;
  isActive?:           boolean;  // only used on update
}

export interface ReorderRulesRequest { ruleIds: string[]; }
