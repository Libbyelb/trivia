# Trivia App

Deze repository bevat:
- `backend`: ASP.NET Core Web API (`net8.0`)
- `frontend`: React-app (Create React App + TypeScript)

## Vereisten

Installeer vooraf:
- .NET SDK 8.0+
- Node.js 18+ (met npm)

Controleer versies:

```bash
dotnet --version
node --version
npm --version
```

## Applicatie bouwen

### 1) Backend bouwen

```bash
cd backend
dotnet restore
dotnet build
```

### 2) Frontend bouwen

```bash
cd frontend
npm install
npm run build
```

## Applicatie lokaal uitvoeren

Gebruik bij voorkeur 2 terminals: een voor backend en een voor frontend.

### Terminal 1 - Backend starten

```bash
cd backend
dotnet run
```

De API draait standaard op:
- `http://localhost:5210`

### Terminal 2 - Frontend starten

```bash
cd frontend
npm install
npm start
```

Frontend URL:
- `http://localhost:3000`

## Belangrijke opmerking (CORS)

De backend staat requests toe vanaf `http://localhost:3000`.
Zorg dus dat de frontend op deze URL draait in development.

