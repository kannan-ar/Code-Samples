import React from 'react';

const Product = ({ name, description, price }) => (
    <dl>
        <dt>Product Name:</dt>
        <dd>{name}</dd>
        <dt>Product Description:</dt>
        <dd>{description}</dd>
        <dt>Product Price:</dt>
        <dd>{price}</dd>
    </dl>
)

export default Product