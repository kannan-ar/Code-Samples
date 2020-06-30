import SocketIO from 'socket.io';

let interval: NodeJS.Timeout;

const listenPurchaseSocket = (io: SocketIO.Server) => {
    io.on("connection", (socket) => {
        console.log("New client connected");

        if(interval) {
            clearInterval(interval);
        }

        interval = setInterval(() => {
            getPurchaseLog(socket);
        }, 1000);

        socket.on("disconnect", () => {
            console.log("Client disconnected");
            clearInterval(interval);
        });
    });
}

const getPurchaseLog = (socket: SocketIO.Socket) => {
    const response = new Date();
    socket.emit("FromAPI", response);
}

export default listenPurchaseSocket;