import React from 'react';

class User extends React.Component
{
    render() {
        return(
            <div>{this.props.user.name}</div>
        )
    }
}

export default User