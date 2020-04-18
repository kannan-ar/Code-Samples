import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';

import Home from './components/home';
import {UserProvider} from './userContext';

class App extends React.Component {
    render() {
        return(
            <UserProvider>
                <Home />
            </UserProvider>
        )
    }
}

ReactDOM.render(<App />, document.getElementById('app'))