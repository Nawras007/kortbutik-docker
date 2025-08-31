# Administrera Molntjänster - Inlämningsuppgift 1

## 📦 Innehåll

- ASP.NET Core backend (Docker)
- Express static frontend (Docker)
- SQL Server container
- Docker Compose setup

## 🚀 Starta applikationen

Krav: Docker och Docker Compose installerat.

```bash
docker-compose up --build
```

Frontend körs på: http://localhost:3000  
Backend körs på: http://localhost:5000

## 🛠️ Teknisk info

- **Backend:** ASP.NET Core (.NET 6)
- **Frontend:** Node.js + Express (statiska filer)
- **Databas:** SQL Server (container)
- Allt är containeriserat och konfigurerat med Docker Compose.

## 📁 Struktur

- `/frontend` – Express server som serverar statiska filer
- `/backend` – ASP.NET Core-applikation
- `/db` – Init-fil för SQL

## 🔁 Version 2

För att skapa version 2:
1. Gör en kodändring (t.ex. ändra titeln i HTML eller en API-route).
2. Commita som `version 2`.
3. Push till GitHub.
