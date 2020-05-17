import React, { useEffect } from 'react'
import {connect} from 'react-redux'

import {fetchProducts} from '../actions/productActions'

const Products = ({dispatch, loading, products, hasErrors}) => {

    useEffect(() => {
        dispatch(fetchProducts())
    }, [dispatch])

    const renderProducts = () => {
        if(loading) return <p>Loading...</p>
        if(hasErrors) return <p>Unable to display posts</p>
        return products.map(product => <div>{product.name}</div>)
    }

    return (
        <div>
            <h1>Products</h1>
            {renderProducts()}
        </div>
    )
}

const mapStateToProps = state => ({
    loading: state.products.loading,
    products: state.products.products,
    hasErrors: state.products.hasErrors
})

export default connect(mapStateToProps)(Products)