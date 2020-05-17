import axios from 'axios'
import {config} from '../config'

export const GET_PRODUCTS = "GET PRODUCTS"
export const GET_PRODUCTS_SUCCESS = "GET PRODUCTS SUCCESS"
export const GET_PRODUCTS_FAILURE = "GET PRODUCTS FAILURE"

export const getProducts = () => ({
    type: GET_PRODUCTS
})

export const getProductsSuccess = products => ({
    type: GET_PRODUCTS_SUCCESS,
    payload: products
})

export const getProductsFailure = () => ({
    type: GET_PRODUCTS_FAILURE
})

export const fetchProducts = () => {
    return async dispatch => {
        dispatch(getProducts())

        try {
            const configData = await config()
            const data = await axios(configData.serverUrl).then(resp => resp.data)
            dispatch(getProductsSuccess(data))
        }
        catch(error) {
            dispatch(getProductsFailure())
        }
    }
}