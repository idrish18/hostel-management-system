'use client';

import { useEffect, useRef } from 'react';

const TOKEN_EXPIRY_KEY = 'tokenExpiry';
const TOKEN_KEY = 'authToken';

export function useTokenExpiry() {
  const timeoutRef = useRef<NodeJS.Timeout>();

  useEffect(() => {
    const setupTokenExpiry = () => {
      const token = localStorage.getItem(TOKEN_KEY);
      const expiryTime = localStorage.getItem(TOKEN_EXPIRY_KEY);

      if (!token || !expiryTime) return;

      const expiryMs = parseInt(expiryTime);
      const nowMs = Date.now();
      const timeUntilExpiry = expiryMs - nowMs;

      if (timeUntilExpiry <= 0) {
        // Token already expired
        handleTokenExpiration();
        return;
      }

      // Set timeout to refresh 5 minutes before expiry
      const refreshTime = Math.max(timeUntilExpiry - 5 * 60 * 1000, 0);

      timeoutRef.current = setTimeout(() => {
        handleTokenExpiration();
      }, refreshTime);
    };

    setupTokenExpiry();

    return () => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
    };
  }, []);
}

function handleTokenExpiration() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(TOKEN_EXPIRY_KEY);
  localStorage.removeItem('userRole');

  if (typeof window !== 'undefined') {
    window.location.href = '/auth/login?expired=true';
  }
}

export function setTokenExpiry(expiresIn: number) {
  const expiryTime = Date.now() + expiresIn * 1000;
  localStorage.setItem(TOKEN_EXPIRY_KEY, expiryTime.toString());
}

export function getTokenExpiry(): number | null {
  const expiry = localStorage.getItem(TOKEN_EXPIRY_KEY);
  return expiry ? parseInt(expiry) : null;
}

export function isTokenExpired(): boolean {
  const expiry = getTokenExpiry();
  if (!expiry) return true;
  return Date.now() >= expiry;
}
