const express = require('express');
const fs = require('fs');

const app = express();
const port = 3000;

app.get('/', (req, res) => res.send('Welcome to Sales Hub'));

app.get('/users', (req, res) => {
    fs.readFile(__dirname + "/db/users.json", "utf8", (err, data) => {
        res.end(data);
    });
});

app.get('/products', (req, res) => {
    fs.readFile(__dirname + "/db/products.json", "utf8", (err, data) => {
        res.end(data);
    });
});

app.get('/carriers', (req, res) => {
    fs.readFile(__dirname + "/db/carriers.json", "utf8", (err, data) => {
        res.end(data);
    });
});

app.listen(port, () => console.log(`Sales Hub is now listening at ${port}`));