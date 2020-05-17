import React from 'react';

const useProductReducer = (product) => {
    const middleware = (currentState, dispatchArgument) => {
        switch(dispatchArgument.type) {
            case `changeName`:
                return changeProductName(currentState, dispatchArgument.payload);
            case `changePrice`:
                return {...state, price: dispatchArgument.payload};
        };
    };

    const initalState = product;

    const [state, dispatch] = React.useReducer(middleware, initalState);
    return {state, dispatch};
};

export default useProductReducer;