import React from 'react';
import ProductContext from './ProductContext';
import Product from './Product';

const ProductDisplay = () => {
    const { 
        state: product, 
        dispatch 
      } = React.useContext(ProductContext);

    return <Product {...product} />
}

export default ProductDisplay;

