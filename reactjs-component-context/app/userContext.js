import React from 'react';

const UserContext = React.createContext();

const {Provider: Provider, Consumer: UserConsumer} = React.createContext();

class UserProvider extends React.Component
{
    constructor(props) {
        super(props);
    }

    render() {
        let user = (this.props.value === undefined || this.props.value === null)? 
            this.getUser(): this.props.value;

        return(
            <Provider value={user}>
                {this.props.children}
            </Provider>
        )
    }

    getUser() {
        return {
            name: "default user"
        }
    }
}

export  {UserProvider, UserConsumer};