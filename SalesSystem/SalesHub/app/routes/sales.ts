import express, { Router } from 'express';
import fs from 'fs';
import path from 'path';

const salesRouter: Router = express.Router();

salesRouter.get('/users', (req, res) => {
    fs.readFile(path.resolve("./db/users.json"), "utf8", (err, data) => {
        res.end(data);
    });
});

salesRouter.get('/products', (req, res) => {
    fs.readFile(path.resolve("./db/products.json"), "utf8", (err, data) => {
        res.end(data);
    });
});

salesRouter.get('/carriers', (req, res) => {
    fs.readFile(path.resolve("./db/carriers.json"), "utf8", (err, data) => {
        res.end(data);
    });
});

export default salesRouter;