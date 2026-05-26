# Yilpinji — Aboriginal Art Gallery

## Backend Setup (ASP.NET Core API + PostgreSQL)

1. **Database Configuration**
   - The application expects a PostgreSQL instance running on your machine.
   - You can update the `DefaultConnection` string in `backend/AboriginalArtGallery.API/appsettings.Development.json` (or `appsettings.json`) if your Postgres username/password differs from the default (`postgres`/`Pass123`).

2. **Run Migrations**
   - Open a terminal in `backend/AboriginalArtGallery.API`
   - Run `dotnet ef database update`
   - This will create the `aboriginal_art_gallery` database, tables, and insert sample Promo Code data (`WELCOME20` for 20% off, `MINUS50` for $50 off).

3. **Start the API**
   - Run `dotnet run` from the `backend/AboriginalArtGallery.API` directory.
   - The API will start on `http://localhost:5000` (or another port depending on your setup).
   - Swagger documentation will be available at `http://localhost:5000/swagger`.

## Frontend Setup (Vanilla HTML/JS/CSS)

1. **Serving the Frontend**
   - Simply open `frontend/index.html` in your web browser. 
   - Alternatively, you can run a local server (e.g., using VS Code Live Server). 
   - Note: The API CORS policy is configured to allow requests from `http://localhost:3000`, `http://127.0.0.1:5500`, and `null` (local file access).

2. **Testing the App**
   - You can browse the gallery without an account.
   - To make a purchase or leave a comment, click "Register" and create an account.
   - Sample Gift Card for testing: `GC-12345678` (any string matching `GC-` followed by 8 alphanumeric characters will work in the dummy validation logic).
