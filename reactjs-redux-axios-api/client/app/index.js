import React from 'react';
import ReactDOM from 'react-dom';
import {createStore, applyMiddleware} from 'redux'
import {Provider} from 'react-redux'
import thunk from 'redux-thunk'

import rootReducer from './reducers'
import App from './app'
import {AppProvider} from './contexts/appContext'

const store = createStore(rootReducer, applyMiddleware(thunk))

ReactDOM.render(<Provider store={store}><AppProvider><App /></AppProvider></Provider>, document.getElementById('app'))