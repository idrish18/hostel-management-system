# 🏨 Hostel Management System - Complete API Documentation

## Overview

This document describes all implemented REST API endpoints for the Hostel Management System. All endpoints are **authenticated** (except Auth endpoints) and require a valid JWT token in the Authorization header.

---

## Authentication Endpoints (`/api/auth`)

These endpoints do NOT require JWT authentication.

### 1. **User Login**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "securePassword123"
}
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "user": {
      "id": 1,
      "email": "user@example.com",
      "fullName": "John Doe"
    }
  }
}
```

### 2. **User Registration**
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "newuser@example.com",
  "password": "securePassword123",
  "fullName": "Jane Smith",
  "hostelId": 1
}
```
**Response (201 Created):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "userId": 2
}
```

### 3. **Get Current User Profile** ⚠️ **[Requires JWT Token]**
```http
GET /api/auth/me
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "user@example.com",
    "fullName": "John Doe",
    "hostelId": 1,
    "roles": ["Student"]
  }
}
```

### 4. **Change Password** ⚠️ **[Requires JWT Token]**
```http
POST /api/auth/change-password
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "currentPassword": "oldPassword123",
  "newPassword": "newPassword456"
}
```
**Response (200 OK):**
```json
{
  "success": true,
  "message": "Password changed successfully"
}
```

---

## Hostels Endpoints (`/api/hostels`) ⚠️ **[Requires JWT Token]**

### 1. **Get All Hostels**
```http
GET /api/hostels
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "hostelId": 1,
      "name": "East Block Hostel",
      "location": "Campus Area",
      "description": "Residential hostel",
      "phoneNumber": "+91-9876543210",
      "totalRooms": 50,
      "occupiedRooms": 45,
      "totalStudents": 120,
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-03-20T15:45:00Z"
    }
  ]
}
```

### 2. **Get Hostel by ID**
```http
GET /api/hostels/{id}
Authorization: Bearer <JWT_TOKEN>
```

### 3. **Create Hostel** 
```http
POST /api/hostels
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "name": "West Block Hostel",
  "location": "North Campus",
  "description": "Girls Hostel",
  "phoneNumber": "+91-9876543211",
  "totalRooms": 60
}
```
**Response (201 Created):** Returns created hostel DTO

### 4. **Update Hostel**
```http
PUT /api/hostels/{id}
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "name": "Updated Hostel Name",
  "location": "New Location",
  "description": "Updated description",
  "phoneNumber": "+91-1234567890",
  "totalRooms": 55
}
```

### 5. **Delete Hostel**
```http
DELETE /api/hostels/{id}
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "message": "Hostel deleted successfully"
}
```

---

## Rooms Endpoints (`/api/hostels/{hostelId}/rooms`) ⚠️ **[Requires JWT Token]**

### 1. **Get All Rooms in Hostel**
```http
GET /api/hostels/1/rooms
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "roomId": 101,
      "hostelId": 1,
      "roomNumber": "A-101",
      "capacity": 4,
      "currentOccupancy": 3,
      "availableSeats": 1,
      "isFull": false,
      "isEmpty": false
    }
  ]
}
```

### 2. **Get Available Rooms**
```http
GET /api/hostels/1/rooms/available
Authorization: Bearer <JWT_TOKEN>
```
Returns only rooms with available seats

### 3. **Get Room by ID**
```http
GET /api/hostels/1/rooms/{roomId}
Authorization: Bearer <JWT_TOKEN>
```

### 4. **Create Room**
```http
POST /api/hostels/1/rooms
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "roomNumber": "A-102",
  "capacity": 4
}
```
**Response (201 Created):** Returns created RoomDto

### 5. **Update Room**
```http
PUT /api/hostels/1/rooms/{roomId}
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "roomNumber": "A-103",
  "capacity": 3
}
```

### 6. **Delete Room**
```http
DELETE /api/hostels/1/rooms/{roomId}
Authorization: Bearer <JWT_TOKEN>
```

### 7. **Check Room Occupancy**
```http
GET /api/hostels/1/rooms/{roomId}/occupancy
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "isFull": false
}
```

---

## Students Endpoints (`/api/students`) ⚠️ **[Requires JWT Token]**

### 1. **Get All Students in Hostel**
```http
GET /api/students/hostel/{hostelId}
Authorization: Bearer <JWT_TOKEN>
```

### 2. **Get Unassigned Students**
```http
GET /api/students/hostel/{hostelId}/unassigned
Authorization: Bearer <JWT_TOKEN>
```
Returns students not assigned to any room

### 3. **Get Students in Room**
```http
GET /api/students/room/{roomId}
Authorization: Bearer <JWT_TOKEN>
```

### 4. **Get Student by ID**
```http
GET /api/students/{studentId}
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "studentId": 1,
    "rollNumber": "STU001",
    "email": "student@example.com",
    "roomId": 101,
    "admissionDate": "2023-08-15T00:00:00Z"
  }
}
```

### 5. **Assign Student to Room**
```http
POST /api/students/{studentId}/assign
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "roomId": 101
}
```
**Response (200 OK):**
```json
{
  "success": true,
  "message": "Student assigned to room successfully",
  "data": { /* StudentDto */ }
}
```

### 6. **Unassign Student from Room**
```http
DELETE /api/students/{studentId}/assign
Authorization: Bearer <JWT_TOKEN>
```

---

## Complaints Endpoints (`/api/complaints`) ⚠️ **[Requires JWT Token]**

### 1. **Get All Complaints in Hostel**
```http
GET /api/complaints/hostel/{hostelId}
Authorization: Bearer <JWT_TOKEN>
```

### 2. **Get Complaints by Status**
```http
GET /api/complaints/hostel/{hostelId}/status/{status}
Authorization: Bearer <JWT_TOKEN>
```
Valid statuses: `Pending`, `Under Review`, `Resolved`, `Closed`

### 3. **Get Complaints by Student**
```http
GET /api/complaints/student/{studentId}
Authorization: Bearer <JWT_TOKEN>
```

### 4. **Get Complaint by ID**
```http
GET /api/complaints/{complaintId}
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "complaintId": 1,
    "studentId": 5,
    "studentName": "John Doe",
    "title": "Water leakage in room",
    "description": "There is water leakage from the ceiling in room A-101",
    "status": "Pending",
    "resolution": null,
    "createdAt": "2024-03-20T10:00:00Z",
    "resolvedAt": null,
    "daysOpen": 3
  }
}
```

### 5. **Raise Complaint**
```http
POST /api/complaints
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "studentId": 5,
  "title": "Water leakage in room",
  "description": "There is water leakage from the ceiling in room A-101"
}
```
**Response (201 Created):** Returns created ComplaintDto

### 6. **Update Complaint Status**
```http
PUT /api/complaints/{complaintId}/status
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "status": "Resolved",
  "resolution": "Ceiling repaired by maintenance team"
}
```

### 7. **Delete Complaint**
```http
DELETE /api/complaints/{complaintId}
Authorization: Bearer <JWT_TOKEN>
```

---

## Fees Endpoints (`/api/fees`) ⚠️ **[Requires JWT Token]**

### 1. **Get Fees by Student**
```http
GET /api/fees/student/{studentId}
Authorization: Bearer <JWT_TOKEN>
```

### 2. **Get All Fees in Hostel**
```http
GET /api/fees/hostel/{hostelId}
Authorization: Bearer <JWT_TOKEN>
```

### 3. **Get Pending Fees**
```http
GET /api/fees/hostel/{hostelId}/pending
Authorization: Bearer <JWT_TOKEN>
```

### 4. **Get Overdue Fees**
```http
GET /api/fees/hostel/{hostelId}/overdue
Authorization: Bearer <JWT_TOKEN>
```

### 5. **Get Fee by ID**
```http
GET /api/fees/{feeId}
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "feeId": 1,
    "studentId": 5,
    "studentName": "John Doe",
    "amount": 5000,
    "amountPaid": 3000,
    "remainingAmount": 2000,
    "status": "Pending",
    "dueDate": "2024-03-31T00:00:00Z",
    "isOverdue": false,
    "receiptNumber": null,
    "createdAt": "2024-03-01T10:00:00Z"
  }
}
```

### 6. **Record Fee**
```http
POST /api/fees
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "studentId": 5,
  "amount": 5000,
  "description": "Hostel fee for March 2024",
  "dueDate": "2024-03-31T00:00:00Z"
}
```
**Response (201 Created):** Returns created FeeDto

### 7. **Record Payment**
```http
POST /api/fees/{feeId}/payment
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "paymentAmount": 3000
}
```
**Response (200 OK):**
```json
{
  "success": true,
  "message": "Payment recorded successfully"
}
```

### 8. **Generate Receipt**
```http
GET /api/fees/{feeId}/receipt
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "receipt": "Receipt details as text/document..."
  }
}
```

### 9. **Delete Fee**
```http
DELETE /api/fees/{feeId}
Authorization: Bearer <JWT_TOKEN>
```

---

## Cleaning Records Endpoints (`/api/cleaningrecords`) ⚠️ **[Requires JWT Token]**

### 1. **Get Cleaning Records for Room**
```http
GET /api/cleaningrecords/room/{roomId}
Authorization: Bearer <JWT_TOKEN>
```

### 2. **Get Today's Cleaning Tasks**
```http
GET /api/cleaningrecords/hostel/{hostelId}/today
Authorization: Bearer <JWT_TOKEN>
```

### 3. **Get Pending Cleaning Tasks**
```http
GET /api/cleaningrecords/hostel/{hostelId}/pending
Authorization: Bearer <JWT_TOKEN>
```

### 4. **Get Cleaning History**
```http
GET /api/cleaningrecords/room/{roomId}/history?days=30
Authorization: Bearer <JWT_TOKEN>
```
`days` parameter is optional (default: 30)

### 5. **Get Cleaning Record by ID**
```http
GET /api/cleaningrecords/{recordId}
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "recordId": 1,
    "roomId": 101,
    "roomNumber": "A-101",
    "status": "Pending",
    "remarks": "Room needs deep cleaning",
    "workerId": 3,
    "assignedAt": "2024-03-20T08:00:00Z",
    "cleanedAt": null,
    "daysOverdue": 2
  }
}
```

### 6. **Create Cleaning Record**
```http
POST /api/cleaningrecords
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "roomId": 101,
  "workerId": 3
}
```
**Response (201 Created):** Returns created CleaningRecordDto

### 7. **Update Cleaning Status**
```http
PUT /api/cleaningrecords/{recordId}/status
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "status": "Completed",
  "remarks": "Room cleaned thoroughly",
  "workerId": 3
}
```

### 8. **Get Daily Cleaning Report**
```http
GET /api/cleaningrecords/hostel/{hostelId}/report?date=2024-03-20
Authorization: Bearer <JWT_TOKEN>
```
`date` parameter is optional (defaults to today)

### 9. **Get Unclean Rooms**
```http
GET /api/cleaningrecords/hostel/{hostelId}/unclean?thresholdDays=3
Authorization: Bearer <JWT_TOKEN>
```
Returns rooms not cleaned for specified days (default: 3)

### 10. **Delete Cleaning Record**
```http
DELETE /api/cleaningrecords/{recordId}
Authorization: Bearer <JWT_TOKEN>
```

---

## Dashboard Endpoints (`/api/hostels/{hostelId}/dashboard`) ⚠️ **[Requires JWT Token]**

### 1. **Get Dashboard Summary**
```http
GET /api/hostels/{hostelId}/dashboard
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "totalRooms": 50,
    "occupiedRooms": 45,
    "totalStudents": 120,
    "pendingComplaints": 5,
    "pendingFees": 25,
    "totalPendingAmount": 125000,
    "averageOccupancy": 90
  }
}
```

### 2. **Get Recent Complaints**
```http
GET /api/hostels/{hostelId}/dashboard/recent-complaints?count=10
Authorization: Bearer <JWT_TOKEN>
```

### 3. **Get Critical Alerts**
```http
GET /api/hostels/{hostelId}/dashboard/alerts
Authorization: Bearer <JWT_TOKEN>
```
**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    "5 pending complaints need urgent attention",
    "Hostel occupancy at 95%",
    "25 students have pending fees totaling ₹125,000"
  ]
}
```

### 4. **Get Utilization Metrics**
```http
GET /api/hostels/{hostelId}/dashboard/metrics/utilization
Authorization: Bearer <JWT_TOKEN>
```

### 5. **Get Complaint Metrics**
```http
GET /api/hostels/{hostelId}/dashboard/metrics/complaints
Authorization: Bearer <JWT_TOKEN>
```

### 6. **Get Fee Metrics**
```http
GET /api/hostels/{hostelId}/dashboard/metrics/fees
Authorization: Bearer <JWT_TOKEN>
```

---

## Error Responses

All endpoints follow standard HTTP status codes:

### Common Error Responses

**400 Bad Request** - Invalid input data
```json
{
  "success": false,
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ]
}
```

**401 Unauthorized** - Missing or invalid JWT token
```json
{
  "success": false,
  "message": "Unauthorized"
}
```

**404 Not Found** - Resource not found
```json
{
  "success": false,
  "message": "Resource not found"
}
```

**500 Internal Server Error** - Server error
```json
{
  "success": false,
  "message": "An error occurred while processing the request"
}
```

---

## Authentication Header Format

All endpoints marked with ⚠️ **[Requires JWT Token]** require the Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Testing the API

### Steps to Test:

1. **Start the application** (already running on http://localhost:5006)

2. **Register/Login** to get JWT token
3. **Use Swagger UI** at `http://localhost:5006/swagger/ui`
4. **Using curl/Postman**:
   ```bash
   # Login
   curl -X POST http://localhost:5006/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"email":"user@example.com","password":"password123"}'

   # Get all hostels (replace TOKEN with actual JWT token)
   curl -X GET http://localhost:5006/api/hostels \
     -H "Authorization: Bearer TOKEN"
   ```

---

## Summary of Implemented Endpoints

| Category | Count | Endpoints |
|----------|-------|-----------|
| **Auth** | 4 | Register, Login, Profile, Change Password |
| **Hostels** | 5 | Get All, Get By ID, Create, Update, Delete |
| **Rooms** | 6 | Get All, Get By ID, Get Available, Create, Update, Delete |
| **Students** | 6 | Get By Hostel, Get Unassigned, Get By Room, Get By ID, Assign, Unassign |
| **Complaints** | 7 | Get All, Get By Status, Get By Student, Get By ID, Raise, Update Status, Delete |
| **Fees** | 9 | Get By Student, Get By Hostel, Get Pending, Get Overdue, Get By ID, Record, Payment, Receipt, Delete |
| **Cleaning Records** | 10 | Get By Room, Today's Tasks, Pending, History, By ID, Create, Update Status, Report, Unclean Rooms, Delete |
| **Dashboard** | 6 | Summary, Recent Complaints, Alerts, Utilization Metrics, Complaint Metrics, Fee Metrics |
| **Total** | **53** | **Complete REST API** |

---

**✅ All APIs are fully implemented and tested!**
