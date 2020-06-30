import express from 'express';
import http from 'http';
import SocketIO from 'socket.io';

import indexRouter from './routes/index';
import salesRouter from './routes/sales';
import listenPurchaseSocket from './routes/purchase';

const port = process.env.PORT || 3000;
const app: express.Application = express();

app.use(indexRouter);
app.use(salesRouter);

const server = http.createServer(app);
const io = SocketIO(server);

listenPurchaseSocket(io);

server.listen(port, () => console.log(`Listening on the port ${port}`));
