import { useMemo, useState } from "react";

export function usePagination(totalItems: number, initialPageSize = 10) {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(initialPageSize);
  const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));

  return useMemo(
    () => ({
      page: Math.min(page, totalPages),
      pageSize,
      totalPages,
      setPage,
      setPageSize,
    }),
    [page, pageSize, totalPages],
  );
}
