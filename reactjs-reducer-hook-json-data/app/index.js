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

const App = () => <ProductList products={products} />

ReactDOM.render(<App />, document.getElementById('app'))
