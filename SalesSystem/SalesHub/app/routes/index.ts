import express, { Router } from 'express';

const indexRouter: Router = express.Router();

indexRouter.get('/', (req, res) => res.send('Welcome to Sales Hub'));

export default indexRouter;