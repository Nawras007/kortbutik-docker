FROM node:18-alpine

WORKDIR /app

# Install deps
COPY package*.json ./
RUN npm install --omit=dev

# App files
COPY . .

EXPOSE 3000
ENV NODE_ENV=production

CMD ["node", "app.js"]
