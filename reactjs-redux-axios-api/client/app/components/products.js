import React, { useEffect } from 'react'
import { connect } from 'react-redux'

import { fetchProducts } from '../actions/productActions'
import { AppContext } from '../contexts/appContext'

const Products = ({ dispatch, loading, products, hasErrors }) => {
    const context = React.useContext(AppContext)

    useEffect(() => {
        if (context.serverUrl) {
            dispatch(fetchProducts(context.serverUrl))
        }
    },[context, dispatch])

    const renderProducts = () => {
        if (loading) return <p>Loading...</p>
        if (hasErrors) return <p>Unable to display posts</p>
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