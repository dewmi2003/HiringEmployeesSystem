export interface SelectOption {
  label: string;
  value: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages?: number;
}

export interface ApiMessage {
  message: string;
}
