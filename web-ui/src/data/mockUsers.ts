import { UserRole, UserStatus } from '@/types/inventory';

export interface MockAuthUser {
  id: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  status: UserStatus;
  avatar?: string;
  createdAt: Date;
  lastLogin?: Date;
}

// Mock users with login credentials
export const mockAuthUsers: MockAuthUser[] = [
  {
    id: '1',
    email: 'admin@localhost',
    password: 'Admin@123',
    firstName: 'Sarah',
    lastName: 'Chen',
    role: 'admin',
    status: 'active',
    avatar: 'SC',
    createdAt: new Date('2023-06-15'),
    lastLogin: new Date(),
  },
  {
    id: '2',
    email: 'manager@localhost',
    password: 'Manager@123',
    firstName: 'Michael',
    lastName: 'Torres',
    role: 'manager',
    status: 'active',
    avatar: 'MT',
    createdAt: new Date('2023-08-20'),
    lastLogin: new Date(),
  },
  {
    id: '3',
    email: 'staff@localhost',
    password: 'Staff@123',
    firstName: 'Emily',
    lastName: 'Johnson',
    role: 'staff',
    status: 'active',
    avatar: 'EJ',
    createdAt: new Date('2023-10-01'),
    lastLogin: new Date(),
  },
];

export function findUserByCredentials(email: string, password: string): MockAuthUser | null {
  return mockAuthUsers.find(
    (user) => user.email.toLowerCase() === email.toLowerCase() && user.password === password
  ) || null;
}

export function findUserByEmail(email: string): MockAuthUser | null {
  return mockAuthUsers.find(
    (user) => user.email.toLowerCase() === email.toLowerCase()
  ) || null;
}
