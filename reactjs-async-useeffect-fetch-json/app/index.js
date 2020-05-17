import React, {useEffect, useState} from 'react';
import ReactDOM from 'react-dom';
import {Config} from './config'

const App = () => {
    const [config, setConfig] = useState({});

    useEffect(() => {
        (async () => {
            const result = await Config()
            setConfig(result)
        })()
    }, [])

    return(
    <div>Hello World {config.serverUrl}</div>
    ) 
}

ReactDOM.render(<App />, document.getElementById('app'))