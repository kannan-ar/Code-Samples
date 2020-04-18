import React from 'react';
import {UserConsumer} from '../userContext';
import User from './user';

//Seperating user context consumer and user components increases resuability. The user component can be reuse in some places
class UserContainer extends React.Component
{
    render() {
        return(
            <UserConsumer>
                {
                    user => (<User user={user} />)
                }
            </UserConsumer>
        )
    }
}

export default UserContainer