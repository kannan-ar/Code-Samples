import * as actions from '../actions/productActions'

export const initalState = {
    products: [],
    loading: false,
    hasErrors: false
}

export default function productReducer(state = initalState, action) {
    switch(action.type) {
        case actions.GET_PRODUCTS:
            return {...state, loading: true}
            break;
        case actions.GET_PRODUCTS_SUCCESS:
            return {products: action.payload, loading: false, hasErrors: false}
            break;
        case actions.GET_PRODUCTS_FAILURE:
            return {...state, loading:false, hasErrors: true}
        default:
            return state
    }
}