# 🔐 Swagger API Authentication Guide

## ✅ Application Status
Your application is **NOW RUNNING** on: **http://localhost:5006**
Database is seeded with test data. All 53 APIs are ready to test.

---

## 📋 Admin Login Credentials

### Primary Admin Account
```
Email:    admin@hms.com
Password: Admin@123
```

### Demo Users (for testing)

#### Workers
| Email | Password | Hostel |
|-------|----------|--------|
| worker1@hms.com | Worker@123 | Boys Hostel A |
| worker2@hms.com | Worker@123 | Girls Hostel B |
| worker3@hms.com | Worker@123 | Senior Hostel C |

#### Students
| Email | Password | Hostel |
|-------|----------|--------|
| student1@hms.com | Student@123 | Boys Hostel A |
| student2@hms.com | Student@123 | Boys Hostel A |
| student3@hms.com | Student@123 | Girls Hostel B |
| student4@hms.com | Student@123 | Senior Hostel C |

---

## 🌐 Access Swagger UI

Open your browser and navigate to:

## **http://localhost:5006**

You'll see the Swagger UI interface with all 53 API endpoints documented.

---

## 🔑 Authentication Methods

### ⭐ **Method 1: Using Postman (Recommended)**

**Step 1: Get JWT Token**
1. Open Postman
2. Create new POST request to: `http://localhost:5006/api/auth/login`
3. Select **Body** → **Raw** → **JSON**
4. Paste admin credentials:
```json
{
  "email": "admin@hms.com",
  "password": "Admin@123"
}
```
5. Click **Send**
6. Copy the `token` value from the response

**Step 2: Use Token in All Requests**
1. Go to **Headers** tab
2. Add new header:
   - **Key**: `Authorization`
   - **Value**: `Bearer <your_token_here>`
3. All subsequent requests will be authenticated

---

### **Method 2: Using cURL**

**Get Token:**
```bash
curl -X POST "http://localhost:5006/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@hms.com\",\"password\":\"Admin@123\"}"
```

**Use Token in Request:**
```bash
curl -X GET "http://localhost:5006/api/hostels" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

### **Method 3: Using Swagger UI (Manual Token)**

1. Navigate to: **http://localhost:5006**
2. Find the **POST /api/auth/login** endpoint
3. Click **Try it out**
4. Enter JSON body:
```json
{
  "email": "admin@hms.com",
  "password": "Admin@123"
}
```
5. Click **Execute**
6. Scroll down to see response with `token` value
7. Copy the entire token (long string starting with `eyJ...`)
8. Scroll to top and find **🔒 Authorize** button (at the top right)
9. Click **Authorize**
10. Paste token in format: `Bearer eyJhbGciOi...`
11. Click **Authorize** in the dialog
12. All endpoints will now include your token automatically

---

## 🧪 Complete Quickstart Workflow

### 1️⃣ Login & Get Token
```
POST /api/auth/login
Content-Type: application/json

Body:
{
  "email": "admin@hms.com",
  "password": "Admin@123"
}

Response:
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 1,
      "email": "admin@hms.com",
      "fullName": "System Admin",
      "roles": ["Admin"]
    }
  }
}
```

### 2️⃣ Test Protected Endpoint (All require token)
```
GET /api/hostels
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

Response:
{
  "success": true,
  "data": [
    {
      "hostelId": 1,
      "name": "Boys Hostel A",
      "location": "Building A, Floor 1",
      "capacity": 100,
      "description": "Premium boys hostel"
    },
    {
      "hostelId": 2,
      "name": "Girls Hostel B",
      "location": "Building B, Floor 1",
      "capacity": 80,
      "description": "Girls hostel with modern amenities"
    },
    {
      "hostelId": 3,
      "name": "Senior Hostel C",
      "location": "Building C, Floor 2",
      "capacity": 50,
      "description": "Senior hostel for final year students"
    }
  ]
}
```

---

## 📚 API Endpoints by Category

### 🔓 **Authentication (No Token Required)**

#### Login
```
POST /api/auth/login
{
  "email": "admin@hms.com",
  "password": "Admin@123"
}
```

#### Register
```
POST /api/auth/register
{
  "email": "newuser@example.com",
  "password": "Password123",
  "fullName": "John Doe",
  "hostelId": 1
}
```

---

### 🔐 **Protected Endpoints (Token Required)**

#### Get Current User Profile
```
GET /api/auth/me
Authorization: Bearer <token>
```

#### Change Password
```
POST /api/auth/change-password
Authorization: Bearer <token>
{
  "currentPassword": "Admin@123",
  "newPassword": "NewPassword123"
}
```

#### Get All Hostels
```
GET /api/hostels
Authorization: Bearer <token>
```

#### Get All Rooms in Hostel
```
GET /api/hostels/{hostelId}/rooms
Authorization: Bearer <token>
```

#### Get All Students
```
GET /api/students/hostel/{hostelId}
Authorization: Bearer <token>
```

#### Create New Hostel
```
POST /api/hostels
Authorization: Bearer <token>
{
  "name": "New Hostel",
  "location": "Building D",
  "description": "New hostel description",
  "phoneNumber": "+91-9876543210",
  "totalRooms": 50
}
```

#### Raise Complaint
```
POST /api/complaints
Authorization: Bearer <token>
{
  "studentId": 1,
  "title": "Water Leakage",
  "description": "Water leaking from ceiling in room A-101"
}
```

#### Record Fee
```
POST /api/fees
Authorization: Bearer <token>
{
  "studentId": 1,
  "amount": 5000,
  "description": "Hostel fee for April 2024",
  "dueDate": "2024-04-30T00:00:00Z"
}
```

#### Get Dashboard Summary
```
GET /api/hostels/{hostelId}/dashboard
Authorization: Bearer <token>
```

#### Get Cleaning Records
```
GET /api/cleaningrecords/room/{roomId}
Authorization: Bearer <token>
```

---

## 🛠️ Troubleshooting

### ❌ **401 Unauthorized Error**
- **Cause**: Missing or invalid JWT token
- **Cause**: Token has expired (default expiration: 24 hours)
- **Fix**: Get a new token by logging in again with admin credentials
- **Fix**: Ensure Authorization header format is exactly: `Bearer YOUR_TOKEN`

### ❌ **403 Forbidden Error**
- **Cause**: User doesn't have permission for this action
- **Fix**: Make sure you're using admin credentials, not student/worker credentials
- **Fix**: Some endpoints are role-specific

### ❌ **404 Not Found Error**
- **Cause**: Resource doesn't exist (wrong ID or missing data)
- **Fix**: Verify the hostel/room/student ID exists in the system
- **Fix**: Check that Swagger shows 53 endpoints total

### ❌ **500 Internal Server Error**
- **Cause**: Server error during request processing
- **Fix**: Check application logs in terminal
- **Fix**: Verify request body has correct data types and required fields

### ❌ **Database Connection Error**
- **Cause**: PostgreSQL not running
- **Fix**: Ensure PostgreSQL  service is running
- **Fix**: Check connection string in `appsettings.json`

### ❌ **Port 5006 Already in Use**
- **Cause**: Another application using same port
- **Fix**: Kill process: `taskkill /im SmartHostelManagementSystem.exe /f`
- **Fix**: Run on different port: `dotnet run -- --urls="http://localhost:5007"`

---

## 📖 Response Format

All API responses follow a standard format:

### ✅ **Success Response (200 OK)**
```json
{
  "success": true,
  "data": {
    /* response data */
  }
}
```

### ❌ **Error Response (400/401/404/500)**
```json
{
  "success": false,
  "message": "Error description here"
}
```

---

## 🎯 Quick Reference

| Action | Endpoint | Method | Auth |
|--------|----------|--------|------|
| Login | `/api/auth/login` | POST | ❌ |
| Register | `/api/auth/register` | POST | ❌ |
| Get Profile | `/api/auth/me` | GET | ✅ |
| List Hostels | `/api/hostels` | GET | ✅ |
| Create Hostel | `/api/hostels` | POST | ✅ |
| Get Hostel | `/api/hostels/{id}` | GET | ✅ |
| Update Hostel | `/api/hostels/{id}` | PUT | ✅ |
| Delete Hostel | `/api/hostels/{id}` | DELETE | ✅ |
| List Rooms | `/api/hostels/{hostelId}/rooms` | GET | ✅ |
| Get Complaints | `/api/complaints/hostel/{hostelId}` | GET | ✅ |
| Raise Complaint | `/api/complaints` | POST | ✅ |
| Get Fees | `/api/fees/student/{studentId}` | GET | ✅ |
| Record Fee | `/api/fees` | POST | ✅ |
| Get Dashboard | `/api/hostels/{hostelId}/dashboard` | GET | ✅ |

---

## 💡 Pro Tips

1. **Save Token**: Store token in a notepad while testing to avoid repeated logins
2. **Token Valid For**: 24 hours from creation
3. **Multiple Users**: Test with different user accounts (admin, worker, student) to see role-based access
4. **Use Postman**: For better request/response visualization
5. **Check Logs**: Terminal shows all database operations and errors
6. **Export Collection**: Can export all endpoints from Swagger as OpenAPI JSON

---

##  🔗 Important URLs

| Resource | URL |
|----------|-----|
| Application Home | http://localhost:5006 |
| Swagger UI | http://localhost:5006/swagger |
| Swagger JSON | http://localhost:5006/swagger/v1/swagger.json |
| API Base URL | http://localhost:5006/api |
| Login Endpoint | http://localhost:5006/api/auth/login |

---

## ✨ Features Enabled

✅ JWT Bearer Authentication (24-hour expiration)  
✅ Role-Based Access Control (Admin, Student, Worker)  
✅ Comprehensive Error Handling  
✅ Database Seeding with Sample Data (3 Hostels, 30 Rooms, 4 Students, 3 Workers)  
✅ Soft Delete Support (Data Recovery)  
✅ CORS Enabled for Cross-Origin Requests  
✅ Swagger/OpenAPI Documentation  
✅ Structured Logging & Debugging  
✅ PostgreSQL Database  
✅ 53 REST API Endpoints  

---

**Happy API Testing! 🎉**

For detailed endpoint documentation, see `API_ENDPOINTS_COMPLETE.md`

