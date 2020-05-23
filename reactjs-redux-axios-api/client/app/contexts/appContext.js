import React, {useEffect, useState} from 'react';
import {config} from '../config'

const AppContext = React.createContext();

const AppProvider = (props) => {
    const [configData, setConfig] = useState({});

    useEffect(() => {
        const fetchContext = async () => {
            const result = await config()
            setConfig(result)
            
        }
       fetchContext()
    },[])

    return(
        <AppContext.Provider value={configData}>
            {props.children}
        </AppContext.Provider>
    )
}

export {AppProvider, AppContext}