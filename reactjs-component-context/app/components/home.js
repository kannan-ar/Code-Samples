import React from 'react';
import UserContainer from './userContainer';
import {UserProvider} from '../userContext';

class Home extends React.Component
{
    getUser() {
        return {
            name: "specific user"
        }
    }

    //Here the second div overrides the global user provider from index.js
    render() {
        return (
            <div>
                <div style={{textAlign: "right"}}>
                    <UserContainer />
                </div>
                <div style={{textAlign:"right"}}>
                    <UserProvider value={this.getUser()}>
                        <UserContainer />
                    </UserProvider>
                </div>
                <div>
                    Content Section
                </div>
            </div>
        )
    }
}

export default Home