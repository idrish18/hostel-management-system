'use client';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@/lib/api-client';

export function useApi<T>(queryKey: string[], endpoint: string) {
  return useQuery({
    queryKey,
    queryFn: async () => {
      const response = await apiClient.get<T>(endpoint);
      if (!response.success) {
        throw new Error(response.error?.message || 'Failed to fetch data');
      }
      return response.data;
    },
  });
}

export function useApiMutation<T, Vars>(
  endpoint: string,
  method: 'post' | 'put' | 'patch' | 'delete' = 'post',
  queryKeyToInvalidate?: string[]
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (variables: Vars) => {
      let response;

      switch (method) {
        case 'post':
          response = await apiClient.post<T>(endpoint, variables);
          break;
        case 'put':
          response = await apiClient.put<T>(endpoint, variables);
          break;
        case 'patch':
          response = await apiClient.patch<T>(endpoint, variables);
          break;
        case 'delete':
          response = await apiClient.delete<T>(endpoint);
          break;
      }

      if (!response.success) {
        throw new Error(response.error?.message || 'An error occurred');
      }

      return response.data;
    },
    onSuccess: () => {
      if (queryKeyToInvalidate) {
        queryKeyToInvalidate.forEach((key) => {
          queryClient.invalidateQueries({ queryKey: [key] });
        });
      }
    },
  });
}
