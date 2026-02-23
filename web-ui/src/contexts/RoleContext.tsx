import React, { createContext, useContext, useState, ReactNode } from 'react';
import { UserRole, User } from '@/types/inventory';
import { mockUsers } from '@/data/mockData';

interface RoleContextType {
  currentUser: User;
  currentRole: UserRole;
  setCurrentRole: (role: UserRole) => void;
  hasPermission: (requiredRoles: UserRole[]) => boolean;
}

const RoleContext = createContext<RoleContextType | undefined>(undefined);

export function RoleProvider({ children }: { children: ReactNode }) {
  const [currentRole, setCurrentRole] = useState<UserRole>('admin');
  
  const currentUser = mockUsers.find(u => u.role === currentRole) || mockUsers[0];
  
  const hasPermission = (requiredRoles: UserRole[]) => {
    return requiredRoles.includes(currentRole);
  };

  return (
    <RoleContext.Provider value={{ currentUser, currentRole, setCurrentRole, hasPermission }}>
      {children}
    </RoleContext.Provider>
  );
}

export function useRole() {
  const context = useContext(RoleContext);
  if (context === undefined) {
    throw new Error('useRole must be used within a RoleProvider');
  }
  return context;
}
