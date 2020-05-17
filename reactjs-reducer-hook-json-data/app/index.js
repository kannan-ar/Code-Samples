import React from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import ProductList from './components/ProductList'

const products = [
    {
        "name": 'one',
        "description": 'des',
        "price": 10
    }
]

axios.get(`http://localhost:3000/products`).then(resp => {
    console.log(resp.data);
});

const App = () => <ProductList products={products} />

ReactDOM.render(<App />, document.getElementById('app'))