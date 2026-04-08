// Auth types
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  createdAt: string;
}

export interface AuthToken {
  accessToken: string;
  refreshToken?: string;
  expiresIn: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role?: UserRole;
}

export enum UserRole {
  ADMIN = 'Admin',
  STUDENT = 'Student',
  WORKER = 'Worker',
}

// Hostel types
export interface Hostel {
  id: string;
  name: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  totalRooms: number;
  occupiedRooms: number;
  createdAt: string;
}

export interface CreateHostelRequest {
  name: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
}

// Room types
export interface Room {
  id: string;
  hostelId: string;
  roomNumber: string;
  capacity: number;
  occupancy: number;
  available: boolean;
  createdAt: string;
}

export interface CreateRoomRequest {
  hostelId: string;
  roomNumber: string;
  capacity: number;
}

// Student types
export interface Student {
  id: string;
  userId: string;
  registrationNumber: string;
  roomId?: string;
  createdAt: string;
}

export interface AssignRoomRequest {
  studentId: string;
  roomId: string;
}

// Complaint types
export interface Complaint {
  id: string;
  studentId: string;
  title: string;
  description: string;
  status: ComplaintStatus;
  createdAt: string;
  updatedAt: string;
}

export enum ComplaintStatus {
  OPEN = 'Open',
  IN_PROGRESS = 'InProgress',
  RESOLVED = 'Resolved',
  CLOSED = 'Closed',
}

export interface CreateComplaintRequest {
  title: string;
  description: string;
}

export interface UpdateComplaintStatusRequest {
  status: ComplaintStatus;
}

// Fee types
export interface Fee {
  id: string;
  studentId: string;
  amount: number;
  status: FeeStatus;
  dueDate: string;
  paidDate?: string;
  createdAt: string;
}

export enum FeeStatus {
  PENDING = 'Pending',
  PAID = 'Paid',
  OVERDUE = 'Overdue',
}

export interface RecordFeeRequest {
  studentId: string;
  amount: number;
  dueDate: string;
}

export interface RecordPaymentRequest {
  feeId: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
