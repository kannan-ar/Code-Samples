import React from 'react';

import ProductContext from './ProductContext';
import ProductDisplay from './ProductDisplay';
import useProductReducer from './reducers/ProductReducer';

const ProductList = ({products}) => products.map(
    product => <ProductContext.Provider value={useProductReducer(product)}>
                    <ProductDisplay />
                </ProductContext.Provider>
);

export default ProductList