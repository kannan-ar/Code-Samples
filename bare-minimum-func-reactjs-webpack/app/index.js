import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';

import Home from './components/home'

const App = () => {
    return(
        <Home />
    )
}

ReactDOM.render(<App />, document.getElementById('app'))