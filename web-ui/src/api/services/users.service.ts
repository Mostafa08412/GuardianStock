import { API_CONFIG } from '../config';
import { apiClient } from '../client';
import { mockUsers } from '@/mocks';
import type {
  User,
  PaginatedResponse,
  GetUsersParams,
  CreateUserRequest,
  UpdateUserRequest,
  UserListItemDto,
  UserDetailsDto,
  CreateUserCommand,
  UpdateUserCommand
} from '../types';

const delay = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));

/**
 * Maps UserListItemDto to User domain object
 */
function mapListItemToUser(dto: UserListItemDto): User {
  return {
    id: dto.id,
    name: `${dto.firstName} ${dto.lastName}`,
    email: dto.email,
    role: (dto.role?.toLowerCase() as any) || 'staff',
    status: dto.isActive ? 'active' : 'inactive',
    avatar: dto.firstName[0] + (dto.lastName[0] || ''),
    createdAt: new Date(), // List item doesn't have createdAt, use fallback or better handle in UI
    lastLogin: dto.lastLoginDate ? new Date(dto.lastLoginDate) : undefined
  };
}

/**
 * Maps UserDetailsDto to User domain object
 */
function mapDetailsToUser(dto: UserDetailsDto): User {
  return {
    id: dto.id,
    name: `${dto.firstName} ${dto.lastName}`,
    email: dto.email,
    role: (dto.role?.toLowerCase() as any) || 'staff',
    status: dto.isActive ? 'active' : 'inactive',
    avatar: dto.firstName[0] + (dto.lastName[0] || ''),
    createdAt: new Date(), // Still missing in details, check if we can add it
    lastLogin: dto.lastLoginDate ? new Date(dto.lastLoginDate) : undefined
  };
}

export const usersService = {
  /**
   * Get paginated list of users with filtering and sorting
   */
  getList: async (params: GetUsersParams = {}): Promise<PaginatedResponse<User>> => {
    if (API_CONFIG.useMockData) {
      await delay(API_CONFIG.mockDelay);
      // For mock, we'd need to adapt filterMockUsers but we are moving towards real API
      return { items: [], totalCount: 0, page: 1, pageSize: 10, totalPages: 0 };
    }

    // Map frontend params to backend params
    const backendParams: any = {
      SearchTerm: params.SearchTerm,
      Role: params.Role,
      IsActive: params.IsActive,
      SortBy: params.SortBy,
      SortDescending: params.SortDescending,
      PageNumber: params.PageNumber,
      PageSize: params.PageSize,
    };

    const response = await apiClient.getPaginated<UserListItemDto>('/v1/users', { params: backendParams });

    return {
      ...response,
      items: response.items.map(mapListItemToUser)
    };
  },

  /**
   * Get a single user by ID
   */
  getById: async (id: string): Promise<User | undefined> => {
    if (API_CONFIG.useMockData) {
      await delay(API_CONFIG.mockDelay);
      return mockUsers.find(u => u.id === id);
    }
    const dto = await apiClient.get<UserDetailsDto>(`/v1/users/${id}`);
    return mapDetailsToUser(dto);
  },

  /**
   * Create a new user
   */
  create: async (data: CreateUserCommand): Promise<void> => {
    if (API_CONFIG.useMockData) {
      await delay(API_CONFIG.mockDelay);
      return;
    }
    return apiClient.post<void>('/v1/users', {
      FirstName: data.firstName,
      LastName: data.lastName,
      Email: data.email,
      Role: data.role.charAt(0).toUpperCase() + data.role.slice(1),
    });
  },

  /**
   * Update an existing user
   */
  update: async (data: UpdateUserCommand): Promise<void> => {
    if (API_CONFIG.useMockData) {
      await delay(API_CONFIG.mockDelay);
      return;
    }
    // Backend expects PascalCase for body properties and capitalized role names.
    // Explicitly sending UserId in the body as well as in the URL.
    return apiClient.put<void>(`/v1/users/${data.userId}`, {
      UserId: data.userId,
      FirstName: data.firstName,
      LastName: data.lastName,
      Role: data.role.charAt(0).toUpperCase() + data.role.slice(1),
    });
  },

  /**
   * Activate a user
   */
  activate: async (userId: string): Promise<void> => {
    if (API_CONFIG.useMockData) {
      await delay(API_CONFIG.mockDelay);
      return;
    }
    return apiClient.post<void>(`/v1/users/${userId}/activate`);
  },

  /**
   * Deactivate a user
   */
  deactivate: async (userId: string): Promise<void> => {
    if (API_CONFIG.useMockData) {
      await delay(API_CONFIG.mockDelay);
      return;
    }
    return apiClient.post<void>(`/v1/users/${userId}/deactivate`);
  },

  // Legacy/Deprecated methods
  delete: async (id: string): Promise<{ success: boolean }> => {
    console.warn('Deactivate should be used instead of delete where possible.');
    return apiClient.delete<{ success: boolean }>(`/v1/users/${id}`);
  },
};
