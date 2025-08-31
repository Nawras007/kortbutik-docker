const express = require('express');
const axios = require('axios');
const cors = require('cors');
const dotenv = require('dotenv');
const path = require('path');


dotenv.config();

const app = express();
const PORT = process.env.PORT || 3000;

// Inside Docker the backend is reachable as http://backend:8080
// When running locally (not containerized), use http://localhost:5000
const NET_API_URL = process.env.BASE_URL || 'http://backend:8080';
console.log('Using backend API:', NET_API_URL);
const BACKEND_URL =
  process.env.BASE_URL ||
  (process.env.NODE_ENV === 'production' ? 'http://backend:8080' : 'http://localhost:5000');

app.use(express.json());
app.use(cors());

// Serve all static files (HTML, JS, CSS, images)
app.use(express.static(path.join(__dirname)));
app.use('/css', express.static(path.join(__dirname, 'css')));
app.use('/images', express.static(path.join(__dirname, 'images')));

// HTML routes (optional—static middleware above already serves these)
app.get(['/','/index.html'], (req, res) => {
  res.sendFile(path.join(__dirname, 'index.html'));
});
app.get('/products.html', (req, res) => {
  res.sendFile(path.join(__dirname, 'products.html'));
});
app.get('/about.html', (req, res) => {
  res.sendFile(path.join(__dirname, 'about.html'));
});

// API proxy -> .NET backend
app.get('/api/cards', async (req, res) => {
  try {
    const response = await axios.get(`${BACKEND_URL}/cards`, {
      headers: { accept: 'application/json' },
      timeout: 10000
    });
    res.json(response.data);
  } catch (err) {
    const status = err.response?.status || 500;
    const detail = err.response?.data || err.message;
    console.error('Failed to fetch cards from backend:', {
      backend: BACKEND_URL,
      status,
      detail
    });
    res.status(status).json({ error: 'Failed to fetch Cards', detail });
  }
});

// Small health check
app.get('/api/health', async (_req, res) => {
  try {
    const r = await axios.get(`${BACKEND_URL}/swagger/v1/swagger.json`, { timeout: 5000 });
    res.json({ ok: true, backend: BACKEND_URL, swagger: !!r.data?.openapi || !!r.data?.info });
  } catch {
    res.status(500).json({ ok: false, backend: BACKEND_URL });
  }
});

app.listen(PORT, () => {
  console.log(`Frontend running at http://localhost:${PORT}`);
  console.log(`Using backend: ${BACKEND_URL}`);
});
