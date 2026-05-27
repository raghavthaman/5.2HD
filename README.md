# 🎨 Yilpinji — Aboriginal Art Gallery

> A full-stack e-commerce web application for buying, browsing, and celebrating authentic Australian Aboriginal artworks.

---

## 🌐 Live Demo

| Service | URL |
|:--------|:----|
| **Frontend (Vercel)** | [https://aboriginal-art-gallery.vercel.app/](https://aboriginal-art-gallery.vercel.app/) |
| **Backend API (Render)** | [https://aboriginal-art-gallery-api.onrender.com](https://aboriginal-art-gallery-api.onrender.com) |
| **Swagger API Docs** | [https://aboriginal-art-gallery-api.onrender.com/swagger](https://aboriginal-art-gallery-api.onrender.com/swagger) |

> ⚠️ The backend runs on a free Render instance and may take **~30 seconds to wake up** on first request after a period of inactivity.

---

---

## ✨ Features

### 🛍️ For Customers
- Browse 10 authentic Aboriginal artworks with rich descriptions and artist biographies
- Filter artworks by art type, availability, and artist
- Add artworks to cart with real-time stock validation
- **Out of Stock** badges auto-display when stock reaches 0
- Apply promo codes at checkout (`WELCOME20` = 20% off, `MINUS50` = $50 off)
- Secure checkout with billing/delivery address capture
- Gift wrapping options with custom messages
- OTP email verification for registration
- Leave comments on artworks

### 🛠️ For Admins
- Full CRUD management for Artists and Artworks
- Toggle sale availability (on/off) per artwork
- Set stock quantity — storefront auto-reflects Out of Stock
- Manage promo codes
- View all customer orders

---

## 🏗️ Architecture

### Tech Stack

| Layer | Technology |
|:------|:-----------|
| **Frontend** | Vanilla HTML5, CSS3, JavaScript (ES6+) |
| **Backend** | ASP.NET Core 10 Web API (C#) |
| **Database** | PostgreSQL (via Npgsql + EF Core 10) |
| **Auth** | JWT Bearer Tokens |
| **Email** | Gmail SMTP (for OTP) |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper |
| **Containerisation** | Docker (multi-stage build) |
| **Frontend Host** | Vercel |
| **Backend Host** | Render (Docker Web Service) |

### Project Structure

```
.
├── Dockerfile                          # Multi-stage Docker build for backend
├── backend/
│   └── AboriginalArtGallery.API/
│       ├── Controllers/                # API endpoints
│       ├── Data/                       # DbContext + DbInitializer (seeding)
│       ├── DTOs/                       # Request/response data transfer objects
│       ├── Exceptions/                 # Custom exception types
│       ├── Extensions/                 # DependencyInjection.cs (DI, CORS, Swagger)
│       ├── Helpers/                    # ImageUrlHelper
│       ├── Mappings/                   # AutoMapper profile
│       ├── Middleware/                 # GlobalExceptionMiddleware
│       ├── Migrations/                 # EF Core migrations
│       ├── Models/                     # Entity models
│       ├── Repositories/               # Generic + entity-specific repos
│       ├── Services/                   # Business logic layer
│       ├── Validators/                 # FluentValidation validators
│       ├── wwwroot/                    # Static files (images + unified frontend)
│       ├── appsettings.json
│       └── Program.cs
└── frontend/
    ├── index.html
    ├── css/
    └── js/
```

---

## 🚀 Local Development Setup

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Node.js](https://nodejs.org/) (optional — for live reload tooling)

### 1. Clone the Repository
```bash
git clone https://github.com/raghavthaman/5.2HD.git
cd 5.2HD
```

### 2. Configure the Database
Update the connection string in `backend/AboriginalArtGallery.API/appsettings.json`:
```json
"DefaultConnection": "Host=localhost;Port=5432;Database=aboriginal_art_gallery;Username=postgres;Password=YourPassword"
```

### 3. Run Database Migrations
```bash
cd backend/AboriginalArtGallery.API
dotnet ef database update
```

### 4. Start the Backend API
```bash
dotnet run --launch-profile "http"
```
The unified application (frontend + backend) will be available at:
- **App**: http://localhost:5161
- **Swagger**: http://localhost:5161/swagger

> ℹ️ The API automatically seeds 5 artists, 10 artworks, promo codes, and downloads artwork images to `wwwroot/images/` on first startup.

---

## ☁️ Deployment

The application is deployed as a unified Docker container on **Render** with the frontend served as static files from `wwwroot/`.

### Required Environment Variables (Render)

| Key | Description |
|:----|:------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL ADO.NET connection string |
| `JwtSettings__Key` | JWT signing key (min 32 characters) |
| `AppSettings__EnableSwagger` | `true` / `false` |
| `AppSettings__EnableHsts` | `true` / `false` |
| `SmtpSettings__Password` | Gmail App Password for OTP emails |

---

## 🎨 Sample Data

### Artworks (Pre-seeded)
| Title | Artist | For Sale | Price |
|:------|:-------|:---------|:------|
| Awelye (Body Paint Dreaming) | Emily Kame Kngwarreye | ❌ Display Only | — |
| Ghost Gums, MacDonnell Ranges | Albert Namatjira | ❌ Display Only | — |
| Cyclone Tracy | Rover Thomas | ❌ Display Only | — |
| Sandhills, Mina Mina | Dorothy Napangardi | ❌ Display Only | — |
| Namorrorddo (Lightning Spirit) | Yirawala | ❌ Display Only | — |
| Kakadu Rock Art — Mimi Spirits | Yirawala | ❌ Display Only | — |
| Yam Dreaming | Emily Kame Kngwarreye | ✅ For Sale | $480 AUD |
| Rain Dreaming | Dorothy Napangardi | ✅ For Sale | $320 AUD |
| MacDonnell Ranges at Dusk | Albert Namatjira | ✅ For Sale | $750 AUD |
| Kimberley Country | Rover Thomas | ✅ For Sale | $920 AUD |

### Promo Codes
| Code | Type | Value |
|:-----|:-----|:------|
| `WELCOME20` | Percentage | 20% off |
| `MINUS50` | Fixed | $50 off |

---

## 📄 License

This project was developed as part of the SIT331 Task 5.2HD assignment at Deakin University.
