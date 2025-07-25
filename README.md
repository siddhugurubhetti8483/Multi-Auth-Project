## 🗒️ **Project Documentation Template**

### Project Name:

### **Multi-Auth Project**

---

# 🔐 MultiAuthAPI - Secure Authentication & Authorization System

A production-grade authentication and authorization system built using **ASP.NET Core Web API** with multiple secure login methods including:

- Email/Password Login
- Google OAuth Login
- Face ID Login (Webcam Based)
- OTP & Multi-Factor Authentication (MFA)
- Role-Based Access Control (Admin/Customer)
- Refresh Tokens
- Email Verification
- Forgot Password with OTP
- Account Lock after 3 wrong attempts
- Admin Dashboard APIs

---

## 🚀 Tech Stack

- ASP.NET Core Web API (.NET 7+)
- MSSQL (EF Core)
- JWT (JSON Web Tokens)
- Google OAuth 2.0
- Face Recognition via ML.NET + Webcam
- BCrypt for password hashing
- Swagger (API Testing)
- Angular (frontend planned)

---

## 📦 Features Covered

Frontend will be developed using Angular 15+

---

### 🔐 Authentication & Authorization

- Register user with email
- Login with email/password (JWT & refresh token)
- Google OAuth Login (ExternalLoginController)
- Face Upload and Face Login (using base64 image)
- Multi-Factor Authentication (MFA)
- OTP-based login and password reset
- Role-based access (Admin/Customer)
- Email verification with Base64 token
- Account lockout after 3 failed attempts

---

## 🧑‍💻 User Management

- Get All Users (admin only)
- Change User Role (admin only)
- View Logged-in User Profile
- Admin Dashboard & User Stats

---

## ⚙️ API Endpoints

> ✅ Fully tested via Swagger & Postman

### AuthController

- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `POST /api/Auth/refresh-token`
- `POST /api/Auth/upload-face`
- `POST /api/Auth/face-login`
- `GET /api/Auth/verify-email`

### ExternalLoginController

- `GET /api/ExternalLogin/google-login`
- `GET /api/ExternalLogin/google-callback`

### ForgotPasswordController

- `POST /api/ForgotPassword/request-otp`
- `POST /api/ForgotPassword/reset`

### MfaController

- `POST /api/Mfa/login`
- `POST /api/Mfa/verify`

### OtpController

- `POST /api/Otp/send`
- `POST /api/Otp/verify`

### UserController

- `GET /api/User/all`
- `PUT /api/User/change-role`
- `GET /api/User/profile`
- `GET /api/User/admin-panel`
- `GET /api/User/dashboard`

---

## 📁 Folder Structure

```bash
📦MultiAuthAPI
 ┣ 📂Controllers
 ┣ 📂DTOs
 ┣ 📂Models
 ┣ 📂Services
 ┣ 📂Data
 ┣ 📂Helpers
 ┣ appsettings.json
 ┣ Program.cs
 ┣ Startup logic (inside Program.cs)
```

### 🧪 Testing Tools:

- Postman (for APIs)
- Gmail SMTP (for email OTP)
- JWT.io (for decoding JWT)
- Ngrok (if frontend testing with local backend)

---

## ✅ Important Notes:

- Use `[Authorize]` for protected routes
- JWT expiration time: 30 min
- OTP expiry: 2 minutes
- Google OAuth needs redirect URI set in Google Console
