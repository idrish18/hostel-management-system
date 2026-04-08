'use client';

import { useAuth } from '@/lib/auth-context';
import { UserRole } from '@/types';

export function useAuthorization() {
  const { user } = useAuth();

  const hasRole = (role: UserRole): boolean => {
    return user?.role === role;
  };

  const hasAnyRole = (roles: UserRole[]): boolean => {
    return user ? roles.includes(user.role as UserRole) : false;
  };

  const isAdmin = (): boolean => {
    return user?.role === UserRole.ADMIN;
  };

  const isStudent = (): boolean => {
    return user?.role === UserRole.STUDENT;
  };

  const isWorker = (): boolean => {
    return user?.role === UserRole.WORKER;
  };

  return {
    user,
    hasRole,
    hasAnyRole,
    isAdmin,
    isStudent,
    isWorker,
  };
}
