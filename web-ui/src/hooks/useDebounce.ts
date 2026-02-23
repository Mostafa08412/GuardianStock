/**
 * useDebounce Hook
 * 
 * Debounces a value to prevent excessive updates.
 */

import { useState, useEffect, useCallback, useRef } from 'react';

/**
 * Debounces a value with a specified delay.
 */
export function useDebounce<T>(value: T, delay: number = 300): T {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(timer);
    };
  }, [value, delay]);

  return debouncedValue;
}

/**
 * Returns a debounced version of a callback function.
 */
export function useDebouncedCallback<T extends (...args: any[]) => any>(
  callback: T,
  delay: number = 300
): T {
  const callbackRef = useRef(callback);
  const timeoutRef = useRef<NodeJS.Timeout>();

  // Update callback ref when callback changes
  useEffect(() => {
    callbackRef.current = callback;
  }, [callback]);

  const debouncedCallback = useCallback(
    (...args: Parameters<T>) => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }

      timeoutRef.current = setTimeout(() => {
        callbackRef.current(...args);
      }, delay);
    },
    [delay]
  ) as T;

  // Cleanup timeout on unmount
  useEffect(() => {
    return () => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
    };
  }, []);

  return debouncedCallback;
}

/**
 * Returns a debounced search value with a loading indicator.
 */
export function useDebouncedSearch(initialValue: string = '', delay: number = 300) {
  const [value, setValue] = useState(initialValue);
  const [isSearching, setIsSearching] = useState(false);
  const debouncedValue = useDebounce(value, delay);

  useEffect(() => {
    if (value !== debouncedValue) {
      setIsSearching(true);
    } else {
      setIsSearching(false);
    }
  }, [value, debouncedValue]);

  return {
    value,
    debouncedValue,
    setValue,
    isSearching,
  };
}
