# Frontend Setup Playbook (Angular)

## Goal

Reusable steps to bootstrap an Angular frontend for API-driven projects.

---

## 1. Environment Setup

### Install Node Version Manager (nvm)

```bash
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.7/install.sh | bash
source ~/.bashrc
```

### Install Node (required for Angular CLI)

```bash
nvm install 20
nvm use 20
node -v   # verify >= v20
```

### Install Angular CLI

```bash
npm install -g @angular/cli
ng version
```

**Why:** Angular CLI requires Node 20+. Using nvm avoids system conflicts.

---

## 2. Project Structure

From repo root:

```bash
mkdir frontend
cd frontend
ng new smart-home-ui
```

### CLI Options Chosen

* Routing: Yes
* Styles: CSS
* SSR: No
* AI tools: None (or GitHub Copilot if desired)

**Why:**

* Keep frontend separate from backend
* Avoid unnecessary complexity (SSR, Tailwind, etc.)

---

## 3. Run the App

```bash
cd smart-home-ui
ng serve
```

Open:

```
http://localhost:4200
```

---

